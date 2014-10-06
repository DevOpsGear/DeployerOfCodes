using Deployer.Services.Config;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Services.StateMachine.States;

namespace Deployer.Services.StateMachine
{
	public class DeployerContext
	{
		private readonly ISimultaneousKeys _keys;
		private readonly IProjectSelector _projectSelect;
		private readonly ICharDisplay _lcd;
		private readonly IIndicators _indicatorRefresh;
		private readonly ISound _sound;
		private readonly IWebUtility _netio;
		private readonly INetwork _network;
		private readonly IWebRequestFactory _webFactory;
		private readonly IGarbage _garbage;
		private readonly IConfigurationService _configurationService;
		private IDeployerController _controller;

		public DeployerContext(ISimultaneousKeys keys, IProjectSelector projectSelect,
		                       ICharDisplay lcd,
		                       IIndicators indicatorRefresh, ISound sound, IWebUtility netio, INetwork network,
		                       IWebRequestFactory webFactory, IGarbage garbage, IConfigurationService configurationService)
		{
			_keys = keys;
			_projectSelect = projectSelect;
			_lcd = lcd;
			_indicatorRefresh = indicatorRefresh;
			_sound = sound;
			_netio = netio;
			_network = network;
			_webFactory = webFactory;
			_garbage = garbage;
			_configurationService = configurationService;
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

		public IIndicators Indicator
		{
			get { return _indicatorRefresh; }
		}

		public INetwork Network
		{
			get { return _network; }
		}

		public IWebUtility WebUtility
		{
			get { return _netio; }
		}

		public IWebRequestFactory WebFactory
		{
			get { return _webFactory; }
		}

		public IGarbage Garbage
		{
			get { return _garbage; }
		}

		public IConfigurationService ConfigurationService
		{
			get { return _configurationService; }
		}

		public void ChangeState(IDeployerState newState)
		{
			_indicatorRefresh.ClearAll();
			_controller.State = newState;
			_controller.State.Check();
		}
	}
}