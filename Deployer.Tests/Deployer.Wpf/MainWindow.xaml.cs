using Deployer.Services.Abstraction;
using Deployer.Services.Input;
using Deployer.Services.StateMachine;
using Deployer.Wpf.Abstraction;
using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Deployer.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly IDeployerController _controller;
        private readonly Timer _timer;

        public MainWindow()
        {
            InitializeComponent();

            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var rootDir = Path.Combine(appDataDir, "DeployerOfCodesWpf");
            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);

            var factory = new WpfDeployerFactory(this);
            var yard = new ConstructionYard(factory, rootDir);
            _controller = yard.BuildRunMode();

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
        } */
    }
}