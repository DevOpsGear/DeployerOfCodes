using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
using Deployer.Services.StateMachine.States;

namespace Deployer.Services.StateMachine
{
	public class DeployerController : IDeployerController
	{
		private readonly DeployerContext _context;

		public DeployerController(DeployerContext context,
		                           IWebRequestFactory webFactory, IGarbage garbage, INetwork network)
		{
			_context = context;
			State = new InitState(_context);
		}

		public void PreflightCheck()
		{
			State.Check();
		}

		public void KeyOnEvent(KeySwitch whichKey)
		{
			_context.Keys.KeyOn(whichKey);
			State.KeyTurned();
		}

		public void KeyOffEvent(KeySwitch whichKey)
		{
			_context.Keys.KeyOff(whichKey);
			State.KeyTurned();
		}

		public void UpPressedEvent()
		{
			State.Up();
		}

		public void DownPressedEvent()
		{
			State.Down();
		}

		public void ArmPressedEvent()
		{
			State.Arm();
		}

		public void DeployPressedEvent()
		{
			State.Deploy();
		}

		public void Tick()
		{
			State.Tick();
		}

		public IDeployerState State { get; set; }
	}
}