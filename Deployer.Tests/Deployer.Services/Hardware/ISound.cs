namespace Deployer.Services.Hardware
{
	public interface ISound
	{
		void SoundAlarm();
		void SoundSuccess();
		void SoundFailure();
	}
}