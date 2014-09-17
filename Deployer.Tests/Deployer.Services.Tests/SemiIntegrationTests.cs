using Deployer.Services.Hardware;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.Output;
using Deployer.Services.StateMachine;
using Deployer.Tests.Spies;
using Deployer.Tests.SpiesFakes;
using Moq;
using NUnit.Framework;
using System;

namespace Deployer.Tests
{
	[TestFixture]
	public class SemiIntegrationTests
	{
		private CharDisplaySpy _display;
		private Mock<IIndicatorRefresh> _indicators;
		private Mock<INetwork> _net;
		private Mock<ISound> _sound;
		private TimeServiceFake _time;
		private Mock<IProjectSelector> _projSel;
		private SimultaneousKeys _simKeys;
		private Mock<IWebRequestFactory> _webFactory;
		private Mock<IGarbage> _garbage;

		private DeployerLoop _loop;
		private DeployerController _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_display = new CharDisplaySpy();
			_indicators = new Mock<IIndicatorRefresh>();
			_net = new Mock<INetwork>();
			_sound = new Mock<ISound>();
			_time = new TimeServiceFake(new DateTime(2010, 01, 01));
			_projSel = new Mock<IProjectSelector>();
			_simKeys = new SimultaneousKeys(false, false, _time);
			_webFactory = new Mock<IWebRequestFactory>();
			_garbage = new Mock<IGarbage>();

			_loop = new DeployerLoop(_display, _indicators.Object, _projSel.Object, _net.Object, _sound.Object);
			_sut = new DeployerController(_loop, _projSel.Object, _simKeys, _webFactory.Object, _garbage.Object);
		}

		[Test]
		public void Turn_Keys_On_Too_Slowly()
		{
			_sut.PreflightCheck();
			_sut.Tick();
			_time.AddSeconds(5);

			Assert.AreEqual(DeployerState.TurnBothKeys, _loop.State, "State");
			_indicators.Verify(x => x.ChangedState(DeployerState.TurnBothKeys), Times.Once);
			Assert.AreEqual("Turn both keys", _display.Line1, "Line 1");
			Assert.AreEqual("simultaneously", _display.Line2, "Line 2");

			_sut.Tick();
			_time.AddSeconds(5);
			_sut.KeyOnEvent(KeySwitch.KeyA);
			_sut.Tick();
			_time.AddSeconds(5);
			_sut.KeyOnEvent(KeySwitch.KeyB);
			_sut.Tick();

			Assert.AreEqual("ABORTED", _display.Line1, "Line 1");
			Assert.AreEqual("Remove keys", _display.Line2, "Line 2");
		}

		[Test]
		public void Turn_Keys_On_Just_Right()
		{
			_sut.PreflightCheck();
			_sut.Tick();
			_time.AddMilliseconds(1);

			Assert.AreEqual(DeployerState.TurnBothKeys, _loop.State, "State");
			_indicators.Verify(x => x.ChangedState(DeployerState.TurnBothKeys), Times.Once);
			Assert.AreEqual("Turn both keys", _display.Line1, "Line 1");
			Assert.AreEqual("simultaneously", _display.Line2, "Line 2");

			_sut.Tick();
			_time.AddMilliseconds(1);
			_sut.KeyOnEvent(KeySwitch.KeyA);
			_sut.Tick();
			_time.AddMilliseconds(1);
			_sut.KeyOnEvent(KeySwitch.KeyB);
			_sut.Tick();

			Assert.AreEqual("Select project", _display.Line1, "Line 1");
			Assert.AreEqual("and press ARM", _display.Line2, "Line 2");
		}
	}
}