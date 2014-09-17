using System;
using Deployer.Services.Hardware;
using GHI.Networking;
using Microsoft.SPOT;

namespace Deployer.App.Hardware
{
	public class NetworkWrapper : INetwork
	{
		private readonly EthernetENC28J60 _ether;

		public NetworkWrapper(EthernetENC28J60 ether)
		{
			_ether = ether;
		}

		public bool IsNetworkUp
		{
			get { return _ether.IPAddress != "0.0.0.0"; }
		}

		public string IpAddress
		{
			get { return _ether.IPAddress; }
		}
	}
}