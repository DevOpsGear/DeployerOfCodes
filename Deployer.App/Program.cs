using Deployer.App.Hardware;
using Deployer.App.Micro;
using Deployer.App.Webs;
using Deployer.Services.Config;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
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
		private Persistence _storage;

		private void ProgramStarted()
		{
			SetupEthernet();
			SetupInputs();
			SetupIndicators();
			SetupPersistence();

			var config = new FakeConfigurationService();
			var charDisp = new CharDisplay(characterDisplay);
			var keys = new SimultaneousKeys(ReversedSwitchA, ReversedSwitchB, new TimeService());
			var webFactory = new WebRequestFactory();
			var garbage = new GarbageCollector();
			var project = new ProjectSelector(charDisp, config);
			var sound = new Sound(tunes);
			var webu = new WebUtility(garbage);

			var indicators = new Indicators(_indicatorTurnKeyA,
			                                _indicatorTurnKeyB,
			                                _indicatorSelectProject,
			                                _indicatorReadyToArm,
			                                _indicatorReadyToDeploy,
			                                _indicatorStateDeploying,
			                                _indicatorStateSucceeded,
			                                _indicatorStateFailed);
			var context = new DeployerContext(keys, project, charDisp, indicators, sound, webu, _network, webFactory, garbage);
			_controller = new DeployerController(context);
			context.SetController(_controller);

			_controller.PreflightCheck();

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

				_webServer = new WebServer();
				var response = new SampleResponder();
				_webServer.AddResponse(response);
			}
			catch (Exception ex)
			{
				Debug.Print(ex.ToString());
				throw;
			}
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

		private void SetupInputs()
		{
			_keySwitchA = SetupInterruptOffAndOn(PinsCerbuino.A0);
			_keySwitchB = SetupInterruptOffAndOn(PinsCerbuino.A1);

			_buttonUp = SetupInterruptRelease(PinsCerbuino.D2);
			_buttonDown = SetupInterruptRelease(PinsCerbuino.A2);
			_buttonArm = SetupInterruptRelease(PinsCerbuino.D4);
			_buttonDeploy = SetupInterruptRelease(PinsCerbuino.D5);
		}

		private void SetupTimers()
		{
			_timerBlink = new Gadgeteer.Timer(500);
			_timerBlink.Tick += BlinkTick;
			_timerBlink.Start();
		}

		private void SetupPersistence()
		{
			_storage = new Persistence(Mainboard.SDCardStorageDevice);

			/* TODO: Experimentation
			Need to throw error condition if no card is present?
			var isStorage = Mainboard.IsSDCardInserted;
			var existDir = _storage.DoesRootDirectoryExist("config");
			if (existDir)
				_storage.DeleteDirectory("config");
			_storage.CreateDirectory("config");
			existDir = _storage.DoesRootDirectoryExist("config");

			var existFile = _storage.DoesFileExist(@"\config\projects.json");
			*/
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

		#region Web server

		/*
		private void OnWebServerCommandReceived(object obj, WebServer.WebServerEventArgs args)
		{
			Debug.Print("Raw URL = " + args.rawURL);
			_controller.ReceivedGetWebRequest(args.rawURL, args.response);
		}
		*/

		#endregion

		#region Blink

		private void BlinkTick(Gadgeteer.Timer timer)
		{
			_controller.Tick();
		}

		#endregion
	}
}