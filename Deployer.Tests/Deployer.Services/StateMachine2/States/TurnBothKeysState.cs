using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Services.StateMachine2.States
{
	public class TurnBothKeysState : DeployerState2
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
			if (Context.Keys.AreBothOn && Context.Keys.SwitchedSimultaneously)
			{
				// Transition to state
			}
			else
			{
				//Context.ChangeState(new AbortState(Context));
			}
		}
	}
}