using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Hardware;

namespace Deployer.Tests.SpiesFakes
{
	internal class IndictatorSpy : ILed
	{
		public bool Active { get; private set; }

		public void Write(bool state)
		{
			Active = state;
		}
	}
}