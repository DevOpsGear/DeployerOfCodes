using Deployer.Services.Micro;
using System;

namespace Deployer.Wpf.Micro
{
    public class Garbage : IGarbage
    {
        public void Collect()
        {
            GC.Collect();
        }
    }
}