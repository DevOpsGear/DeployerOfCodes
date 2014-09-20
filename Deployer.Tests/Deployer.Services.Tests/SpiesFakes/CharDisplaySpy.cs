using Deployer.Services.Hardware;

namespace Deployer.Tests.SpiesFakes
{
	public class CharDisplaySpy : ICharDisplay
	{
		public string Line1 { get; private set; }
		public string Line2 { get; private set; }

		public CharDisplaySpy()
		{
			Line1 = string.Empty;
			Line2 = string.Empty;
		}

		public void Write(string line1, string line2 = "")
		{
			Line1 = line1 ?? string.Empty;
			Line2 = line2 ?? string.Empty;
		}
	}
}