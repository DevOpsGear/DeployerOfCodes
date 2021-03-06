﻿using Deployer.Services.Builders;
using Deployer.Services.Micro.Web;
using Deployer.Services.Models;
using Deployer.Tests.SpiesFakes;
using Moq;
using NeonMika.Interfaces;
using NUnit.Framework;
using System.Collections;

namespace Deployer.Tests.Builders
{
	[TestFixture]
	public class TeamCityBuildTests
	{
		private Mock<IGarbage> _garbage;
		private IWebUtility _webUtility;
		private WebFactorySpy _webFactory;
		private TeamCityBuildService _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_webFactory = new WebFactorySpy();
			_garbage = new Mock<IGarbage>();
			_webUtility = new WebUtility(_garbage.Object);

			_sut = new TeamCityBuildService(_webFactory, _webUtility);
		}

		[Test]
		public void Bad_config()
		{
			var status = _sut.StartBuild(null);

			AssertFailed(status);
		}

		[Test]
		public void Start()
		{
			var config = GetConfig();
			var status = _sut.StartBuild(config);

			AssertQueued(status);
		}

		[Test]
		public void Empty_config_causes_failure()
		{
			var status = _sut.StartBuild(new Hashtable());

			AssertFailed(status);
		}

		[Test]
		public void Bad_response_causes_failure()
		{
			MockBadResponse();
			var status = _sut.StartBuild(new Hashtable());

			AssertFailed(status);
		}

		[Test]
		public void Sequence()
		{
			AssertRunning(_sut.GetStatus());
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

		private void AssertFailed(BuildState state)
		{
			Assert.AreEqual(BuildStatus.Failed, state.Status);
		}

		private static Hashtable GetConfig()
		{
			return new Hashtable
				{
					{"url", "URL"},
					{"buildId", "BUILDID"},
					{"username", "USERNAME"},
					{"password", "PASSWORD"}
				};
		}

		private void MockBadResponse()
		{
			var wr = _webFactory.SpyWebRequest;
			wr.SpyResponse.SetData(null);
		}
	}
}