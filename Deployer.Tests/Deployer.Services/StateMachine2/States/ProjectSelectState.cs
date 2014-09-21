namespace Deployer.Services.StateMachine2.States
{
	public class ProjectSelectState : DeployerStateBase
	{
		public ProjectSelectState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			Context.CharDisplay.Write("Select project", "and press ARM");
		}

		public override void Up()
		{
			Context.Project.Up();
		}

		public override void Down()
		{
			Context.Project.Down();
		}

		public override void Arm()
		{
			if (Context.Project.IsProjectSelected)
			{
				Context.ChangeState(new ReadyToDeployState(Context));
			}
		}

		public override void Tick()
		{
			Context.Indicator.BlinkProjectAndArm();
		}
	}
}