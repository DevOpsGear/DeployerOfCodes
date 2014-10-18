using Deployer.App.Abstraction;
using Deployer.Services.Abstraction;
using Deployer.Services.Input;
using Deployer.Services.StateMachine;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.IO;

namespace Deployer.App
{
    public partial class Program
    {
        private InterruptPort _keySwitchA;
        private InterruptPort _keySwitchB;
        private InterruptPort _buttonUp;
        private InterruptPort _buttonDown;
        private InterruptPort _buttonArm;
        private InterruptPort _buttonDeploy;

        //private WebServer _webServer;
        private string _rootDir;
        private Gadgeteer.Timer _timerBlink;

        private IDeployerController _controller;

        private void ProgramStarted()
        {
            var memory = Debug.GC(true);
            Debug.Print("Memory at startup = " + memory);

            SetupPersistence();
            SetupInputs();

            var factory = new RealDeployerFactory(Mainboard, breakoutTB10, characterDisplay, tunes);
            var yard = new ConstructionYard(factory, _rootDir);
            _controller = yard.BuildRunMode();

            //SetupWebServer();

            SetupInterrupts();
            SetupTimers();
        }

        #region Setup methods

        /*
        private void SetupWebServer()
        {
            _webServer = new WebServer(_logger, _garbage);

            var authApiService = new AuthApiService(_configService, _garbage);
            var authResponder = new ApiServiceResponder(authApiService);
            _webServer.AddResponse(authResponder);

            var configApiService = new ConfigApiService(_configService, _garbage);
            var configResponder = new ApiServiceResponder(configApiService);
            _webServer.AddResponse(configResponder);

            var updateClient = new FilePutResponder(_rootDir, "client", _logger);
            _webServer.AddResponse(updateClient);

            var fileServe = new FileGetResponder(_rootDir, "client", _logger);
            _webServer.AddResponse(fileServe);
        }
         * */

        private void SetupInputs()
        {
            _keySwitchA = SetupInterruptOffAndOn(PinsCerbuino.A0);
            _keySwitchB = SetupInterruptOffAndOn(PinsCerbuino.A1);

            _buttonUp = SetupInterruptRelease(PinsCerbuino.D2);
            _buttonDown = SetupInterruptRelease(PinsCerbuino.A2);
            _buttonArm = SetupInterruptRelease(PinsCerbuino.D4);
            _buttonDeploy = SetupInterruptRelease(PinsCerbuino.D5);
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
                //var storage = new Persistence(Mainboard.SDCardStorageDevice);
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

        #region Blink

        private void BlinkTick(Gadgeteer.Timer timer)
        {
            _controller.Tick();
        }

        #endregion
    }
}