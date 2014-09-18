using Deployer.Services.Models;

namespace Deployer.Services.Builders
{
	public interface IBuildService
	{
		BuildState StartBuild(string config);
		BuildState GetStatus();
		void CancelBuild();
	}
}