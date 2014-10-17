using System;
using System.IO;
using System.Threading;
using System.Timers;
using Deployer.App.WebResponders;
using Deployer.Services.Api;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro.Web;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Text.Hardware;
using Deployer.Text.Micro;
using NeonMika;
using Timer = System.Timers.Timer;

namespace Deployer.Text
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var deployer = new DeployerConsole();
			deployer.Loop();
		}
	}

	public class DeployerConsole
	{
		private readonly IDeployerController _controller;
		private readonly Led _indictatorA;
		private readonly Led _indictatorB;
		private readonly Led _indictatorSelect;
		private readonly Led _indictatorArm;
		private readonly Led _indictatorFire;
		private readonly Led _indictatorRunning;
		private readonly Led _indictatorSucceeded;
		private readonly Led _indictatorFailed;

		private readonly INetwork _network;
		//private WebServer _webServer;
		private IPersistence _storage;
		private readonly string _rootDir;
		private readonly Garbage _garbage;
		private readonly Logger _logger;
		private readonly IConfigurationService _configService;
		private readonly Timer _timer;
		private WebServer _webServer;

		public DeployerConsole()
		{
			var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			_rootDir = Path.Combine(appDataDir, "DeployerOfCodesWpf");
			if(!Directory.Exists(_rootDir))
				Directory.CreateDirectory(_rootDir);

			_garbage = new Garbage();
			_logger = new Logger();
			var smallIo = new SmallTextFileIo();
			var jsonPersist = new JsonPersistence(smallIo);
			var slugCreator = new SlugCreator();
			_configService = new RealConfigurationService(_rootDir, jsonPersist, slugCreator);
			var charDisp = new CharDisplay();
			var keys = new SimultaneousKeys(false, false, new TimeService());
			var webFactory = new WebRequestFactory();
			var project = new ProjectSelector(charDisp, _configService);
			var sound = new Sound();
			var webu = new WebUtility(_garbage);
			_network = new NetworkWrapper();

			_indictatorA = new Led("A");
			_indictatorB = new Led("B");
			_indictatorSelect = new Led("Select");
			_indictatorArm = new Led("Arm");
			_indictatorFire = new Led("Fire");
			_indictatorRunning = new Led("Running");
			_indictatorSucceeded = new Led("Succeeded");
			_indictatorFailed = new Led("Failed");

			var indicators = new Indicators(_indictatorA,
			                                _indictatorB,
			                                _indictatorSelect,
			                                _indictatorArm,
			                                _indictatorFire,
			                                _indictatorRunning,
			                                _indictatorSucceeded,
			                                _indictatorFailed);

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

			SetupWebServer();

			_timer = new Timer {Interval = 500.0};
			_timer.Elapsed += _timer_Elapsed;
			_timer.Start();
		}

		public void Loop()
		{
			while(true)
			{
				Thread.Sleep(100);
				if(!Console.KeyAvailable) continue;

				var key = Console.ReadKey(true);
				var isShiftDown = key.Modifiers.HasFlag(ConsoleModifiers.Shift);
				switch(key.Key)
				{
					case ConsoleKey.A:
						if(isShiftDown)
							_controller.KeyOnEvent(KeySwitch.KeyA);
						else
							_controller.KeyOffEvent(KeySwitch.KeyA);
						break;
					case ConsoleKey.B:
						if(isShiftDown)
							_controller.KeyOnEvent(KeySwitch.KeyB);
						else
							_controller.KeyOffEvent(KeySwitch.KeyB);
						break;

					case ConsoleKey.UpArrow:
						_controller.UpPressedEvent();
						break;
					case ConsoleKey.DownArrow:
						_controller.DownPressedEvent();
						break;

					case ConsoleKey.Spacebar:
						_controller.ArmPressedEvent();
						break;
					case ConsoleKey.Enter:
						_controller.DeployPressedEvent();
						break;

					case ConsoleKey.Escape:
						return;
				}
			}
		}

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			_controller.Tick();
		}

		private void SetupWebServer()
		{
			_webServer = new WebServer(_logger, _garbage, 8091);

			var authApiService = new AuthApiService(_configService, _garbage);
			var authResponder = new ApiServiceResponder(authApiService);
			_webServer.AddResponse(authResponder);

			var configApiService = new ConfigApiService(_configService, _garbage);
			var configResponder = new ApiServiceResponder(configApiService);
			_webServer.AddResponse(configResponder);

			var updateClient = new FilePutResponder(_rootDir, _logger);
			_webServer.AddResponse(updateClient);

			var fileServe = new FileGetResponder(_rootDir, "client", _logger);
			_webServer.AddResponse(fileServe);
		}
	}
}