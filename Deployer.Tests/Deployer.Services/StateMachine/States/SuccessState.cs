
namespace Deployer.Services.StateMachine.States
{
	public class SuccessState : DeployerStateBase
	{
		public SuccessState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			var title = Context.Project.SelectedProjectName;
			Context.CharDisplay.Write("SUCCESS!", title);
			Context.Indicator.LightSucceeded();
		}
	}
}