using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Services.StateMachine2.States;

namespace Deployer.Services.StateMachine2
{
	public class DeployerContext
	{
		private readonly ISimultaneousKeys _keys;
		private readonly IProjectSelector _projectSelect;
		private readonly ICharDisplay _lcd;
		private readonly IIndicatorRefresh2 _indicatorRefresh;
		private readonly ISound _sound;
		private readonly INetwork _network;
		private IDeployerController _controller;

		public DeployerContext(ISimultaneousKeys keys, IProjectSelector projectSelect,
		                       ICharDisplay lcd,
		                       IIndicatorRefresh2 indicatorRefresh, ISound sound, INetwork network)
		{
			_keys = keys;
			_projectSelect = projectSelect;
			_lcd = lcd;
			_indicatorRefresh = indicatorRefresh;
			_sound = sound;
			_network = network;
		}

		public void SetController(IDeployerController controller)
		{
			_controller = controller;
		}

		public ICharDisplay CharDisplay
		{
			get { return _lcd; }
		}

		public ISimultaneousKeys Keys
		{
			get { return _keys; }
		}

		public IProjectSelector Project
		{
			get { return _projectSelect; }
		}

		public INetwork Network
		{
			get { return _network; }
		}

		public void ChangeState(DeployerState2 newState)
		{
			_controller.State = newState;
			_controller.State.Check();
		}
	}
}