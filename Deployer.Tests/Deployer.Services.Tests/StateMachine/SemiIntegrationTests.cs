﻿using Deployer.Services.Builders;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro.Web;
using Deployer.Services.Models;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Tests.SpiesFakes;
using Moq;
using NUnit.Framework;
using System;
using NeonMika.Interfaces;

namespace Deployer.Tests.StateMachine
{
	[TestFixture]
	public class SemiIntegrationTests
	{
		private CharDisplaySpy _display;
		private Mock<IIndicators> _indicators;
		private Mock<IConfigurationService> _config;
		private Mock<INetwork> _net;
		private Mock<ISound> _sound;
		private TimeServiceFake _time;
		private ProjectSelector _projSel;
		private SimultaneousKeys _simKeys;
		private Mock<IWebRequestFactory> _webFactory;
		private Mock<IGarbage> _garbage;
		private IWebUtility _netio;

		private DeployerContext _context;
		private DeployerController _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_display = new CharDisplaySpy();
			_indicators = new Mock<IIndicators>();
			_config = new Mock<IConfigurationService>();
			_net = new Mock<INetwork>();
			_sound = new Mock<ISound>();
			_time = new TimeServiceFake(new DateTime(2010, 01, 01));
			_projSel = new ProjectSelector(_display, _config.Object);
			_simKeys = new SimultaneousKeys(false, false, _time);
			_webFactory = new Mock<IWebRequestFactory>();
			_garbage = new Mock<IGarbage>();
			_netio = new WebUtility(_garbage.Object);

			ConstructSut();
		}

		[Test]
		public void Press_down_to_see_ip_address()
		{
			_net.Setup(x => x.IpAddress).Returns("999.888.777.666");
			_sut.DownPressedEvent();

			Assert.AreEqual("IP address:", _display.Line1, "Line 1");
			Assert.AreEqual("999.888.777.666", _display.Line2, "Line 2");

			AssertIndicators("K");
		}

		[Test]
		public void Init_reject_all_buttons_except_down()
		{
			_sut.UpPressedEvent();
			VerifyTurnBothKeysState();

			_sut.ArmPressedEvent();
			VerifyTurnBothKeysState();

			_sut.DeployPressedEvent();
			VerifyTurnBothKeysState();

			AssertIndicators("K");

			_sut.PreflightCheck();
			AssertIndicators("K");
		}

		[Test]
		public void Start_with_both_keys_on_and_turn_both_off()
		{
			_simKeys = new SimultaneousKeys(true, true, _time);
			ConstructSut();

			Assert.AreEqual("Both keys off", _display.Line1, "Line 1");
			Assert.AreEqual("to begin", _display.Line2, "Line 2");
			AssertIndicators("K");

			_sut.KeyOffEvent(KeySwitch.KeyA);
			Assert.AreEqual("Both keys off", _display.Line1, "Line 1");
			Assert.AreEqual("to begin", _display.Line2, "Line 2");
			AssertIndicators("K");

			_sut.KeyOffEvent(KeySwitch.KeyB);
			Assert.AreEqual("Turn both keys", _display.Line1, "Line 1");
			Assert.AreEqual("simultaneously", _display.Line2, "Line 2");
			AssertIndicators("K");
		}

		[Test]
		public void Start_with_keyB_on_and_turn_it_off()
		{
			_simKeys = new SimultaneousKeys(false, true, _time);
			ConstructSut();

			Assert.AreEqual("Both keys off", _display.Line1, "Line 1");
			Assert.AreEqual("to begin", _display.Line2, "Line 2");
			AssertIndicators("K");

			_sut.KeyOffEvent(KeySwitch.KeyB);
			Assert.AreEqual("Turn both keys", _display.Line1, "Line 1");
			Assert.AreEqual("simultaneously", _display.Line2, "Line 2");
			AssertIndicators("K");
		}

		[Test]
		public void Start_with_keyA_on_and_turn_it_off()
		{
			_simKeys = new SimultaneousKeys(true, false, _time);
			ConstructSut();

			Assert.AreEqual("Both keys off", _display.Line1, "Line 1");
			Assert.AreEqual("to begin", _display.Line2, "Line 2");
			AssertIndicators("K");

			_sut.KeyOffEvent(KeySwitch.KeyA);
			Assert.AreEqual("Turn both keys", _display.Line1, "Line 1");
			Assert.AreEqual("simultaneously", _display.Line2, "Line 2");
			AssertIndicators("K");
		}

		[Test]
		public void Turn_keys_on_too_slowly()
		{
			VerifyTurnBothKeysState();
			AssertIndicators("K");

			_time.AddSeconds(5);
			_sut.KeyOnEvent(KeySwitch.KeyA);
			_time.AddSeconds(5);
			_sut.KeyOnEvent(KeySwitch.KeyB);

			Assert.AreEqual("ABORTED", _display.Line1, "Line 1");
			Assert.AreEqual("Remove keys", _display.Line2, "Line 2");
			AssertIndicators("K");
		}

		[Test]
		public void Turn_keys_on_within_threshold()
		{
			VerifyTurnBothKeysState();
			AssertIndicators("K");
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");
		}

		[Test]
		public void Select_project2_then_back_to_project1()
		{
			MockConfigs();

			VerifyTurnBothKeysState();
			AssertIndicators("K");
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");

			_sut.DownPressedEvent();
			Assert.AreEqual("Title1", _display.Line1, "Line 1");
			Assert.AreEqual("Sub1", _display.Line2, "Line 2");

			_sut.DownPressedEvent();
			Assert.AreEqual("Title2", _display.Line1, "Line 1");
			Assert.AreEqual("Sub2", _display.Line2, "Line 2");

			_sut.UpPressedEvent();
			Assert.AreEqual("Title1", _display.Line1, "Line 1");
			Assert.AreEqual("Sub1", _display.Line2, "Line 2");
		}

		[Test]
		public void While_in_project_select_state_reject_arm_and_deploy()
		{
			MockConfigs();

			VerifyTurnBothKeysState();
			AssertIndicators("K");
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");

			_sut.ArmPressedEvent();
			VerifySelectProjectState();

			_sut.DeployPressedEvent();
			VerifySelectProjectState();

			_sut.DownPressedEvent();
			Assert.AreEqual("Title1", _display.Line1, "Line 1");
			Assert.AreEqual("Sub1", _display.Line2, "Line 2");
		}

		[Test]
		public void Project_select_state_then_turn_keyA_to_abort()
		{
			MockConfigs();

			VerifyTurnBothKeysState();
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");

			_sut.KeyOffEvent(KeySwitch.KeyA);

			Assert.AreEqual("ABORTED", _display.Line1, "Line 1");
			Assert.AreEqual("Remove keys", _display.Line2, "Line 2");
		}

		[Test]
		public void Project_select_state_then_turn_keyB_to_abort()
		{
			MockConfigs();

			VerifyTurnBothKeysState();
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");

			_sut.KeyOffEvent(KeySwitch.KeyB);

			Assert.AreEqual("ABORTED", _display.Line1, "Line 1");
			Assert.AreEqual("Remove keys", _display.Line2, "Line 2");
		}

		[Test]
		public void Abort_then_reset()
		{
			MockConfigs();

			VerifyTurnBothKeysState();
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");

			_sut.KeyOffEvent(KeySwitch.KeyA);

			Assert.AreEqual("ABORTED", _display.Line1, "Line 1");
			Assert.AreEqual("Remove keys", _display.Line2, "Line 2");

			_sut.KeyOnEvent(KeySwitch.KeyA);
			Assert.AreEqual("ABORTED", _display.Line1, "Line 1");
			Assert.AreEqual("Remove keys", _display.Line2, "Line 2");

			_sut.KeyOffEvent(KeySwitch.KeyA);
			Assert.AreEqual("ABORTED", _display.Line1, "Line 1");
			Assert.AreEqual("Remove keys", _display.Line2, "Line 2");

			_sut.KeyOffEvent(KeySwitch.KeyB);
			Assert.AreEqual("Turn both keys", _display.Line1, "Line 1");
			Assert.AreEqual("simultaneously", _display.Line2, "Line 2");
		}

		[Test]
		public void Complete_process_through_failed_depoyment()
		{
			MockConfigs();

			VerifyTurnBothKeysState();
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");

			_sut.DownPressedEvent();
			Assert.AreEqual("Title1", _display.Line1, "Line 1");
			Assert.AreEqual("Sub1", _display.Line2, "Line 2");
			AssertIndicators("KP");

			_sut.ArmPressedEvent();
			Assert.AreEqual("Ready to deploy", _display.Line1, "Line 1");
			Assert.AreEqual("Title1", _display.Line2, "Line 2");
			AssertIndicators("KPD");

			_sut.DeployPressedEvent();
			Assert.AreEqual("*** Queued", _display.Line1, "Line 1");
			Assert.AreEqual("Title1", _display.Line2, "Line 2");
			AssertIndicators("KPDR", false);

			for (int idx = 0; idx < 6; idx++)
				_sut.Tick();
			Assert.AreEqual("*** Queued", _display.Line1, "Line 1");
			Assert.AreEqual("Title1", _display.Line2, "Line 2");

			_sut.Tick();
			Assert.AreEqual("*** Building", _display.Line1, "Line 1");
			Assert.AreEqual("Title1", _display.Line2, "Line 2");

			for (int idx = 0; idx < 5; idx++)
				_sut.Tick();
			Assert.AreEqual("*** Building", _display.Line1, "Line 1");
			Assert.AreEqual("Title1", _display.Line2, "Line 2");

			_sut.Tick();
			Assert.AreEqual("* FAILURE *", _display.Line1, "Line 1");
			Assert.AreEqual("Title1", _display.Line2, "Line 2");
			AssertIndicators("KPDRF");
		}

		[Test]
		public void Complete_process_through_successful_depoyment()
		{
			MockConfigs();

			VerifyTurnBothKeysState();
			TurnKeysTogether();
			VerifySelectProjectState();
			AssertIndicators("KP");

			_sut.DownPressedEvent();
			Assert.AreEqual("Title1", _display.Line1, "Line 1");
			Assert.AreEqual("Sub1", _display.Line2, "Line 2");

			_sut.DownPressedEvent();
			Assert.AreEqual("Title2", _display.Line1, "Line 1");
			Assert.AreEqual("Sub2", _display.Line2, "Line 2");

			_sut.ArmPressedEvent();
			Assert.AreEqual("Ready to deploy", _display.Line1, "Line 1");
			Assert.AreEqual("Title2", _display.Line2, "Line 2");
			AssertIndicators("KPD");

			_sut.DeployPressedEvent();
			Assert.AreEqual("*** Queued", _display.Line1, "Line 1");
			Assert.AreEqual("Title2", _display.Line2, "Line 2");
			AssertIndicators("KPDR", false);

			for (int idx = 0; idx < 10; idx++)
				_sut.Tick();
			Assert.AreEqual("*** Queued", _display.Line1, "Line 1");
			Assert.AreEqual("Title2", _display.Line2, "Line 2");

			_sut.Tick();
			Assert.AreEqual("*** Building", _display.Line1, "Line 1");
			Assert.AreEqual("Title2", _display.Line2, "Line 2");

			for (int idx = 0; idx < 4; idx++)
				_sut.Tick();
			Assert.AreEqual("*** Building", _display.Line1, "Line 1");
			Assert.AreEqual("Title2", _display.Line2, "Line 2");

			_sut.Tick();
			Assert.AreEqual("SUCCESS!", _display.Line1, "Line 1");
			Assert.AreEqual("Title2", _display.Line2, "Line 2");
			AssertIndicators("KPDRS");
		}

		private void ConstructSut()
		{
			_context = new DeployerContext(_simKeys, _projSel, _display, _indicators.Object, _sound.Object, _netio, _net.Object,
			                               _webFactory.Object, _garbage.Object, _config.Object);
			_sut = new DeployerController(_context);
			_context.SetController(_sut);
			_sut.PreflightCheck();
		}

		private void MockConfigs()
		{
			_config.Setup(x => x.GetProjects()).Returns(new[]
				{
					new ProjectModel("fail-1", "Title1", "Sub1", 1, BuildServiceProvider.Failing),
					new ProjectModel("succ-2", "Title2", "Sub2", 2, BuildServiceProvider.Succeeding)
				});
		}

		private void VerifySelectProjectState()
		{
			Assert.AreEqual("Select project", _display.Line1, "Line 1");
			Assert.AreEqual("and press ARM", _display.Line2, "Line 2");
		}

		private void TurnKeysTogether()
		{
			_sut.Tick();
			_time.AddMilliseconds(1);
			_sut.KeyOnEvent(KeySwitch.KeyA);
			_sut.Tick();
			_time.AddMilliseconds(1);
			_sut.KeyOnEvent(KeySwitch.KeyB);
			_sut.Tick();
		}

		private void VerifyTurnBothKeysState()
		{
			_sut.Tick();
			_time.AddSeconds(5);

			//Assert.AreEqual(DeployerState.TurnBothKeys, _loop.State, "State");
			//_indicators.Verify(x => x.ChangedState(DeployerState.TurnBothKeys), Times.Once);
			Assert.AreEqual("Turn both keys", _display.Line1, "Line 1");
			Assert.AreEqual("simultaneously", _display.Line2, "Line 2");
		}

		private void AssertIndicators(string s, bool doTick = true)
		{
			if (doTick)
				_sut.Tick();

			if (s.Contains("K"))
				_indicators.Verify(x => x.BlinkKeys(), Times.AtLeastOnce, "Keys");
			else
				_indicators.Verify(x => x.BlinkKeys(), Times.Never, "Keys");

			if (s.Contains("P"))
				_indicators.Verify(x => x.BlinkProjectAndArm(), Times.AtLeastOnce, "Project select");
			else
				_indicators.Verify(x => x.BlinkProjectAndArm(), Times.Never, "Project select");

			if (s.Contains("D"))
				_indicators.Verify(x => x.BlinkReadyToDeploy(), Times.AtLeastOnce, "Ready to deploy");
			else
				_indicators.Verify(x => x.BlinkReadyToDeploy(), Times.Never, "Ready to deploy");

			if (s.Contains("R"))
				_indicators.Verify(x => x.LightRunning(), Times.AtLeastOnce, "Running");
			else
				_indicators.Verify(x => x.LightRunning(), Times.Never, "Running");

			if (s.Contains("S"))
				_indicators.Verify(x => x.LightSucceeded(), Times.AtLeastOnce, "Succeeded");
			else
				_indicators.Verify(x => x.LightSucceeded(), Times.Never, "Succeeded");

			if (s.Contains("F"))
				_indicators.Verify(x => x.LightFailed(), Times.AtLeastOnce, "Failed");
			else
				_indicators.Verify(x => x.LightFailed(), Times.Never, "Failed");
		}
	}
}