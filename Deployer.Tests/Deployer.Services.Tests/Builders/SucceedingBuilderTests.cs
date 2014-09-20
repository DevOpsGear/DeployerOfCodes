using Deployer.Services.Builders;
using Deployer.Services.Models;
using NUnit.Framework;

namespace Deployer.Tests.Builders
{
	[TestFixture]
	public class SucceedingBuilderTests
	{
		private SuceedingBuildService _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_sut = new SuceedingBuildService();
		}

		[Test]
		public void Start()
		{
			var status = _sut.StartBuild(string.Empty);
			AssertQueued(status);
		}

		[Test]
		public void Stop()
		{
			_sut.CancelBuild();
		}

		[Test]
		public void Sequence()
		{
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertQueued(_sut.GetStatus());
			AssertRunning(_sut.GetStatus());
			AssertRunning(_sut.GetStatus());
			AssertRunning(_sut.GetStatus());
			AssertRunning(_sut.GetStatus());
			AssertRunning(_sut.GetStatus());
			AssertSucceeded(_sut.GetStatus());
			AssertSucceeded(_sut.GetStatus());
			AssertSucceeded(_sut.GetStatus());
			AssertSucceeded(_sut.GetStatus());
			AssertSucceeded(_sut.GetStatus());
		}

		private void AssertQueued(BuildState state)
		{
			Assert.AreEqual(BuildStatus.Queued, state.Status);
		}

		private void AssertRunning(BuildState state)
		{
			Assert.AreEqual(BuildStatus.Running, state.Status);
		}

		private void AssertSucceeded(BuildState state)
		{
			Assert.AreEqual(BuildStatus.Succeeded, state.Status);
		}
	}
}