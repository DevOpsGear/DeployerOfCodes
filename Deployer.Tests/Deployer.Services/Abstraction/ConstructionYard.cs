using Deployer.Services.Api;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Services.WebResponders;
using NeonMika;
using NeonMika.Interfaces;
using NeonMika.Requests;

namespace Deployer.Services.Abstraction
{
    public class ConstructionYard
    {
        private readonly IDeployerFactory _factory;

        private readonly string _rootDir;
        private readonly IGarbage _garbage;
        private readonly ILogger _logger;
        private readonly IPersistence _persist;
        private readonly IConfigurationService _configService;

        public ConstructionYard(IDeployerFactory factory, string rootDir)
        {
            _factory = factory;
            _rootDir = rootDir;
            _garbage = _factory.CreateGarbage();
            _logger = _factory.CreateLogger();
            _persist = _factory.CreatePersistence();

            var smallIo = _factory.CreateSmallTextIo(_persist);
            var jsonPersist = new JsonPersistence(smallIo);
            var slugCreator = new SlugCreator();
            _configService = new RealConfigurationService(_rootDir, jsonPersist, slugCreator);
        }

        public IDeployerController BuildDeploymentMode()
        {
            var charDisp = _factory.CreateCharacterDisplay();
            var timeService = _factory.CreateTimeService();
            var keys = _factory.CreateSimultaneousKeys(timeService);
            var webFactory = _factory.CreateWebRequestFactory();
            var project = _factory.CreateProjectSelector(charDisp, _configService);
            var sound = _factory.CreateSound();
            var webu = _factory.CreateWebUtility(_garbage);
            var network = _factory.CreateNetworkWrapper();

            var indicators = new Indicators(_factory.CreateIndicatorKeyA(),
                                            _factory.CreateIndicatorKeyB(),
                                            _factory.CreateIndicatorSelect(),
                                            _factory.CreateIndicatorArm(),
                                            _factory.CreateIndicatorFire(),
                                            _factory.CreateIndicatorRunning(),
                                            _factory.CreateIndicatorSucceeded(),
                                            _factory.CreateIndicatorFailed());

            var context = new DeployerContext(keys,
                                              project,
                                              charDisp,
                                              indicators,
                                              sound,
                                              webu,
                                              network,
                                              webFactory,
                                              _garbage,
                                              _configService);

            var controller = new DeployerController(context);
            context.SetController(controller);
            controller.PreflightCheck();

            return controller;
        }

        /*  public IWebServer BuildConfigurationNotAvailable(int port)
        {
            RequestHelper.SetLogger(_logger);
            return new WebServerNotAvailable(_logger, port);
        } */

        public IWebServer BuildConfigurationMode(int port)
        {
            var charDisp = _factory.CreateCharacterDisplay();
            var network = _factory.CreateNetworkWrapper();
            charDisp.Write("Config mode", network.IpAddress + ":" + port);

            RequestHelper.SetLogger(_logger);
            var webServer = new WebServer(_logger, _garbage, port);

            var authApiService = new AuthApiService(_configService, _garbage);
            var authResponder = new ApiServiceResponder(authApiService);
            webServer.AddResponse(authResponder);

            var configApiService = new ConfigApiService(_configService, _garbage);
            var configResponder = new ApiServiceResponder(configApiService);
            webServer.AddResponse(configResponder);

            var updateClient = new FilePutResponder(_rootDir, "client", _logger);
            webServer.AddResponse(updateClient);

            var fileServe = new FileGetResponder(_rootDir, "client", _logger);
            webServer.AddResponse(fileServe);

            return webServer;
        }
    }
}