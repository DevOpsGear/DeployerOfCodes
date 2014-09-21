namespace Deployer.Services.StateMachine2.States
{
	public abstract class DeployerState2
	{
		protected readonly DeployerContext Context;

		protected DeployerState2(DeployerContext context)
		{
			Context = context;
		}

		public virtual void KeyTurned()
		{
		}

		public virtual void Up()
		{
		}

		public virtual void Down()
		{
		}

		public virtual void Arm()
		{
		}

		public virtual void Deploy()
		{
		}
	}
}