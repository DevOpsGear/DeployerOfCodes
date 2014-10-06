using Deployer.Services.Config;
using NUnit.Framework;

namespace Deployer.Tests.Config
{
	[TestFixture]
	public class SlugCreatorTests
	{
		private SlugCreator _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_sut = new SlugCreator();
		}

		[Test]
		public void No_existing_slugs()
		{
			var emptySlugs = new string[0];
			var actual = _sut.CreateSlug(emptySlugs);
			Assert.AreEqual("project-1", actual);
		}

		[Test]
		public void One_good_existing_slug()
		{
			var oneSlug = new[] {"thiing-2"};
			var actual = _sut.CreateSlug(oneSlug);
			Assert.AreEqual("project-3", actual);
		}

		[Test]
		public void One_bad_existing_slug()
		{
			var oneSlug = new[] {"fnargiffiffif"};
			var actual = _sut.CreateSlug(oneSlug);
			Assert.AreEqual("project-1", actual);
		}

		[Test]
		public void Several_good_and_one_bad_existing_slug()
		{
			var oneSlug = new[] {"flop-4", "fnargiffiffif", "wingo-2"};
			var actual = _sut.CreateSlug(oneSlug);
			Assert.AreEqual("project-5", actual);
		}
	}
}