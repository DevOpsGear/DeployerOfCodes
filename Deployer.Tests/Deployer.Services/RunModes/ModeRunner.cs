using Deployer.Services.Abstraction;
using Deployer.Services.Input;
using Deployer.Services.StateMachine;
using Deployer.Services.StateMachine.States;
using NeonMika;

namespace Deployer.Services.RunModes
{
    public class ModeRunner : IInputEvents
    {
        private readonly IDeployerFactory _factory;
        private readonly string _rootDir;
        private IDeployerController _controller;
        private IWebServer _webServer;

        public ModeRunner(IDeployerFactory factory, string rootDir)
        {
            _factory = factory;
            _rootDir = rootDir;
        }

        public void Start()
        {
            var yard = new ConstructionYard(_factory, _rootDir);
            _controller = yard.BuildDeploymentMode();
        }

        public void KeyOnEvent(KeySwitch whichKey)
        {
            if (_controller == null) return;
            _controller.KeyOnEvent(whichKey);
        }

        public void KeyOffEvent(KeySwitch whichKey)
        {
            if (_controller == null) return;
            _controller.KeyOffEvent(whichKey);
        }

        public void UpPressedEvent()
        {
            if (InConfigurationMode)
                SwitchToDeploymentMode();
            if (_controller == null) return;
            _controller.UpPressedEvent();
        }

        public void DownPressedEvent()
        {
            if (InDeploymentIdleState)
                SwitchToConfigurationMode();
            if (_controller == null) return;
            _controller.DownPressedEvent();
        }

        public void ArmPressedEvent()
        {
            if (_controller == null) return;
            _controller.ArmPressedEvent();
        }

        public void DeployPressedEvent()
        {
            if (_controller == null) return;
            _controller.DeployPressedEvent();
        }

        public void Tick()
        {
            if (_controller == null) return;
            _controller.Tick();
        }

        private void StopEverything()
        {
            if (_controller != null)
            {
                _controller.Stop();
                _controller.Dispose();
                _controller = null;
            }
            if (_webServer != null)
            {
                _webServer.Stop();
                _webServer.Dispose();
                _webServer = null;
            }
            _factory.CreateGarbage().Collect();
        }

        private bool InDeploymentIdleState
        {
            get
            {
                if (_controller != null)
                {
                    if (_controller.State is TurnBothKeysState)
                        return true;
                }
                return false;
            }
        }

        private bool InConfigurationMode
        {
            get { return _webServer != null; }
        }

        private void SwitchToDeploymentMode()
        {
            StopEverything();
            var yard = new ConstructionYard(_factory, _rootDir);
            _controller = yard.BuildDeploymentMode();
        }

        private void SwitchToConfigurationMode()
        {
            StopEverything();
            var yard = new ConstructionYard(_factory, _rootDir);
            _webServer = yard.BuildConfigurationMode(_factory.WebServerPort);
        }
    }
}