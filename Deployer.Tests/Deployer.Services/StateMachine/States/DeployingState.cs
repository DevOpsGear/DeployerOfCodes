﻿using Deployer.Services.Builders;
using Deployer.Services.Models;

namespace Deployer.Services.StateMachine.States
{
    public class DeployingState : DeployerStateBase
    {
        private IBuildService _currentBuild;

        public DeployingState(DeployerContext context)
            : base(context)
        {
        }

        public override void Check()
        {
            lock (this)
            {
                _currentBuild = null;
                var proj = Context.Project.SelectedProject;
                _currentBuild = BuildServiceFactory.Create(proj.Provider,
                                                           Context.WebFactory,
                                                           Context.WebUtility,
                                                           Context.Garbage);
                var config = Context.ConfigurationService.GetBuildParams(proj.Slug);
                var state = _currentBuild.StartBuild(config);
                ProcessBuildState(state, proj.Title);
            }
        }

        public override void Tick()
        {
            lock (this)
            {
                if (_currentBuild == null) return;
                var proj = Context.Project.SelectedProjectName;
                var state = _currentBuild.GetStatus();
                ProcessBuildState(state, proj);
            }
        }

        private void ProcessBuildState(BuildState state, string proj)
        {
            switch (state.Status)
            {
                case BuildStatus.Queued:
                    Context.CharDisplay.Write("*** Queued", proj);
                    Context.Indicator.LightRunning();
                    break;

                case BuildStatus.Running:
                    Context.CharDisplay.Write("*** Building", proj);
                    Context.Indicator.LightRunning();
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