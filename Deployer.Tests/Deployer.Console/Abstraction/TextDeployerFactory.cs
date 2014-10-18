using Deployer.Services.Abstraction;
using Deployer.Services.Hardware;
using Deployer.Text.Hardware;
using Deployer.Text.Micro;
using NeonMika.Interfaces;

namespace Deployer.Text.Abstraction
{
    public class TextDeployerFactory : CommonFactory
    {
        public override ILed CreateIndicatorKeyA()
        {
            return new Led("A");
        }

        public override ILed CreateIndicatorKeyB()
        {
            return new Led("B");
        }

        public override ILed CreateIndicatorSelect()
        {
            return new Led("Select");
        }

        public override ILed CreateIndicatorArm()
        {
            return new Led("Arm");
        }

        public override ILed CreateIndicatorFire()
        {
            return new Led("Fire");
        }

        public override ILed CreateIndicatorRunning()
        {
            return new Led("Running");
        }

        public override ILed CreateIndicatorSucceeded()
        {
            return new Led("Succeeded");
        }

        public override ILed CreateIndicatorFailed()
        {
            return new Led("Failed");
        }

        public override IGarbage CreateGarbage()
        {
            return new Garbage();
        }

        public override ILogger CreateLogger()
        {
            return new Logger();
        }

        public override ICharDisplay CreateCharacterDisplay()
        {
            return new TextCharDisplay();
        }

        public override ISound CreateSound()
        {
            return new TextSound();
        }

        public override INetwork CreateNetworkWrapper()
        {
            return new TextNetworkWrapper();
        }
    }
}