using Deployer.Services.Input;

namespace Deployer.Services.StateMachine
{
	public interface IDeployerController
	{
		void PreflightCheck();
		void KeyOnEvent(KeySwitch whichKey);
		void KeyOffEvent(KeySwitch whichKey);
		void UpPressedEvent();
		void DownPressedEvent();
		void ArmPressedEvent();
		void DeployPressedEvent();
		void Tick();
	}
}