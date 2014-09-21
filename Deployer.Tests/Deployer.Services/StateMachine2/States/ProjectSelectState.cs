using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Services.StateMachine2.States
{
	public class ProjectSelectState : DeployerState2
	{
		public ProjectSelectState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			Context.CharDisplay.Write("Select project", "and press ARM");
		}
	}
}