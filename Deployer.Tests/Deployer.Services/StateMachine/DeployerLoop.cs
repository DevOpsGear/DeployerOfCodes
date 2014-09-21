using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Models;
using Deployer.Services.Output;

namespace Deployer.Services.StateMachine
{
	public class DeployerLoop : IDeployerLoop
	{
		private readonly ICharDisplay _lcd;
		private readonly IIndicatorRefresh _indicatorRefresh;
		private readonly IProjectSelector _project;
		private readonly INetwork _network;
		private readonly ISound _sound;
		public DeployerState State { get; private set; }

		public DeployerLoop(ICharDisplay lcd, IIndicatorRefresh indicatorRefresh, IProjectSelector project, INetwork network,
		                    ISound sound)
		{
			_lcd = lcd;
			_indicatorRefresh = indicatorRefresh;
			_project = project;
			_network = network;
			_sound = sound;
			State = DeployerState.Init;
			_lcd.Write("Both keys off", "to begin");
		}

		public void InitDone()
		{
			State = DeployerState.TurnBothKeys;
			_indicatorRefresh.ChangedState(State);

			_lcd.Write("Turn both keys", "simultaneously");
		}

		public void BothKeysTurned()
		{
			State = DeployerState.SelectProjectAndArm;
			_indicatorRefresh.ChangedState(State);

			_lcd.Write("Select project", "and press ARM");
		}

		public void Up()
		{
			_project.Up();
		}

		public void Down()
		{
			_project.Down();
		}

		public void Arm()
		{
			if (!_project.IsProjectSelected)
			{
				return;
			}
			var projectName = _project.SelectedProjectName;

			State = DeployerState.ReadyToDeploy;
			_indicatorRefresh.ChangedState(State);

			_lcd.Write("Ready to deploy", projectName);
			_sound.SoundAlarm();
		}

		public void Deploy()
		{
			State = DeployerState.Deploying;
			_indicatorRefresh.ChangedState(State);
			var projectName = _project.SelectedProjectName;

			_lcd.Write("*** Deploying", projectName);
		}

		public void Succeeded()
		{
			State = DeployerState.Succeeded;
			_indicatorRefresh.ChangedState(State);
			var projectName = _project.SelectedProjectName;

			_lcd.Write("SUCCESS!", projectName);
			_sound.SoundSuccess();
		}

		public void Failed()
		{
			State = DeployerState.Failed;
			_indicatorRefresh.ChangedState(State);
			var projectName = _project.SelectedProjectName;

			_lcd.Write("* FAILURE *", projectName);
			_sound.SoundFailure();
		}

		public void Abort()
		{
			State = DeployerState.Abort;
			_indicatorRefresh.ChangedState(State);

			_lcd.Write("ABORTED", "Remove keys");
		}

		public void DisplayQueued()
		{
			var projectName = _project.SelectedProjectName;
			_lcd.Write("*** Queued", projectName);
		}

		public void DisplayBuilding(BuildState state)
		{
			var projectName = _project.SelectedProjectName;
			_lcd.Write("*** Building", projectName);
		}

		public void ShowIpAddress()
		{
			_lcd.Write("IP address:", _network.IpAddress);
		}
	}
}