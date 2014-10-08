using Deployer.Services.Config;
using NUnit.Framework;
using System.IO;

namespace Deployer.Tests.Config
{
	[TestFixture]
	public class SmallTextFileIoTests
	{
		private string _filePath;
		private SmallTextFileIo _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_filePath = Path.GetTempFileName();
			DeleteFile();
			_sut = new SmallTextFileIo();
		}

		[TearDown]
		public void AfterEachTest()
		{
			DeleteFile();
		}

		[Test]
		public void Write_then_read()
		{
			Assert.IsFalse(File.Exists(_filePath), "Start with no file");

			const string content = "test-content-yep-here-ya-go";
			_sut.Write(_filePath, content);
			var returnedContent = _sut.Read(_filePath);

			Assert.IsTrue(File.Exists(_filePath), "End up with a file");
			Assert.AreEqual(content, returnedContent, "Same content");
		}

		[Test]
		public void Read_non_existent_file()
		{
			var returnedContent = _sut.Read(_filePath);
			Assert.AreEqual("", returnedContent, "Empty");
		}

		private void DeleteFile()
		{
			if(File.Exists(_filePath))
				File.Delete(_filePath);
		}
	}
}