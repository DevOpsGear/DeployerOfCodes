namespace Deployer.Services.Hardware
{
	public interface ICharDisplay
	{
		void Write(string line1, string line2 = "");
	}
}