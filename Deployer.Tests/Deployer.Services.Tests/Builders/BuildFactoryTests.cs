using System;
using Deployer.Services.Builders;
using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests.Builders
{
	[TestFixture]
	public class BuildFactoryTests
	{
		private Mock<IWebRequestFactory> _webFactory;
		private Mock<IGarbage> _garbage;
		private Mock<IWebUtility> _netio;

		[SetUp]
		public void BeforeEachTest()
		{
			_webFactory = new Mock<IWebRequestFactory>();
			_garbage = new Mock<IGarbage>();
			_netio = new Mock<IWebUtility>();
		}

		[Test]
		public void Various()
		{
			var av = BuildServiceFactory.Create(BuildServiceProvider.AppVeyor, _webFactory.Object, _netio.Object, _garbage.Object);
			var tc = BuildServiceFactory.Create(BuildServiceProvider.TeamCity, _webFactory.Object, _netio.Object, _garbage.Object);
			var suc = BuildServiceFactory.Create(BuildServiceProvider.Succeeding, _webFactory.Object, _netio.Object,
			                                     _garbage.Object);
			var fail = BuildServiceFactory.Create(BuildServiceProvider.Failing, _webFactory.Object, _netio.Object,
			                                      _garbage.Object);

			Assert.IsNotNull(av);
			Assert.IsNotNull(tc);
			Assert.IsNotNull(suc);
			Assert.IsNotNull(fail);
		}

		[Test]
		[ExpectedException(typeof (Exception))]
		public void RejectInvalid()
		{
			const BuildServiceProvider provider = (BuildServiceProvider) 9999;
			var av = BuildServiceFactory.Create(provider, _webFactory.Object, _netio.Object, _garbage.Object);
		}
	}
}