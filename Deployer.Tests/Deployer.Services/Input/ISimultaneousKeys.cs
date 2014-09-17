namespace Deployer.Services.Input
{
	public interface ISimultaneousKeys
	{
		void KeyOn(KeySwitch whichKey);
		void KeyOff(KeySwitch whichKey);
		bool SwitchedSimultaneously { get; }
		bool AreBothOn { get; }
		bool AreBothOff { get; }
	}
}