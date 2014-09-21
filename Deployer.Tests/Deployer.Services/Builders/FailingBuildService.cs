using Deployer.Services.Models;

namespace Deployer.Services.Builders
{
	public class FailingBuildService : IBuildService
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
			if (_index > 12)
				return new BuildState(BuildStatus.Failed);
			if (_index > 6)
				return new BuildState(BuildStatus.Running);
			return new BuildState(BuildStatus.Queued);
		}

		public void CancelBuild()
		{
		}
	}
}