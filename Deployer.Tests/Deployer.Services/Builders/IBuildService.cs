using System.Collections;
using Deployer.Services.Models;

namespace Deployer.Services.Builders
{
	public interface IBuildService
	{
		BuildState StartBuild(Hashtable config);
		BuildState GetStatus();
		void CancelBuild();
	}
}