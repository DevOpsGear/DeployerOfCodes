using Deployer.Services.Api;
using NUnit.Framework;

namespace Deployer.Tests.Api
{
	[TestFixture]
	internal class UrlSplitterTests
	{
		[Test]
		public void Entirely_empty()
		{
			var sut = new UrlSplitter("");
			Assert.AreEqual("", sut.Endpoint);
			Assert.AreEqual("", sut.Id);
			Assert.AreEqual("", sut.Option);
			Assert.AreEqual("", sut.Moar);
		}

		[Test]
		public void Default_with_trailing_slash()
		{
			var sut = new UrlSplitter("projects/");
			Assert.AreEqual("projects", sut.Endpoint);
			Assert.AreEqual("", sut.Id);
			Assert.AreEqual("", sut.Option);
			Assert.AreEqual("", sut.Moar);
		}

		[Test]
		public void Default_without_trailing_slash()
		{
			var sut = new UrlSplitter("projects");
			Assert.AreEqual("projects", sut.Endpoint);
			Assert.AreEqual("", sut.Id);
			Assert.AreEqual("", sut.Option);
			Assert.AreEqual("", sut.Moar);
		}

		[Test]
		public void With_slug_with_trailing_slash()
		{
			var sut = new UrlSplitter("projects/slug/");
			Assert.AreEqual("projects", sut.Endpoint);
			Assert.AreEqual("slug", sut.Id);
			Assert.AreEqual("", sut.Option);
			Assert.AreEqual("", sut.Moar);
		}


		[Test]
		public void With_slug_without_trailing_slash()
		{
			var sut = new UrlSplitter("projects/slug");
			Assert.AreEqual("projects", sut.Endpoint);
			Assert.AreEqual("slug", sut.Id);
			Assert.AreEqual("", sut.Option);
			Assert.AreEqual("", sut.Moar);
		}

		[Test]
		public void With_slug_build_with_trailing_slash()
		{
			var sut = new UrlSplitter("projects/slug/build/");
			Assert.AreEqual("projects", sut.Endpoint);
			Assert.AreEqual("slug", sut.Id);
			Assert.AreEqual("build", sut.Option);
			Assert.AreEqual("", sut.Moar);
		}

		[Test]
		public void With_slug_build_without_trailing_slash()
		{
			var sut = new UrlSplitter("projects/slug/build");
			Assert.AreEqual("projects", sut.Endpoint);
			Assert.AreEqual("slug", sut.Id);
			Assert.AreEqual("build", sut.Option);
			Assert.AreEqual("", sut.Moar);
		}

		[Test]
		public void With_lots_of_things()
		{
			var sut = new UrlSplitter("projects/slug/build/blerg/snerf/collapsium");
			Assert.AreEqual("projects", sut.Endpoint);
			Assert.AreEqual("slug", sut.Id);
			Assert.AreEqual("build", sut.Option);
			Assert.AreEqual("blerg", sut.Moar);
		}
	}
}