﻿using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;

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
        IDeployerGarbage CreateGarbage();
        IDeployerLogger CreateLogger();
        ISmallTextFileIo CreateSmallTextIo();
        IJsonPersistence CreateJsonPersistence(ISmallTextFileIo smallIo);
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
        IWebUtility CreateWebUtility(IDeployerGarbage garbage);
        INetwork CreateNetworkWrapper();
    }
}