using Deployer.Services.Micro;
using System;
using NeonMika.Interfaces;

namespace Deployer.Wpf.Micro
{
    public class WpfGarbage : IGarbage
    {
        public void Collect()
        {
            GC.Collect();
        }
    }
}