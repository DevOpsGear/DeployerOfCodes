
namespace Deployer.Services.StateMachine2.States
{
	public class FailureState : DeployerState2
	{
		public FailureState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			var title = Context.Project.SelectedProjectName;
			Context.CharDisplay.Write("* FAILURE *", title);
			Context.Indicator.LightFailed();
		}
	}
}