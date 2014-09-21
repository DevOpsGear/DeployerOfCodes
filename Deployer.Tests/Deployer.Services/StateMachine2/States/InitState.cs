namespace Deployer.Services.StateMachine2.States
{
	public class InitState : DeployerState2
	{
		public InitState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			if (Context.Keys.AreBothOff)
				Context.ChangeState(new TurnBothKeysState(Context));
			else
				Context.CharDisplay.Write("Both keys off", "to begin");
		}

		public override void KeyTurned()
		{
			if (Context.Keys.AreBothOff)
				Context.ChangeState(new TurnBothKeysState(Context));
		}
	}
}