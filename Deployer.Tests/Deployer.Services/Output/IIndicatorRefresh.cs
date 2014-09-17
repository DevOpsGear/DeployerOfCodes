using Deployer.Services.StateMachine;

namespace Deployer.Services.Output
{
	public interface IIndicatorRefresh
	{
		void ChangedState(DeployerState state);
		void Tick();
	}
}