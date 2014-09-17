namespace Deployer.Services.Hardware
{
	public interface INetwork
	{
		bool IsNetworkUp { get; }
		string IpAddress { get; }
	}
}