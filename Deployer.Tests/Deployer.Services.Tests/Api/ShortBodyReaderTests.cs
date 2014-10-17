using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Api;
using NUnit.Framework;

namespace Deployer.Tests.Api
{
	[TestFixture]
	internal class ShortBodyReaderTests
	{
		[SetUp]
		public void BeforeEachTest()
		{
		}

		[Test]
		public void Do()
		{
			const int expectedLength = 128 + 5;
			var buffer = new byte[1024];
			var fakeBody = new FakeApiBody();

			var countBytes = ShortBodyReader.ReadBody(fakeBody, buffer);

			Assert.AreEqual(expectedLength, countBytes);
			for(var idx = 0; idx < expectedLength; idx++)
			{
				Assert.AreEqual((byte) idx, buffer[idx]);
			}
		}
	}
}