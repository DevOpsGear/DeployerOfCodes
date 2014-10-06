using System.Collections;
using Deployer.Services.Builders;
using Deployer.Services.Config;
using Deployer.Services.Models;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests.Config
{
	[TestFixture]
	public class ConfigBuildParamTests
	{
		private Mock<IJsonPersistence> _jsonPersist;
		private Mock<ISlugCreator> _slugCreator;
		private RealConfigurationService _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_jsonPersist = new Mock<IJsonPersistence>();
			_slugCreator = new Mock<ISlugCreator>();

			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json")).Returns(new Hashtable());

			_sut = new RealConfigurationService(@"\root\", _jsonPersist.Object, _slugCreator.Object);
		}

		[Test]
		public void Start_with_zero_projects()
		{
			//_sut.GetBuildParams();
		}
	}
}