using System.Collections;
using System.Text;
using Deployer.Services.Builders;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
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
		private WebFactorySpy _webFactory;
		private Mock<IGarbage> _garbage;
		private IWebUtility _netio;
		private AppVeyorBuildService _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_webFactory = new WebFactorySpy();
			_garbage = new Mock<IGarbage>();
			_netio = new WebUtility(_garbage.Object);

			_sut = new AppVeyorBuildService(_webFactory, _netio);
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

		[Test]
		public void Unexpected_status_causes_failure()
		{
			MockStatus("quack");
			AssertFailed(_sut.GetStatus());
		}

		[Test]
		public void Bad_status_causes_failure()
		{
			MockBadStatus();
			AssertFailed(_sut.GetStatus());
		}

		[Test]
		public void Bad_response_causes_failure()
		{
			var config = GetConfig();
			MockBadResponse();

			var status = _sut.StartBuild(config);

			AssertFailed(status);
		}

		[Test]
		public void Empty_config__causes_failure()
		{
			var status = _sut.StartBuild(new Hashtable());

			AssertFailed(status);
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

		private void MockBadResponse()
		{
			var fakeResponse = new Hashtable
				{
					{"erk", "ERKKK!"},
					{"flerbb", "BLERBBBB!"},
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

		private void MockBadStatus()
		{
			var fakeBlerg = new Hashtable
				{
					{"nerk", "NERK!"},
				};
			var fakeResponse = new Hashtable
				{
					{"blerg", fakeBlerg},
				};
			var json = JsonSerializer.SerializeObject(fakeResponse);
			var data = Encoding.UTF8.GetBytes(json);
			var wr = _webFactory.SpyWebRequest;
			wr.SpyResponse.SetData(data);
		}

		private static Hashtable GetConfig()
		{
			return new Hashtable
				{
					{"apiToken", "TOKEN"},
					{"accountName", "ACCOUNT"},
					{"projectSlug", "SLUG"},
					{"branch", "BRANCH"}
				};
		}
	}
}