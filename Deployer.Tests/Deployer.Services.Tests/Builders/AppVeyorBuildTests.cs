using System.Collections;
using System.Text;
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
	public class AppVeyorBuildTests
	{
		private Mock<IGarbage> _garbage;
		private WebFactorySpy _webFactory;
		private AppVeyorBuildService _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_webFactory = new WebFactorySpy();
			_garbage = new Mock<IGarbage>();

			_sut = new AppVeyorBuildService(_webFactory, _garbage.Object);
		}

		[Test]
		public void Start()
		{
			var config = GetConfig();
			MockQueueBuild();

			var status = _sut.StartBuild(config);

			AssertQueued(status);
		}

		[Test]
		public void Stop()
		{
			MockCancelBuild();

			_sut.CancelBuild();
		}

		[Test]
		public void Status_is_queued()
		{
			MockStatus("queued");
			AssertQueued(_sut.GetStatus());
		}

		[Test]
		public void Status_is_running()
		{
			MockStatus("running");
			AssertRunning(_sut.GetStatus());
		}

		[Test]
		public void Status_is_failed()
		{
			MockStatus("failed");
			AssertFailed(_sut.GetStatus());
		}

		private void AssertQueued(BuildState state)
		{
			Assert.AreEqual(BuildStatus.Queued, state.Status);
		}

		private void AssertRunning(BuildState state)
		{
			Assert.AreEqual(BuildStatus.Running, state.Status);
		}

		private void AssertFailed(BuildState state)
		{
			Assert.AreEqual(BuildStatus.Failed, state.Status);
		}

		private void MockQueueBuild()
		{
			var fakeResponse = new Hashtable
				{
					{"version", "15"},
					{"status", "queued"},
				};
			var json = JsonSerializer.SerializeObject(fakeResponse);
			var data = Encoding.UTF8.GetBytes(json);
			var wr = _webFactory.SpyWebRequest;
			wr.SpyResponse.SetData(data);
		}

		private void MockCancelBuild()
		{
			var fakeResponse = new Hashtable
				{
					{"nothing", "really"},
					{"this", "does_not_yet_care"},
				};
			var json = JsonSerializer.SerializeObject(fakeResponse);
			var data = Encoding.UTF8.GetBytes(json);
			var wr = _webFactory.SpyWebRequest;
			wr.SpyResponse.SetData(data);
		}

		private void MockStatus(string status)
		{
			var fakeBuild = new Hashtable
				{
					{"status", status},
				};
			var fakeResponse = new Hashtable
				{
					{"build", fakeBuild},
				};
			var json = JsonSerializer.SerializeObject(fakeResponse);
			var data = Encoding.UTF8.GetBytes(json);
			var wr = _webFactory.SpyWebRequest;
			wr.SpyResponse.SetData(data);
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
	}
}