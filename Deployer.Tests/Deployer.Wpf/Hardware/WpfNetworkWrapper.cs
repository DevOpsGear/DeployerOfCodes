using Deployer.Services.Hardware;

namespace Deployer.Wpf.Hardware
{
    public class WpfNetworkWrapper : INetwork
    {
        public bool IsNetworkUp
        {
            get { return true; }
        }

        public string IpAddress
        {
            get { return "127.0.0.1"; }
        }
    }
}