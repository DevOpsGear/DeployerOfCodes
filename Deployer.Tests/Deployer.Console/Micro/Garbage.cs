using System;
using NeonMika.Interfaces;

namespace Deployer.Text.Micro
{
    public class Garbage : IGarbage
    {
        public void Collect()
        {
            GC.Collect();
        }
    }
}