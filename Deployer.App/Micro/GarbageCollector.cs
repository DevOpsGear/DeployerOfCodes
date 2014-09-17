using Deployer.Services.Micro;
using Microsoft.SPOT;

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