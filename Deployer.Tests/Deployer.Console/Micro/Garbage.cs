using Deployer.Services.Micro;
using System;
using NeonMika.Interfaces;

namespace Deployer.Text.Micro
{
    public class Garbage : IDeployerGarbage, INeonGarbage
    {
        void IDeployerGarbage.Collect()
        {
            GC.Collect();
        }

        void INeonGarbage.Collect()
        {
            GC.Collect();
        }
    }
}