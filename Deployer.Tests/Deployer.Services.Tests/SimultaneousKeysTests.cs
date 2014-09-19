using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Input;
using Deployer.Tests.SpiesFakes;
using NUnit.Framework;

namespace Deployer.Tests
{
	[TestFixture]
	public class SimultaneousKeysTests
	{
		private TimeServiceFake _time;
		private SimultaneousKeys _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_time = new TimeServiceFake();
			_sut = new SimultaneousKeys(false, false, _time);
		}

		[Test]
		public void Starts_with_both_off()
		{
			Assert.IsTrue(_sut.AreBothOff);
			Assert.IsFalse(_sut.AreBothOn);
		}

		[Test]
		public void Turn_A()
		{
			_sut.KeyOn(KeySwitch.KeyA);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsFalse(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_B()
		{
			_sut.KeyOn(KeySwitch.KeyB);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsFalse(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_A_then_B_after_300_msec_does_nothing()
		{
			_sut.KeyOn(KeySwitch.KeyA);
			_time.AddMilliseconds(300);
			_sut.KeyOn(KeySwitch.KeyB);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsTrue(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_B_then_A_after_300_msec_does_nothing()
		{
			_sut.KeyOn(KeySwitch.KeyB);
			_time.AddMilliseconds(300);
			_sut.KeyOn(KeySwitch.KeyA);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsTrue(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_A_then_B_after_100_msec_triggers_simultaneous()
		{
			_sut.KeyOn(KeySwitch.KeyA);
			_time.AddMilliseconds(100);
			_sut.KeyOn(KeySwitch.KeyB);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsTrue(_sut.AreBothOn);
			Assert.IsTrue(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_B_then_A_after_100_msec_triggers_simultaneous()
		{
			_sut.KeyOn(KeySwitch.KeyB);
			_time.AddMilliseconds(100);
			_sut.KeyOn(KeySwitch.KeyA);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsTrue(_sut.AreBothOn);
			Assert.IsTrue(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_B_then_A_after_100_msec_then_B_off_does_nothing()
		{
			_sut.KeyOn(KeySwitch.KeyB);
			_time.AddMilliseconds(100);
			_sut.KeyOn(KeySwitch.KeyA);
			_time.AddMilliseconds(100);
			_sut.KeyOff(KeySwitch.KeyB);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsFalse(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_B_then_A_after_100_msec_then_A_off_does_nothing()
		{
			_sut.KeyOn(KeySwitch.KeyB);
			_time.AddMilliseconds(100);
			_sut.KeyOn(KeySwitch.KeyA);
			_time.AddMilliseconds(100);
			_sut.KeyOff(KeySwitch.KeyA);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsFalse(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_A_then_B_after_100_msec_then_A_off_does_nothing()
		{
			_sut.KeyOn(KeySwitch.KeyA);
			_time.AddMilliseconds(100);
			_sut.KeyOn(KeySwitch.KeyB);
			_time.AddMilliseconds(100);
			_sut.KeyOff(KeySwitch.KeyA);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsFalse(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}

		[Test]
		public void Turn_A_then_B_after_100_msec_then_B_off_does_nothing()
		{
			_sut.KeyOn(KeySwitch.KeyA);
			_time.AddMilliseconds(100);
			_sut.KeyOn(KeySwitch.KeyB);
			_time.AddMilliseconds(100);
			_sut.KeyOff(KeySwitch.KeyB);

			Assert.IsFalse(_sut.AreBothOff);
			Assert.IsFalse(_sut.AreBothOn);
			Assert.IsFalse(_sut.SwitchedSimultaneously);
		}
	}
}