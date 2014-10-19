using Deployer.Services.Input;
using Deployer.Services.RunModes;
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
        private readonly ModeRunner _runner;

        public DeployerConsole()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var rootDir = Path.Combine(appDataDir, "DeployerOfCodes");
            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);

            var factory = new TextDeployerFactory();
            _runner = new ModeRunner(factory, rootDir);
            _runner.Start();

            _timer = new Timer {Interval = 1000.0};
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
                            _runner.KeyOnEvent(KeySwitch.KeyA);
                        else
                            _runner.KeyOffEvent(KeySwitch.KeyA);
                        break;
                    case ConsoleKey.B:
                        if (isShiftDown)
                            _runner.KeyOnEvent(KeySwitch.KeyB);
                        else
                            _runner.KeyOffEvent(KeySwitch.KeyB);
                        break;

                    case ConsoleKey.UpArrow:
                        _runner.UpPressedEvent();
                        break;
                    case ConsoleKey.DownArrow:
                        _runner.DownPressedEvent();
                        break;

                    case ConsoleKey.Spacebar:
                        _runner.ArmPressedEvent();
                        break;
                    case ConsoleKey.Enter:
                        _runner.DeployPressedEvent();
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            _runner.Tick();
        }
    }
}