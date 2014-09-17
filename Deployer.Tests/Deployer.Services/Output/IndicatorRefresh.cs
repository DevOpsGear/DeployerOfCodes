using Deployer.Services.Hardware;
using Deployer.Services.StateMachine;

namespace Deployer.Services.Output
{
	public class IndicatorRefresh : IIndicatorRefresh
	{
		private readonly ILed _keyA;
		private readonly ILed _keyB;
		private readonly ILed _selectProject;
		private readonly ILed _arm;
		private readonly ILed _deploy;
		private readonly ILed _deploying;
		private readonly ILed _succeeded;
		private readonly ILed _failed;
		private DeployerState _state;
		private bool _blink;

		public IndicatorRefresh(ILed keyA,
		                        ILed keyB,
		                        ILed selectProject,
		                        ILed arm,
		                        ILed deploy,
		                        ILed deploying,
		                        ILed succeeded,
		                        ILed failed)
		{
			_keyA = keyA;
			_keyB = keyB;
			_selectProject = selectProject;
			_arm = arm;
			_deploy = deploy;
			_deploying = deploying;
			_succeeded = succeeded;
			_failed = failed;
		}

		public void ChangedState(DeployerState state)
		{
			_keyA.Write(false);
			_keyB.Write(false);
			_selectProject.Write(false);
			_arm.Write(false);
			_deploy.Write(false);
			_deploying.Write(false);
			_succeeded.Write(false);
			_failed.Write(false);

			_state = state;
		}

		public void Tick()
		{
			_blink = !_blink;

			switch (_state)
			{
				case DeployerState.TurnBothKeys:
					_keyA.Write(_blink);
					_keyB.Write(_blink);
					break;

				case DeployerState.SelectProjectAndArm:
					_selectProject.Write(_blink);
					_arm.Write(!_blink);
					break;

				case DeployerState.ReadyToDeploy:
					_deploy.Write(_blink);
					break;

				case DeployerState.Deploying:
					_deploying.Write(true);
					break;

				case DeployerState.Succeeded:
					_succeeded.Write(true);
					break;

				case DeployerState.Failed:
					_failed.Write(true);
					break;
			}
		}
	}
}