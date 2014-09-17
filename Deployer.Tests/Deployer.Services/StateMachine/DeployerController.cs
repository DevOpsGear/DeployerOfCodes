using Deployer.Services.Build;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Models;

namespace Deployer.Services.StateMachine
{
	public class DeployerController
	{
		private readonly IDeployerLoop _loop;
		private readonly IProjectSelector _projectSelect;
		private readonly ISimultaneousKeys _keys;
		private readonly IWebRequestFactory _webFactory;
		private readonly IGarbage _garbage;
		private bool _networkState;
		private IBuildService _currentBuild;

		public DeployerController(IDeployerLoop loop, IProjectSelector projectSelect, ISimultaneousKeys keys,
		                          IWebRequestFactory webFactory, IGarbage garbage)
		{
			_loop = loop;
			_projectSelect = projectSelect;
			_keys = keys;
			_webFactory = webFactory;
			_garbage = garbage;
			_networkState = false;
		}

		public void PreflightCheck()
		{
			if (_keys.AreBothOff)
			{
				_loop.InitDone();
			}
		}

		public bool NetworkState
		{
			get { return _networkState; }
			set
			{
				if (_networkState == value) return;

				_networkState = value;
				if (_networkState)
					NetworkUpEvent();
				else
					NetworkDownEvent();
			}
		}

		private void NetworkUpEvent()
		{
		}

		private void NetworkDownEvent()
		{
		}

		public void KeyOnEvent(KeySwitch whichKey)
		{
			_keys.KeyOn(whichKey);
			if (_loop.State == DeployerState.TurnBothKeys)
			{
				if (_keys.AreBothOn)
				{
					if (_keys.SwitchedSimultaneously)
						_loop.BothKeysTurned();
					else
						_loop.Abort();
				}
			}
		}

		public void KeyOffEvent(KeySwitch whichKey)
		{
			_keys.KeyOff(whichKey);
			if (_loop.State == DeployerState.Init)
			{
				if (_keys.AreBothOff)
					_loop.InitDone();
				return;
			}
			if (_loop.State == DeployerState.TurnBothKeys)
				return;
			if (_loop.State == DeployerState.Abort)
			{
				_loop.InitDone();
				return;
			}

			_loop.Abort();
			if (_keys.AreBothOff)
				_loop.InitDone();
		}

		public void UpPressedEvent()
		{
			if (_loop.State == DeployerState.SelectProjectAndArm)
				_loop.Up();
		}

		public void DownPressedEvent()
		{
			if (_loop.State == DeployerState.SelectProjectAndArm)
				_loop.Down();
			else
				_loop.ShowIpAddress();
		}

		public void ArmPressedEvent()
		{
			if (_loop.State == DeployerState.SelectProjectAndArm)
				_loop.Arm();
		}

		public void DeployPressedEvent()
		{
			if (_loop.State != DeployerState.ReadyToDeploy) return;
			_loop.Deploy();

			_currentBuild = null;
			var proj = _projectSelect.SelectedProject;
			var build = BuildServiceFactory.Create(proj.BuildServiceProvider, _webFactory, _garbage);
			var state = build.StartBuild(proj.CiConfig);
			ProcessBuildState(state);

			_currentBuild = build;
		}

		public void Tick()
		{
			if (_loop.State == DeployerState.Deploying)
				DuringDeployment();
		}

		private void DuringDeployment()
		{
			if (_currentBuild == null) return;
			var state = _currentBuild.GetStatus();
			ProcessBuildState(state);
		}

		private void ProcessBuildState(BuildState state)
		{
			switch (state.Status)
			{
				case BuildStatus.Queued:
					_loop.DisplayQueued();
					break;

				case BuildStatus.Running:
					_loop.DisplayBuilding(state);
					break;

				case BuildStatus.Succeeded:
					_loop.Succeeded();
					_currentBuild = null;
					break;

				case BuildStatus.Failed:
					_loop.Failed();
					_currentBuild = null;
					break;

				case BuildStatus.Aborted:
					_loop.Failed();
					_currentBuild = null;
					break;
			}
		}
	}
}