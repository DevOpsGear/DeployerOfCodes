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
			Context.CharDisplay.Write("Turn both keys", "simultaneously");
		}
	}
}