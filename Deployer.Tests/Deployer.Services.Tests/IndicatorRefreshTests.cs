using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Tests.SpiesFakes;
using NUnit.Framework;

namespace Deployer.Tests
{
	[TestFixture]
	internal class IndicatorRefreshTests
	{
		private IndictatorSpy _ledA;
		private IndictatorSpy _ledB;
		private IndictatorSpy _ledProject;
		private IndictatorSpy _ledArm;
		private IndictatorSpy _ledFire;
		private IndictatorSpy _ledDeploying;
		private IndictatorSpy _ledSucceeded;
		private IndictatorSpy _ledFailed;
		private IndicatorRefresh _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_ledA = new IndictatorSpy();
			_ledB = new IndictatorSpy();
			_ledProject = new IndictatorSpy();
			_ledArm = new IndictatorSpy();
			_ledFire = new IndictatorSpy();
			_ledDeploying = new IndictatorSpy();
			_ledSucceeded = new IndictatorSpy();
			_ledFailed = new IndictatorSpy();
			_sut = new IndicatorRefresh(
				_ledA, _ledB, _ledProject,
				_ledArm, _ledFire,
				_ledDeploying, _ledSucceeded, _ledFailed);
		}

		[Test]
		public void Everything_off_immediately_after_state_change()
		{
			_sut.ChangedState(DeployerState.TurnBothKeys);
			AssertIndicators("");
		}

		[Test]
		public void TurnBothKeys_blinks_A_and_B_together()
		{
			_sut.ChangedState(DeployerState.TurnBothKeys);
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("AB");
			_sut.Tick();
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("AB");
			_sut.Tick();
			AssertIndicators("");
		}

		[Test]
		public void SelectProjectAndArm_alternates_blinking_project_and_arm()
		{
			_sut.ChangedState(DeployerState.SelectProjectAndArm);
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("P");
			_sut.Tick();
			AssertIndicators("R");
			_sut.Tick();
			AssertIndicators("P");
			_sut.Tick();
			AssertIndicators("R");
		}

		[Test]
		public void ReadyToDeploy_blinks_fire()
		{
			_sut.ChangedState(DeployerState.ReadyToDeploy);
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("F");
			_sut.Tick();
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("F");
			_sut.Tick();
			AssertIndicators("");
		}

		[Test]
		public void Deploying_lights_deploying()
		{
			_sut.ChangedState(DeployerState.Deploying);
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("D");
			_sut.Tick();
			AssertIndicators("D");
			_sut.Tick();
			AssertIndicators("D");
			_sut.Tick();
			AssertIndicators("D");
		}

		[Test]
		public void Succeeded_lights_succeeded()
		{
			_sut.ChangedState(DeployerState.Succeeded);
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("S");
			_sut.Tick();
			AssertIndicators("S");
			_sut.Tick();
			AssertIndicators("S");
			_sut.Tick();
			AssertIndicators("S");
		}

		[Test]
		public void Failed_lights_failed()
		{
			_sut.ChangedState(DeployerState.Failed);
			AssertIndicators("");
			_sut.Tick();
			AssertIndicators("X");
			_sut.Tick();
			AssertIndicators("X");
			_sut.Tick();
			AssertIndicators("X");
			_sut.Tick();
			AssertIndicators("X");
		}

		private void AssertIndicators(string visual)
		{
			AssertOne(_ledA, visual, "A", "KeyA");
			AssertOne(_ledB, visual, "B", "KeyB");
			AssertOne(_ledProject, visual, "P", "Project select");
			AssertOne(_ledArm, visual, "R", "Arm");
			AssertOne(_ledFire, visual, "F", "Fire");
			AssertOne(_ledDeploying, visual, "D", "Deploying");
			AssertOne(_ledSucceeded, visual, "S", "Succeded");
			AssertOne(_ledFailed, visual, "X", "Failed");
		}

		private static void AssertOne(IndictatorSpy led, string visual, string key, string name)
		{
			if (visual.Contains(key))
				Assert.IsTrue(led.Active, name + " should be lit");
			else
				Assert.IsFalse(led.Active, name + " should be dark");
		}
	}
}