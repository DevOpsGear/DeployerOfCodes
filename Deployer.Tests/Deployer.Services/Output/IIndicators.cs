namespace Deployer.Services.Output
{
	public interface IIndicators
	{
		void ClearAll();
		void BlinkKeys();
		void BlinkProjectAndArm();
		void BlinkReadyToDeploy();
		void LightRunning();
		void LightSucceeded();
		void LightFailed();
	}
}