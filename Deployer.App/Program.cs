using System.IO;
using Deployer.App.Hardware;
using Deployer.App.Micro;
using Deployer.App.WebResponders;
using Deployer.Services.Api;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Gadgeteer;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NeonMika;
using System;
using System.Threading;
using NeonMika.Interfaces;

namespace Deployer.App
{
    public partial class Program
    {
        private IDeployerController _controller;

        private Gadgeteer.Timer _timerBlink;

        private InterruptPort _keySwitchA;
        private InterruptPort _keySwitchB;
        private InterruptPort _buttonUp;
        private InterruptPort _buttonDown;
        private InterruptPort _buttonArm;
        private InterruptPort _buttonDeploy;

        private ILed _indicatorTurnKeyA;
        private ILed _indicatorTurnKeyB;
        private ILed _indicatorSelectProject;
        private ILed _indicatorReadyToArm;
        private ILed _indicatorReadyToDeploy;
        private ILed _indicatorStateDeploying;
        private ILed _indicatorStateSucceeded;
        private ILed _indicatorStateFailed;

        private NetworkWrapper _network;
        private WebServer _webServer;
        private IPersistence _storage;
        private string _rootDir;
        private GarbageCollector _garbage;
        private INeonLogger _logger;
        private IConfigurationService _configService;

        private void ProgramStarted()
        {
            var memory = Debug.GC(true);
            Debug.Print("Memory at startup = " + memory);

            SetupEthernet();
            SetupPersistence();

            SetupInputs();
            SetupIndicators();

            _garbage = new GarbageCollector();
            _logger = new Logger();
            var smallIo = new SmallTextFileIo();
            var jsonPersist = new JsonPersistence(smallIo);
            var slugCreator = new SlugCreator();
            _configService = new FakeConfigurationService();
                // RealConfigurationService(_rootDir, jsonPersist, slugCreator);
            var charDisp = new CharDisplay(characterDisplay);
            var keys = new SimultaneousKeys(ReversedSwitchA, ReversedSwitchB, new TimeService());
            var webFactory = new WebRequestFactory();
            var project = new ProjectSelector(charDisp, _configService);
            var sound = new Sound(tunes);
            var webu = new WebUtility(_garbage);

            var indicators = new Indicators(_indicatorTurnKeyA,
                                            _indicatorTurnKeyB,
                                            _indicatorSelectProject,
                                            _indicatorReadyToArm,
                                            _indicatorReadyToDeploy,
                                            _indicatorStateDeploying,
                                            _indicatorStateSucceeded,
                                            _indicatorStateFailed);
            var context = new DeployerContext(keys,
                                              project,
                                              charDisp,
                                              indicators,
                                              sound,
                                              webu,
                                              _network,
                                              webFactory,
                                              _garbage,
                                              _configService);
            _controller = new DeployerController(context);
            context.SetController(_controller);
            _controller.PreflightCheck();

            //SetupWebServer();

            SetupInterrupts();
            SetupTimers();
        }

        #region Setup methods

        // Try mIP? http://mip.codeplex.com/
        private void SetupEthernet()
        {
            try
            {
                characterDisplay.Clear();
                characterDisplay.SetCursorPosition(0, 0);
                characterDisplay.Print("Getting IP address...");

                NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
                NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

                var eth = Mainboard.Ethernet;
                eth.Open();
                eth.EnableDhcp();
                eth.EnableDynamicDns();
                while (eth.IPAddress == "0.0.0.0")
                {
                    Debug.Print("Waiting for DHCP");
                    Thread.Sleep(250);
                }
                _network = new NetworkWrapper(eth);
            }
            catch (Exception ex)
            {
                Debug.Print("Could not set up Ethernet - " + ex);
                throw;
            }
        }

        private void SetupWebServer()
        {
            _webServer = new WebServer(_logger, _garbage);

            var authApiService = new AuthApiService(_configService, _garbage);
            var authResponder = new ApiServiceResponder(authApiService);
            _webServer.AddResponse(authResponder);

            var configApiService = new ConfigApiService(_configService, _garbage);
            var configResponder = new ApiServiceResponder(configApiService);
            _webServer.AddResponse(configResponder);

            var updateClient = new FilePutResponder(_rootDir);
            _webServer.AddResponse(updateClient);

            var fileServe = new FileGetResponder(_rootDir, "client");
            _webServer.AddResponse(fileServe);
        }

        private void SetupInputs()
        {
            _keySwitchA = SetupInterruptOffAndOn(PinsCerbuino.A0);
            _keySwitchB = SetupInterruptOffAndOn(PinsCerbuino.A1);

            _buttonUp = SetupInterruptRelease(PinsCerbuino.D2);
            _buttonDown = SetupInterruptRelease(PinsCerbuino.A2);
            _buttonArm = SetupInterruptRelease(PinsCerbuino.D4);
            _buttonDeploy = SetupInterruptRelease(PinsCerbuino.D5);
        }

        private void SetupIndicators()
        {
            _indicatorTurnKeyA = SetupHeaderOutput(PinsCerbuino.D6);
            _indicatorTurnKeyB = SetupHeaderOutput(PinsCerbuino.D7);

            _indicatorSelectProject = SetupHeaderOutput(PinsCerbuino.D8);
            _indicatorReadyToArm = SetupHeaderOutput(PinsCerbuino.D9);

            _indicatorReadyToDeploy = SetupHeaderOutput(PinsCerbuino.D10);

            _indicatorStateDeploying = SetupBreakoutOutput(Socket.Pin.Three);
            _indicatorStateSucceeded = SetupBreakoutOutput(Socket.Pin.Four);
            _indicatorStateFailed = SetupBreakoutOutput(Socket.Pin.Five);
        }

        private void SetupInterrupts()
        {
            _keySwitchA.OnInterrupt += KeySwitchAOnInterrupt;
            _keySwitchB.OnInterrupt += KeySwitchBOnInterrupt;

            _buttonUp.OnInterrupt += ButtonUpOnInterrupt;
            _buttonDown.OnInterrupt += ButtonDownOnInterrupt;

            _buttonArm.OnInterrupt += ButtonArmOnInterrupt;
            _buttonDeploy.OnInterrupt += ButtonDeployOnInterrupt;
        }

        private void SetupTimers()
        {
            _timerBlink = new Gadgeteer.Timer(500);
            _timerBlink.Tick += BlinkTick;
            _timerBlink.Start();
        }

        private void SetupPersistence()
        {
            try
            {
                _storage = new Persistence(Mainboard.SDCardStorageDevice);
                var isStorage = Mainboard.IsSDCardInserted;
                if (!isStorage)
                    throw new Exception("No SD card has been inserted");
                _rootDir = Mainboard.SDCardStorageDevice.Volume.RootDirectory;
                var authDir = Path.Combine(_rootDir, "auth");
                var configDir = Path.Combine(_rootDir, "config");
                var clientDir = Path.Combine(_rootDir, "client");
                if (!Directory.Exists(authDir))
                    Directory.CreateDirectory(authDir);
                if (!Directory.Exists(configDir))
                    Directory.CreateDirectory(configDir);
                if (!Directory.Exists(clientDir))
                    Directory.CreateDirectory(clientDir);
            }
            catch (Exception ex)
            {
                Debug.Print("Could not initialize storage - " + ex);
            }
        }

        #endregion

        #region Key switches

        private bool ReversedSwitchA
        {
            get { return !_keySwitchA.Read(); }
        }

        private bool ReversedSwitchB
        {
            get { return !_keySwitchB.Read(); }
        }

        private void KeySwitchAOnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (ReversedSwitchA)
                _controller.KeyOnEvent(KeySwitch.KeyA);
            else
                _controller.KeyOffEvent(KeySwitch.KeyA);
        }

        private void KeySwitchBOnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (ReversedSwitchB)
                _controller.KeyOnEvent(KeySwitch.KeyB);
            else
                _controller.KeyOffEvent(KeySwitch.KeyB);
        }

        #endregion

        #region Push buttons

        private void ButtonUpOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _controller.UpPressedEvent();
        }

        private void ButtonDownOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _controller.DownPressedEvent();
        }


        private void ButtonArmOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _controller.ArmPressedEvent();
        }

        private void ButtonDeployOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _controller.DeployPressedEvent();
        }

        #endregion

        #region Indicator outputs

        private Led SetupHeaderOutput(Cpu.Pin pin)
        {
            try
            {
                var port = new OutputPort(pin, false);
                return new Led(port);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
            return new Led();
        }

        private LedDigital SetupBreakoutOutput(Socket.Pin pin)
        {
            var output = breakoutTB10.CreateDigitalOutput(pin, false);
            return new LedDigital(output);
        }

        #endregion

        #region Interrupt setup

        private InterruptPort SetupInterruptOffAndOn(Cpu.Pin pin)
        {
            return new InterruptPort(pin, true,
                                     Port.ResistorMode.PullUp,
                                     Port.InterruptMode.InterruptEdgeBoth);
        }

        private InterruptPort SetupInterruptRelease(Cpu.Pin pin)
        {
            return new InterruptPort(pin, true,
                                     Port.ResistorMode.PullUp,
                                     Port.InterruptMode.InterruptEdgeLow);
        }

        #endregion

        #region Network up/down

        private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Debug.Print("NetworkChange_NetworkAvailabilityChanged");
        }

        private void OnNetworkAddressChanged(object sender, EventArgs e)
        {
            Debug.Print("NetworkChange_NetworkAddressChanged");
        }

        #endregion

        #region Blink

        private void BlinkTick(Gadgeteer.Timer timer)
        {
            _controller.Tick();
        }

        #endregion
    }
}