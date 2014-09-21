namespace Deployer.Services.StateMachine2.States
{
	public class InitState : DeployerState2
	{
		public InitState(DeployerContext context)
			: base(context)
		{
			Context.CharDisplay.Write("Both keys off", "to begin");
		}

		public override void KeyTurned()
		{
			if (Context.Keys.AreBothOff)
			{
				Context.ChangeState(new TurnBothKeysState(Context));
			}
		}

		public override void Down()
		{
			var ipAddress = Context.Network.IpAddress;
			Context.CharDisplay.Write("IP address:", ipAddress);
		}
	}
}