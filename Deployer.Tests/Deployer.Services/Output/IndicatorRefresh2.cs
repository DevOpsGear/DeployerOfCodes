﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Hardware;
using Deployer.Services.StateMachine;

namespace Deployer.Services.Output
{
	public interface IIndicatorRefresh2
	{
		void ClearAll();
		void BlinkKeys();
		void BlinkProjectAndArm();
		void BlinkReadyToDeploy();
		void LightRunning();
		void LightSucceeded();
		void LightFailed();
	}

	public class IndicatorRefresh2 : IIndicatorRefresh2
	{
		private readonly ILed _keyA;
		private readonly ILed _keyB;
		private readonly ILed _selectProject;
		private readonly ILed _arm;
		private readonly ILed _deploy;
		private readonly ILed _deploying;
		private readonly ILed _succeeded;
		private readonly ILed _failed;
		private bool _blink;

		public IndicatorRefresh2(ILed keyA,
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

		public void ClearAll()
		{
			_keyA.Write(false);
			_keyB.Write(false);
			_selectProject.Write(false);
			_arm.Write(false);
			_deploy.Write(false);
			_deploying.Write(false);
			_succeeded.Write(false);
			_failed.Write(false);
		}

		public void BlinkKeys()
		{
			_blink = !_blink;
			_keyA.Write(_blink);
			_keyB.Write(_blink);
		}

		public void BlinkProjectAndArm()
		{
			_blink = !_blink;
			_selectProject.Write(_blink);
			_arm.Write(!_blink);
		}

		public void BlinkReadyToDeploy()
		{
			_blink = !_blink;
			_deploy.Write(_blink);
		}

		public void LightRunning()
		{
			_deploying.Write(true);
		}

		public void LightSucceeded()
		{
			_succeeded.Write(true);
		}

		public void LightFailed()
		{
			_failed.Write(true);
		}
	}
}