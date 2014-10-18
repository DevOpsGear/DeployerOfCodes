using Deployer.Services.Abstraction;
using Deployer.Services.Input;
using Deployer.Services.StateMachine;
using Deployer.Text.Abstraction;
using System;
using System.IO;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Deployer.Text
{
    internal class Program
    {
        private static void Main()
        {
            var deployer = new DeployerConsole();
            deployer.Loop();
        }
    }

    public class DeployerConsole
    {
        private readonly Timer _timer;
        private readonly IDeployerController _controller;

        public DeployerConsole()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var rootDir = Path.Combine(appDataDir, "DeployerOfCodesWpf");
            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);
            var factory = new TextDeployerFactory();
            var yard = new ConstructionYard(factory, rootDir);
            _controller = yard.BuildRunMode();

            _timer = new Timer {Interval = 500.0};
            _timer.Elapsed += Tick;
            _timer.Start();
        }

        public void Loop()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (!Console.KeyAvailable) continue;

                var key = Console.ReadKey(true);
                var isShiftDown = key.Modifiers.HasFlag(ConsoleModifiers.Shift);
                switch (key.Key)
                {
                    case ConsoleKey.A:
                        if (isShiftDown)
                            _controller.KeyOnEvent(KeySwitch.KeyA);
                        else
                            _controller.KeyOffEvent(KeySwitch.KeyA);
                        break;
                    case ConsoleKey.B:
                        if (isShiftDown)
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

        private void Tick(object sender, ElapsedEventArgs e)
        {
            _controller.Tick();
        }

        /* private void SetupWebServer()
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
        } */
    }
}