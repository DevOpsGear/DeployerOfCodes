using Deployer.Services.StateMachine.States;

namespace Deployer.Services.StateMachine
{
    public interface IDeployerController : IInputEvents
	{
		void PreflightCheck();
		IDeployerState State { get; set; }
	}
}