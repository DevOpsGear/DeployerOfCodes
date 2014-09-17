namespace Deployer.Services.Models
{
	public class BuildState
	{
		public BuildStatus Status { get; private set; }
		public int ElapsedTimeMs { get; private set; }
		public int EstimatedDurationMs { get; private set; }

		public BuildState(BuildStatus status, int elapsedTime = 0, int estimatedTime = 0)
		{
			Status = status;
			ElapsedTimeMs = elapsedTime;
			EstimatedDurationMs = estimatedTime;
		}
	}
}