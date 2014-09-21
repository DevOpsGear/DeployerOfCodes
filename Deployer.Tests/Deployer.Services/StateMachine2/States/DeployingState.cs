using Deployer.Services.Builders;
using Deployer.Services.Models;

namespace Deployer.Services.StateMachine2.States
{
	public class DeployingState : DeployerState2
	{
		private IBuildService _currentBuild;

		public DeployingState(DeployerContext context)
			: base(context)
		{
		}

		public override void Check()
		{
			_currentBuild = null;
			var proj = Context.Project.SelectedProject;
			_currentBuild = BuildServiceFactory.Create(proj.BuildServiceProvider, Context.WebFactory, Context.Garbage);
			var state = _currentBuild.StartBuild(proj.CiConfig);
			ProcessBuildState(state, proj.Title);
			Context.Indicator.LightRunning();
		}

		public override void Tick()
		{
			var proj = Context.Project.SelectedProjectName;
			var state = _currentBuild.GetStatus();
			ProcessBuildState(state, proj);
		}

		private void ProcessBuildState(BuildState state, string proj)
		{
			switch (state.Status)
			{
				case BuildStatus.Queued:
					Context.CharDisplay.Write("*** Queued", proj);
					break;

				case BuildStatus.Running:
					Context.CharDisplay.Write("*** Building", proj);
					break;

				case BuildStatus.Succeeded:
					Context.ChangeState(new SuccessState(Context));
					break;

				case BuildStatus.Failed:
					Context.ChangeState(new FailureState(Context));
					_currentBuild = null;
					break;
			}
		}
	}
}