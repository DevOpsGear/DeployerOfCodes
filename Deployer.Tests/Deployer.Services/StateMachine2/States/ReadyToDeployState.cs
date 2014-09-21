namespace Deployer.Services.StateMachine2.States
{
	public class ReadyToDeployState : DeployerStateBase
	{
		public ReadyToDeployState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			var projectName = Context.Project.SelectedProjectName;
			Context.CharDisplay.Write("Ready to deploy", projectName);
			//Context.Sound.SoundAlarm();
		}

		public override void Deploy()
		{
			Context.ChangeState(new DeployingState(Context));
		}

		public override void Tick()
		{
			Context.Indicator.BlinkReadyToDeploy();
		}
	}
}