
namespace Deployer.Services.StateMachine.States
{
	public class FailureState : DeployerStateBase
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