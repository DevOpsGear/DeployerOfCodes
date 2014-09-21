
namespace Deployer.Services.StateMachine2.States
{
	public class AbortState : DeployerState2
	{
		public AbortState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			Context.CharDisplay.Write("ABORTED", "Remove keys");
		}

		public override void KeyTurned()
		{
			if (Context.Keys.AreBothOff)
			{
				Context.ChangeState(new InitState(Context));
			}
		}
	}
}