
namespace Deployer.Services.StateMachine2.States
{
	public class SuccessState : DeployerState2
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