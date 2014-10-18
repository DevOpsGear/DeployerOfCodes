using Deployer.Services.Input;

namespace Deployer.Services.StateMachine
{
    public interface IInputEvents
    {
        void KeyOnEvent(KeySwitch whichKey);
        void KeyOffEvent(KeySwitch whichKey);
        void UpPressedEvent();
        void DownPressedEvent();
        void ArmPressedEvent();
        void DeployPressedEvent();
        void Tick();
    }
}