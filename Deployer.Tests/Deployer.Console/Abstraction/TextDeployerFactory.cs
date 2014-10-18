using Deployer.Services.Abstraction;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
using Deployer.Text.Hardware;
using Deployer.Text.Micro;

namespace Deployer.Text.Abstraction
{
    public class TextDeployerFactory : IDeployerFactory
    {
        public ILed CreateIndicatorKeyA()
        {
            return new Led("A");
        }

        public ILed CreateIndicatorKeyB()
        {
            return new Led("B");
        }

        public ILed CreateIndicatorSelect()
        {
            return new Led("Select");
        }

        public ILed CreateIndicatorArm()
        {
            return new Led("Arm");
        }

        public ILed CreateIndicatorFire()
        {
            return new Led("Fire");
        }

        public ILed CreateIndicatorRunning()
        {
            return new Led("Running");
        }

        public ILed CreateIndicatorSucceeded()
        {
            return new Led("Succeeded");
        }

        public ILed CreateIndicatorFailed()
        {
            return new Led("Failed");
        }

        public IDeployerGarbage CreateGarbage()
        {
            return new Garbage();
        }

        public IDeployerLogger CreateLogger()
        {
            return new Logger();
        }

        public ISmallTextFileIo CreateSmallTextIo()
        {
            return new SmallTextFileIo();
        }

        public IJsonPersistence CreateJsonPersistence(ISmallTextFileIo smallIo)
        {
            return new JsonPersistence(smallIo);
        }

        public ISlugCreator CreateSlugCreator()
        {
            return new SlugCreator();
        }

        public IConfigurationService CreateConfigurationService(string rootDir, IJsonPersistence jsonPersist,
                                                                ISlugCreator slugCreator)
        {
            return new RealConfigurationService(rootDir, jsonPersist, slugCreator);
        }

        public ICharDisplay CreateCharacterDisplay()
        {
            return new CharDisplay();
        }

        public ITimeService CreateTimeService()
        {
            return new TimeService();
        }

        public ISimultaneousKeys CreateSimultaneousKeys(ITimeService timeService)
        {
            return new SimultaneousKeys(false, false, timeService);
        }

        public IWebRequestFactory CreateWebRequestFactory()
        {
            return new WebRequestFactory();
        }

        public IProjectSelector CreateProjectSelector(ICharDisplay charDisplay,
                                                      IConfigurationService configurationService)
        {
            return new ProjectSelector(charDisplay, configurationService);
        }

        public ISound CreateSound()
        {
            return new Sound();
        }

        public IWebUtility CreateWebUtility(IDeployerGarbage garbage)
        {
            return new WebUtility(garbage);
        }

        public INetwork CreateNetworkWrapper()
        {
            return new NetworkWrapper();
        }
    }
}