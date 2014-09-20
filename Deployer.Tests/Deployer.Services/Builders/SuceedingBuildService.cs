using System.Diagnostics.CodeAnalysis;
using Deployer.Services.Models;

namespace Deployer.Services.Builders
{
	public class SuceedingBuildService : IBuildService
	{
		private int _index;

		public BuildState StartBuild(string config)
		{
			_index = 0;
			return new BuildState(BuildStatus.Queued);
		}

		public BuildState GetStatus()
		{
			_index++;
			if (_index > 15)
				return new BuildState(BuildStatus.Succeeded);
			if (_index > 10)
				return new BuildState(BuildStatus.Running);
			return new BuildState(BuildStatus.Queued);
		}

		public void CancelBuild()
		{
		}
	}
}