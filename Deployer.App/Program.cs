using System;
using System.Threading;
using Deployer.App.Hardware;
using Deployer.App.Micro;
using Deployer.Services.Config;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Gadgeteer;
using Gadgeteer.Networking;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using Timer = Gadgeteer.Timer;

namespace Deployer.App
{
	public partial class Program
	{
		private IDeployerLoop _loop;
		private IndicatorRefresh _indicator;
		private IProjectSelector _project;
		private DeployerController _controller;

		private Timer _timerBlink;

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
		private Persistence _storage;
		private Sound _sound;

		private void ProgramStarted()
		{
			characterDisplay.Clear();
			characterDisplay.SetCursorPosition(0, 0);
			characterDisplay.Print("Getting IP address...");

			SetupEthernet();
			SetupInputs();
			SetupIndicators();
			SetupPersistence();
			SetupSound();

			var config = new FakeConfigurationService();
			var charDisp = new CharDisplay(characterDisplay);
			var keys = new SimultaneousKeys(ReversedSwitchA, ReversedSwitchB);
			var webFactory = new WebRequestFactory();
			var garbage = new GarbageCollector();

			_indicator = new IndicatorRefresh(_indicatorTurnKeyA,
			                                  _indicatorTurnKeyB,
			                                  _indicatorSelectProject,
			                                  _indicatorReadyToArm,
			                                  _indicatorReadyToDeploy,
			                                  _indicatorStateDeploying,
			                                  _indicatorStateSucceeded,
			                                  _indicatorStateFailed);
			_project = new ProjectSelector(charDisp, config);
			_loop = new DeployerLoop(charDisp, _indicator, _project, _network, _sound);
			_controller = new DeployerController(_loop, _project, keys, webFactory, garbage);

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

				WebServer.DefaultEvent.WebEventReceived += OnWebEvent;
				WebServer.StartLocalServer(eth.IPAddress, 80);
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
			_timerBlink = new Timer(500);
			_timerBlink.Tick += BlinkTick;
			_timerBlink.Start();
		}

		private void SetupPersistence()
		{
			// TODO: ERROR CONDITION IF NO CARD IS INSERTED
			var isStorage = Mainboard.IsSDCardInserted;
			_storage = new Persistence(Mainboard.SDCardStorageDevice);
			return;

			// TODO: Experimentation
			var existDir = _storage.DoesRootDirectoryExist("config");
			if (existDir)
				_storage.DeleteDirectory("config");
			_storage.CreateDirectory("config");
			existDir = _storage.DoesRootDirectoryExist("config");

			var existFile = _storage.DoesFileExist(@"\config\projects.json");
		}

		private void SetupSound()
		{
			_sound = new Sound(tunes);
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
			var interr = new InterruptPort(pin, true,
			                               Port.ResistorMode.PullUp,
			                               Port.InterruptMode.InterruptEdgeBoth);
			return interr;
		}

		private InterruptPort SetupInterruptRelease(Cpu.Pin pin)
		{
			var interr = new InterruptPort(pin, true,
			                               Port.ResistorMode.PullUp,
			                               Port.InterruptMode.InterruptEdgeLow);
			return interr;
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

		private void OnWebEvent(string path, WebServer.HttpMethod method, Responder responder)
		{
			Debug.Print("Web! " + path);
			responder.Respond("Hello");
		}

		private void OnWebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
		{
			Debug.Print("SPECIFIC EVENT!!!");
			responder.Respond("SPECIFIC");
		}

		#endregion

		#region Blink

		private void BlinkTick(Timer timer)
		{
			_indicator.Tick();
			_controller.Tick();
		}

		#endregion
	}
}