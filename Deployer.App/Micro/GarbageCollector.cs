using Microsoft.SPOT;
using NeonMika.Interfaces;

namespace Deployer.App.Micro
{
    public class GarbageCollector : IGarbage
    {
        public void Collect()
        {
            Debug.GC(true);
        }
    }
}