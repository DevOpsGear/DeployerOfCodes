using System;
using System.IO;
using System.Timers;
using System.Windows.Threading;
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
using Deployer.Wpf.Hardware;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Deployer.Wpf.Micro;
using NeonMika;
using NeonMika.Interfaces;

namespace Deployer.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDeployerController _controller;
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

        public MainWindow()
        {
            InitializeComponent();

            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _rootDir = Path.Combine(appDataDir, "DeployerOfCodesWpf");
            if (!Directory.Exists(_rootDir))
                Directory.CreateDirectory(_rootDir);


            _garbage = new Garbage();
            _logger = new Logger();
            var smallIo = new SmallTextFileIo();
            var jsonPersist = new JsonPersistence(smallIo);
            var slugCreator = new SlugCreator();
            _configService = new RealConfigurationService(_rootDir, jsonPersist, slugCreator);
            var charDisp = new CharDisplay(LcdLineOne, LcdLineTwo, Dispatcher);
            var keys = new SimultaneousKeys(false, false, new TimeService());
            var webFactory = new WebRequestFactory();
            var project = new ProjectSelector(charDisp, _configService);
            var sound = new Sound();
            var webu = new WebUtility(_garbage);
            _network = new NetworkWrapper();

            _indictatorA = new Led(IndicatorA, Dispatcher);
            _indictatorB = new Led(IndicatorB, Dispatcher);
            _indictatorSelect = new Led(IndicatorSelect, Dispatcher);
            _indictatorArm = new Led(IndicatorArm, Dispatcher);
            _indictatorFire = new Led(IndicatorFire, Dispatcher, Colors.Red);
            _indictatorRunning = new Led(IndicatorRunning, Dispatcher, Colors.Yellow);
            _indictatorSucceeded = new Led(IndicatorSucceeded, Dispatcher, Colors.SpringGreen);
            _indictatorFailed = new Led(IndicatorFailed, Dispatcher, Colors.Red);

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

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _controller.Tick();
        }

        private void SwitchA_Checked(object sender, RoutedEventArgs e)
        {
            if (!SwitchA.IsChecked.HasValue) return;
            if (SwitchA.IsChecked.Value)
                _controller.KeyOnEvent(KeySwitch.KeyA);
            else
                _controller.KeyOffEvent(KeySwitch.KeyA);
        }

        private void SwitchB_Checked(object sender, RoutedEventArgs e)
        {
            if (!SwitchB.IsChecked.HasValue) return;
            if (SwitchB.IsChecked.Value)
                _controller.KeyOnEvent(KeySwitch.KeyB);
            else
                _controller.KeyOffEvent(KeySwitch.KeyB);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            switch (button.Name)
            {
                case "TurnBothKeysOn":
                    SwitchA.IsChecked = true;
                    SwitchB.IsChecked = true;
                    break;

                case "UpButton":
                    _controller.UpPressedEvent();
                    break;

                case "DownButton":
                    _controller.DownPressedEvent();
                    break;

                case "ArmButton":
                    _controller.ArmPressedEvent();
                    break;

                case "FireButton":
                    _controller.DeployPressedEvent();
                    break;
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

            var updateClient = new FilePutResponder(_rootDir, "client", _logger);
            _webServer.AddResponse(updateClient);

            var fileServe = new FileGetResponder(_rootDir, "client", _logger);
            _webServer.AddResponse(fileServe);
        }
    }
}