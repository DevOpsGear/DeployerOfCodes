using Deployer.Services.Hardware;

namespace Deployer.Tests.Spies
{
	public class CharDisplaySpy : ICharDisplay
	{
		public string Line1 { get; private set; }
		public string Line2 { get; private set; }

		public void Write(string line1, string line2 = "")
		{
			Line1 = line1;
			Line2 = line2;
		}
	}
}