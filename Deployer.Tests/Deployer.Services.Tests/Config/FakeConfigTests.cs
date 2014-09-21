using Deployer.Services.Config;
using NUnit.Framework;

namespace Deployer.Tests.Config
{
	[TestFixture]
	public class FakeConfigTests
	{
		private FakeConfigurationService _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_sut = new FakeConfigurationService();
		}

		[Test]
		public void GetProjects()
		{
			var projects = _sut.GetProjects();
			Assert.AreEqual(4, projects.Length);
		}
	}
}