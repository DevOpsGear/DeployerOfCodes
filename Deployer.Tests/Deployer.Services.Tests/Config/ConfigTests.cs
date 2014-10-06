using Deployer.Services.Config;
using Moq;
using NUnit.Framework;
using System.Collections;

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

			_sut = new RealConfigurationService(@"\root\", _jsonPersist.Object, _slugCreator.Object);
		}

		[Test]
		public void Empty_build_params()
		{
			_jsonPersist.Setup(x => x.Read(@"\root\config\slug-1.json")).Returns(new Hashtable());

			var hash = _sut.GetBuildParams("slug-1");

			Assert.IsNotNull(hash);
			Assert.AreEqual(0, hash.Count);
			_jsonPersist.Verify(x => x.Read(It.IsAny<string>()), Times.Once);
			_jsonPersist.Verify(x => x.Read(@"\root\config\slug-1.json"), Times.Once);
		}

		[Test]
		public void Read_params()
		{
			var mockHash = new Hashtable
				{
					{"key1", "value1"},
					{"key2", "value2"}
				};
			_jsonPersist.Setup(x => x.Read(@"\root\config\slug-1.json")).Returns(mockHash);

			var actualHash = _sut.GetBuildParams("slug-1");

			Assert.IsNotNull(actualHash);
			Assert.AreEqual(2, actualHash.Count);
			Assert.AreEqual("value1", actualHash["key1"]);
			Assert.AreEqual("value2", actualHash["key2"]);
			_jsonPersist.Verify(x => x.Read(It.IsAny<string>()), Times.Once);
			_jsonPersist.Verify(x => x.Read(@"\root\config\slug-1.json"), Times.Once);
		}

		[Test]
		public void Write_params()
		{
			var mockHash = new Hashtable
				{
					{"key1", "value1"},
					{"key2", "value2"}
				};
			_jsonPersist.Setup(x => x.Write(@"\root\config\slug-1.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable hash) =>
				            {
					            Assert.AreEqual(2, hash.Count);
					            Assert.AreEqual("value1", hash["key1"]);
					            Assert.AreEqual("value2", hash["key2"]);
				            });

			_sut.SaveBuildParams("slug-1", mockHash);

			_jsonPersist.Verify(x => x.Write(@"\root\config\slug-1.json", It.IsAny<Hashtable>()), Times.Once);
		}
	}
}