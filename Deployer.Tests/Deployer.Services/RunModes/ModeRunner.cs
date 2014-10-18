using Deployer.Services.Abstraction;
using Deployer.Services.Input;
using Deployer.Services.StateMachine;

namespace Deployer.Services.RunModes
{
    public class ModeRunner : IRunner
    {
        private readonly IDeployerFactory _factory;
        private readonly string _rootDir;
        private IDeployerController _controller;

        public ModeRunner(IDeployerFactory factory, string rootDir)
        {
            _factory = factory;
            _rootDir = rootDir;
        }

        public void Start()
        {
            var yard = new ConstructionYard(_factory, _rootDir);
            _controller = yard.BuildRunMode();
        }

        public void KeyOnEvent(KeySwitch whichKey)
        {
            _controller.KeyOnEvent(whichKey);
        }

        public void KeyOffEvent(KeySwitch whichKey)
        {
            _controller.KeyOffEvent(whichKey);
        }

        public void UpPressedEvent()
        {
            _controller.UpPressedEvent();
        }

        public void DownPressedEvent()
        {
            _controller.DownPressedEvent();
        }

        public void ArmPressedEvent()
        {
            _controller.ArmPressedEvent();
        }

        public void DeployPressedEvent()
        {
            _controller.DeployPressedEvent();
        }

        public void Tick()
        {
            _controller.Tick();
        }
    }
}