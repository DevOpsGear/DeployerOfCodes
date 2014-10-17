using Deployer.Services.Micro;
using Microsoft.SPOT;
using NeonMika.Interfaces;

namespace Deployer.App.Micro
{
    public class GarbageCollector : IGarbage, INeonGarbage
	{
        void IGarbage.Collect()
        {
            Debug.GC(true);
        }

        void INeonGarbage.Collect()
        {
            Debug.GC(true);
        }
    }
}