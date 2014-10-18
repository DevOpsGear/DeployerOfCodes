using Deployer.Services.Abstraction;
using Deployer.Services.Hardware;
using Deployer.Services.Micro;
using Deployer.Wpf.Hardware;
using Deployer.Wpf.Micro;
using System.Windows.Media;

namespace Deployer.Wpf.Abstraction
{
    public class WpfDeployerFactory : CommonFactory
    {
        private readonly MainWindow _mainWindow;

        public WpfDeployerFactory(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public override ILed CreateIndicatorKeyA()
        {
            return new Led(_mainWindow.IndicatorA, _mainWindow.Dispatcher);
        }

        public override ILed CreateIndicatorKeyB()
        {
            return new Led(_mainWindow.IndicatorB, _mainWindow.Dispatcher);
        }

        public override ILed CreateIndicatorSelect()
        {
            return new Led(_mainWindow.IndicatorSelect, _mainWindow.Dispatcher);
        }

        public override ILed CreateIndicatorArm()
        {
            return new Led(_mainWindow.IndicatorArm, _mainWindow.Dispatcher);
        }

        public override ILed CreateIndicatorFire()
        {
            return new Led(_mainWindow.IndicatorFire, _mainWindow.Dispatcher, Colors.Red);
        }

        public override ILed CreateIndicatorRunning()
        {
            return new Led(_mainWindow.IndicatorRunning, _mainWindow.Dispatcher, Colors.Yellow);
        }

        public override ILed CreateIndicatorSucceeded()
        {
            return new Led(_mainWindow.IndicatorSucceeded, _mainWindow.Dispatcher, Colors.SpringGreen);
        }

        public override ILed CreateIndicatorFailed()
        {
            return new Led(_mainWindow.IndicatorFailed, _mainWindow.Dispatcher, Colors.Red);
        }

        public override IDeployerGarbage CreateGarbage()
        {
            return new WpfGarbage();
        }

        public override IDeployerLogger CreateLogger()
        {
            return new WpfLogger();
        }

        public override ICharDisplay CreateCharacterDisplay()
        {
            return new WpfCharDisplay(_mainWindow.LcdLineOne, _mainWindow.LcdLineTwo, _mainWindow.Dispatcher);
        }

        public override ISound CreateSound()
        {
            return new WpfSound();
        }

        public override INetwork CreateNetworkWrapper()
        {
            return new WpfNetworkWrapper();
        }

    }
}