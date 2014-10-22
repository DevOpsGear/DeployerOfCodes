using System;
using Deployer.Services.StateMachine.States;

namespace Deployer.Services.StateMachine
{
    public interface IDeployerController : IInputEvents, IDisposable
    {
        void PreflightCheck();
        IDeployerState State { get; set; }
        bool Stop();
    }
}