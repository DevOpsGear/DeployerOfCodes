﻿using Deployer.Services.Hardware;

namespace Deployer.Text.Hardware
{
    public class TextNetworkWrapper : INetwork
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