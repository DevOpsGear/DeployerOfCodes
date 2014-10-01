namespace Deployer.Services.StateMachine.States
{
	public interface IDeployerState
	{
		void Check();
		void KeyTurned();
		void Up();
		void Down();
		void Arm();
		void Deploy();
		void Tick();
	}
}