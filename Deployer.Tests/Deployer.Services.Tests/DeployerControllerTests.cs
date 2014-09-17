using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Input;
using Deployer.Services.Micro;
using Deployer.Services.StateMachine;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests
{
	[TestFixture]
	public class DeployerControllerTests
	{
		private DeployerController _sut;
		private Mock<IDeployerLoop> _loop;
		private Mock<IProjectSelector> _projSel;
		private Mock<ISimultaneousKeys> _simKeys;
		private Mock<IWebRequestFactory> _webFactory;
		private Mock<IGarbage> _garbage;

		[SetUp]
		public void BeforeEachTest()
		{
			_loop = new Mock<IDeployerLoop>();
			_projSel = new Mock<IProjectSelector>();
			_simKeys = new Mock<ISimultaneousKeys>();
			_webFactory = new Mock<IWebRequestFactory>();
			_garbage = new Mock<IGarbage>();

			_sut = new DeployerController(_loop.Object, _projSel.Object, _simKeys.Object, _webFactory.Object, _garbage.Object);
		}

		[Test]
		public void Preflight_If_Both_Keys_Off_Then_Initializes()
		{
			_simKeys.Setup(x => x.AreBothOn).Returns(false);
			_simKeys.Setup(x => x.AreBothOff).Returns(true);

			_sut.PreflightCheck();

			_loop.Verify(x => x.InitDone(), Times.Once);
		}

		[Test]
		public void Preflight_If_Both_Keys_On_Then_No_Init()
		{
			_simKeys.Setup(x => x.AreBothOn).Returns(true);
			_simKeys.Setup(x => x.AreBothOff).Returns(false);

			_sut.PreflightCheck();

			_loop.Verify(x => x.InitDone(), Times.Never);
		}

		[Test]
		public void TurnBothKeys_Mode_Turn_On_Only_KeyA_On_Causes_No_State_Change()
		{
			_loop.Setup(x => x.State).Returns(DeployerState.TurnBothKeys);
			_sut.KeyOnEvent(KeySwitch.KeyA);

			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyA), Times.Once);
			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyB), Times.Never);
			_simKeys.Verify(x => x.KeyOff(It.IsAny<KeySwitch>()), Times.Never);
		}

		[Test]
		public void TurnBothKeys_Mode_Turn_On_Only_KeyB_On_Causes_No_State_Change()
		{
			_loop.Setup(x => x.State).Returns(DeployerState.TurnBothKeys);
			_sut.KeyOnEvent(KeySwitch.KeyB);

			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyB), Times.Once);
			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyA), Times.Never);
			_simKeys.Verify(x => x.KeyOff(It.IsAny<KeySwitch>()), Times.Never);
		}

		[Test]
		public void TurnBothKeys_Mode_Turning_On_Both_Keys_Simultaneously_Succeeds()
		{
			_loop.Setup(x => x.State).Returns(DeployerState.TurnBothKeys);
			_simKeys.Setup(x => x.AreBothOn).Returns(true);
			_simKeys.Setup(x => x.AreBothOff).Returns(false);
			_simKeys.Setup(x => x.SwitchedSimultaneously).Returns(true);

			_sut.KeyOnEvent(KeySwitch.KeyA);

			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyA), Times.Once);
			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyB), Times.Never);
			_simKeys.Verify(x => x.KeyOff(It.IsAny<KeySwitch>()), Times.Never);
			_loop.Verify(x => x.Abort(), Times.Never);
			_loop.Verify(x => x.BothKeysTurned(), Times.Once);
		}

		[Test]
		public void TurnBothKeys_Mode_Turning_On_Both_Keys_But_NOT_Simultaneously_Aborts()
		{
			_loop.Setup(x => x.State).Returns(DeployerState.TurnBothKeys);
			_simKeys.Setup(x => x.AreBothOn).Returns(true);
			_simKeys.Setup(x => x.AreBothOff).Returns(false);
			_simKeys.Setup(x => x.SwitchedSimultaneously).Returns(false);

			_sut.KeyOnEvent(KeySwitch.KeyA);

			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyA), Times.Once);
			_simKeys.Verify(x => x.KeyOn(KeySwitch.KeyB), Times.Never);
			_simKeys.Verify(x => x.KeyOff(It.IsAny<KeySwitch>()), Times.Never);
			_loop.Verify(x => x.Abort(), Times.Once);
			_loop.Verify(x => x.BothKeysTurned(), Times.Never);
		}

		[Test]
		public void Init_Mode_Both_Keys_Off_Turning_Off_KeyA_Causes_InitDone()
		{
			_loop.Setup(x => x.State).Returns(DeployerState.Init);
			_simKeys.Setup(x => x.AreBothOn).Returns(false);
			_simKeys.Setup(x => x.AreBothOff).Returns(true);

			_sut.KeyOffEvent(KeySwitch.KeyA);

			_loop.Verify(x => x.InitDone(), Times.Once);
			_loop.Verify(x => x.Abort(), Times.Never);
			_simKeys.Verify(x => x.KeyOff(KeySwitch.KeyA), Times.Once);
			_simKeys.Verify(x => x.KeyOff(KeySwitch.KeyB), Times.Never);
			_simKeys.Verify(x => x.KeyOn(It.IsAny<KeySwitch>()), Times.Never);
		}

		[Test]
		public void Init_Mode_Both_Keys_NOT_Off_Turning_Off_KeyA_Causes_No_Change()
		{
			_loop.Setup(x => x.State).Returns(DeployerState.Init);
			_simKeys.Setup(x => x.AreBothOn).Returns(false);
			_simKeys.Setup(x => x.AreBothOff).Returns(false);

			_sut.KeyOffEvent(KeySwitch.KeyA);

			_loop.Verify(x => x.InitDone(), Times.Never);
			_loop.Verify(x => x.Abort(), Times.Never);
			_simKeys.Verify(x => x.KeyOff(KeySwitch.KeyA), Times.Once);
			_simKeys.Verify(x => x.KeyOff(KeySwitch.KeyB), Times.Never);
			_simKeys.Verify(x => x.KeyOn(It.IsAny<KeySwitch>()), Times.Never);
		}

		[Test]
		public void TurnBothKeys_Mode_Turning_Off_KeyA_Does_Nothing()
		{
			_loop.Setup(x => x.State).Returns(DeployerState.TurnBothKeys);

			_sut.KeyOffEvent(KeySwitch.KeyA);

			_loop.Verify(x => x.Abort(), Times.Never);
			_loop.Verify(x => x.InitDone(), Times.Never);
		}
	}
}