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