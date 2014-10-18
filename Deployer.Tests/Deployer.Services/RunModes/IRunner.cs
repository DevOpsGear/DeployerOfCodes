using Deployer.Services.StateMachine;

namespace Deployer.Services.RunModes
{
    public interface IRunner : IInputEvents
    {
        void Start();
    }
}