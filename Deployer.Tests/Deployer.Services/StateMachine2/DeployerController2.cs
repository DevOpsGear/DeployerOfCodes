using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Services.StateMachine2.States;

namespace Deployer.Services.StateMachine2
{
	public class DeployerController2 : IDeployerController
	{
		private readonly DeployerContext _context;

		public DeployerController2(DeployerContext context,
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

		public DeployerState2 State { get; set; }
	}
}