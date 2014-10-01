namespace Deployer.Services.StateMachine.States
{
	public abstract class DeployerStateBase : IDeployerState
	{
		protected readonly DeployerContext Context;

		protected DeployerStateBase(DeployerContext context)
		{
			Context = context;
		}

		public abstract void Check();

		public virtual void KeyTurned()
		{
			Context.ChangeState(new AbortState(Context));
		}

		public virtual void Up()
		{
		}

		public virtual void Down()
		{
			var ipAddress = Context.Network.IpAddress;
			Context.CharDisplay.Write("IP address:", ipAddress);
		}

		public virtual void Arm()
		{
		}

		public virtual void Deploy()
		{
		}

		public virtual void Tick()
		{
		}
	}
}