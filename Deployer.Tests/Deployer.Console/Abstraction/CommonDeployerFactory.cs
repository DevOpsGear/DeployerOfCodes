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
    public class CommonDeployerFactory
    {
        public ITimeService CreateTimeService()
        {
            return new TimeService();
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

        public IWebUtility CreateWebUtility(IDeployerGarbage garbage)
        {
            return new WebUtility(garbage);
        }

        public INetwork CreateNetworkWrapper()
        {
            return new TextNetworkWrapper();
        }
    }
}