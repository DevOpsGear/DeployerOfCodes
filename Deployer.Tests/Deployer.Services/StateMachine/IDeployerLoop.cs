using Deployer.Services.Models;

namespace Deployer.Services.StateMachine
{
	public interface IDeployerLoop
	{
		DeployerState State { get; }
		void InitDone();
		void BothKeysTurned();
		void Up();
		void Down();
		void Arm();
		void Deploy();
		void Succeeded();
		void Failed();
		void Abort();
		void DisplayQueued();
		void DisplayBuilding(BuildState state);
		void ShowIpAddress();
	}
}