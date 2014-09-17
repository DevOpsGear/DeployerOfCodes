namespace Deployer.Services.StateMachine
{
	public enum DeployerState
	{
		Init,
		TurnBothKeys,
		SelectProjectAndArm,
		ReadyToDeploy,
		Deploying,
		Succeeded,
		Failed,
		Abort
	}
}