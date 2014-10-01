namespace Deployer.Services.StateMachine.States
{
	public class InitState : DeployerStateBase
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

		public override void Tick()
		{
			Context.Indicator.BlinkKeys();
		}
	}
}