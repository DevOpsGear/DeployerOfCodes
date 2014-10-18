﻿using Deployer.Services.Micro;
using System;
using NeonMika.Interfaces;

namespace Deployer.Wpf.Micro
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