using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro.Web;
using NeonMika.Interfaces;

namespace Deployer.Services.Abstraction
{
    public interface IDeployerFactory
    {
        ILed CreateIndicatorKeyA();
        ILed CreateIndicatorKeyB();
        ILed CreateIndicatorSelect();
        ILed CreateIndicatorArm();
        ILed CreateIndicatorFire();
        ILed CreateIndicatorRunning();
        ILed CreateIndicatorSucceeded();
        ILed CreateIndicatorFailed();
        IGarbage CreateGarbage();
        ILogger CreateLogger();
        IPersistence CreatePersistence();
        ISmallTextFileIo CreateSmallTextIo(IPersistence persistence);
        IJsonPersistence CreateJsonPersistence(ISmallTextFileIo io);
        ISlugCreator CreateSlugCreator();

        IConfigurationService CreateConfigurationService(
            string rootDir,
            IJsonPersistence jsonPersist,
            ISlugCreator slugCreator);

        ICharDisplay CreateCharacterDisplay();
        ITimeService CreateTimeService();
        ISimultaneousKeys CreateSimultaneousKeys(ITimeService timeService);
        IWebRequestFactory CreateWebRequestFactory();

        IProjectSelector CreateProjectSelector(
            ICharDisplay charDisplay,
            IConfigurationService configurationService);

        ISound CreateSound();
        IWebUtility CreateWebUtility(IGarbage garbage);
        INetwork CreateNetworkWrapper();
        int WebServerPort { get; }
    }
}