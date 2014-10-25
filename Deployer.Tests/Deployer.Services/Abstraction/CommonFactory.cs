using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
using NeonMika.Interfaces;

namespace Deployer.Services.Abstraction
{
    public abstract class CommonFactory : IDeployerFactory
    {
        public ITimeService CreateTimeService()
        {
            return new TimeService();
        }

        public ISmallTextFileIo CreateSmallTextIo()
        {
            return new SmallTextFileIo();
        }

        public IJsonPersistence CreateJsonPersistence(ISmallTextFileIo io)
        {
            return new JsonPersistence(io);
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

        public IWebUtility CreateWebUtility(IGarbage garbage)
        {
            return new WebUtility(garbage);
        }

        public abstract void Initialize();
        public abstract INetwork CreateNetworkWrapper();
        public abstract int WebServerPort { get; }
        public abstract ILed CreateIndicatorKeyA();
        public abstract ILed CreateIndicatorKeyB();
        public abstract ILed CreateIndicatorSelect();
        public abstract ILed CreateIndicatorArm();
        public abstract ILed CreateIndicatorFire();
        public abstract ILed CreateIndicatorRunning();
        public abstract ILed CreateIndicatorSucceeded();
        public abstract ILed CreateIndicatorFailed();
        public abstract IGarbage CreateGarbage();
        public abstract ILogger CreateLogger();
        public abstract ICharDisplay CreateCharacterDisplay();
        public abstract ISound CreateSound();
    }
}