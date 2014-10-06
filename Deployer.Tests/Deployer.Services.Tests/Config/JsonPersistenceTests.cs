using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests.Config
{
	[TestFixture]
	public class JsonPersistenceTests
	{
		private Mock<ISmallTextFileIo> _fileIo;
		private JsonPersistence _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_fileIo = new Mock<ISmallTextFileIo>();
			_sut = new JsonPersistence(_fileIo.Object);
		}

		[Test]
		public void Write()
		{
			const string path = @"\sd\argh.json";
			const string expectedJson = @"{""key3"":""val3"",""key2"":2,""key1"":""val1""}";
			var hash = new Hashtable
				{
					{"key1", "val1"},
					{"key2", 2},
					{"key3", "val3"},
				};

			_sut.Write(path, hash);

			_fileIo.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			_fileIo.Verify(x => x.Write(path, expectedJson), Times.Once);
		}

		[Test]
		public void Read()
		{
			const string path = @"\sd\argh.json";
			const string incomingJson = @"{""key3"":""val3"",""key2"":2,""key1"":""val1""}";
			_fileIo.Setup(x => x.Read(path)).Returns(incomingJson);

			var hash = _sut.Read(path);

			Assert.AreEqual(3, hash.Count);
			Assert.AreEqual("val1", hash["key1"]);
			Assert.AreEqual(2, hash["key2"]);
			Assert.AreEqual("val3", hash["key3"]);
			_fileIo.Verify(x => x.Read(It.IsAny<string>()), Times.Once);
			_fileIo.Verify(x => x.Read(path), Times.Once);
		}
	}
}