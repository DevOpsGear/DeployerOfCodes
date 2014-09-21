namespace Deployer.Services.StateMachine2.States
{
	public class TurnBothKeysState : DeployerStateBase
	{
		public TurnBothKeysState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			Context.CharDisplay.Write("Turn both keys", "simultaneously");
		}

		public override void KeyTurned()
		{
			if (Context.Keys.AreBothOn)
			{
				if (Context.Keys.SwitchedSimultaneously)
				{
					Context.ChangeState(new ProjectSelectState(Context));
				}
				else
				{
					Context.ChangeState(new AbortState(Context));
				}
			}
		}

		public override void Tick()
		{
			Context.Indicator.BlinkKeys();
		}
	}
}