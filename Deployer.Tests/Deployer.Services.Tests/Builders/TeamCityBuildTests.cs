using System.Collections;
using Deployer.Services.Builders;
using Deployer.Services.Micro;
using Deployer.Services.Models;
using Deployer.Tests.SpiesFakes;
using Json.NETMF;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests.Builders
{
	[TestFixture]
	public class TeamCityBuildTests
	{
		private Mock<IGarbage> _garbage;
		private WebFactorySpy _webFactory;
		private TeamCityBuildService _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_webFactory = new WebFactorySpy();
			_garbage = new Mock<IGarbage>();

			_sut = new TeamCityBuildService(_webFactory, _garbage.Object);
		}

		[Test]
		public void Start()
		{
			var config = GetConfig();
			var status = _sut.StartBuild(config);

			AssertQueued(status);
		}

		private static string GetConfig()
		{
			var config = new Hashtable
				{
					{"apiToken", "TOKEN"},
					{"accountName", "ACCOUNT"},
					{"projectSlug", "SLUG"},
					{"branch", "BRANCH"}
				};
			return JsonSerializer.SerializeObject(config);
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