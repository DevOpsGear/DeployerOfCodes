using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Tests.SpiesFakes;
using NUnit.Framework;

namespace Deployer.Tests.Output
{
	[TestFixture]
	internal class IndicatorTests
	{
		private IndictatorSpy _ledA;
		private IndictatorSpy _ledB;
		private IndictatorSpy _ledProject;
		private IndictatorSpy _ledArm;
		private IndictatorSpy _ledFire;
		private IndictatorSpy _ledDeploying;
		private IndictatorSpy _ledSucceeded;
		private IndictatorSpy _ledFailed;
		private Indicators _sut;

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
			_sut = new Indicators(
				_ledA, _ledB, _ledProject,
				_ledArm, _ledFire,
				_ledDeploying, _ledSucceeded, _ledFailed);
		}

		[Test]
		public void Everything_off_immediately_after_state_change()
		{
			_sut.ClearAll();
			AssertIndicators("");
		}

		[Test]
		public void TurnBothKeys_blinks_A_and_B_together()
		{
			_sut.BlinkKeys();
			AssertIndicators("AB");
			_sut.BlinkKeys();
			AssertIndicators("");
			_sut.BlinkKeys();
			AssertIndicators("AB");
			_sut.BlinkKeys();
			AssertIndicators("");
		}

		[Test]
		public void SelectProjectAndArm_alternates_blinking_project_and_arm()
		{
			_sut.BlinkProjectAndArm();
			AssertIndicators("P");
			_sut.BlinkProjectAndArm();
			AssertIndicators("R");
			_sut.BlinkProjectAndArm();
			AssertIndicators("P");
			_sut.BlinkProjectAndArm();
			AssertIndicators("R");
		}

		[Test]
		public void ReadyToDeploy_blinks_fire()
		{
			_sut.BlinkReadyToDeploy();
			AssertIndicators("F");
			_sut.BlinkReadyToDeploy();
			AssertIndicators("");
			_sut.BlinkReadyToDeploy();
			AssertIndicators("F");
			_sut.BlinkReadyToDeploy();
			AssertIndicators("");
		}

		[Test]
		public void Deploying_lights_deploying()
		{
			_sut.LightRunning();
			AssertIndicators("D");
		}

		[Test]
		public void Succeeded_lights_succeeded()
		{
			_sut.LightSucceeded();
			AssertIndicators("S");
		}

		[Test]
		public void Failed_lights_failed()
		{
			_sut.LightFailed();
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