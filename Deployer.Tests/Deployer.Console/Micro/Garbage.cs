using Deployer.Services.Micro;
using System;
using NeonMika.Interfaces;

namespace Deployer.Text.Micro
{
    public class Garbage : IGarbage, INeonGarbage
    {
        void IGarbage.Collect()
        {
            GC.Collect();
        }

        void INeonGarbage.Collect()
        {
            GC.Collect();
        }
    }
}