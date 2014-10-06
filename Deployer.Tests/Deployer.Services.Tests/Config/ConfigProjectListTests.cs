using System.Collections;
using Deployer.Services.Builders;
using Deployer.Services.Config;
using Deployer.Services.Models;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests.Config
{
	[TestFixture]
	public class ConfigProjectListTests
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
			var projects = _sut.GetProjects();
			Assert.AreEqual(0, projects.Length);
			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Once);
		}

		[Test]
		public void Start_with_zero_projects_and_add_one()
		{
			const string slug = "slug-1";
			var project = new ProjectModel("", "New project", "New subtitle", 5, BuildServiceProvider.Succeeding);

			_slugCreator.Setup(x => x.CreateSlug(new string[0])).Returns(slug);
			_jsonPersist.Setup(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable hash) =>
				            {
					            Assert.AreEqual(1, hash.Count, "Count of projects being saved");
					            AssertContainsProject(hash, slug, project);
				            });

			_sut.SaveProject(project);

			_slugCreator.Verify(x => x.CreateSlug(new string[0]), Times.Once);
			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Once);
			_jsonPersist.Verify(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()), Times.Once);
		}

		[Test]
		public void Start_with_one_project_and_add_one()
		{
			var existingProject = new ProjectModel("slug-ex",
			                                       "Existing project",
			                                       "Existing subtitle",
			                                       3, BuildServiceProvider.Failing);
			const string newSlug = "slug-new";
			var newProject = new ProjectModel("",
			                                  "New project",
			                                  "New subtitle",
			                                  5, BuildServiceProvider.Succeeding);

			var existingHash = HashFromProject(existingProject);
			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json"))
			            .Returns(new Hashtable
				            {
					            {existingProject.Slug, existingHash}
				            });

			_slugCreator.Setup(x => x.CreateSlug(new[] {existingProject.Slug})).Returns(newSlug);
			_jsonPersist.Setup(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable hash) =>
				            {
					            Assert.AreEqual(2, hash.Count, "Count of projects being saved");
					            AssertContainsProject(hash, existingProject.Slug, existingProject);
					            AssertContainsProject(hash, newSlug, newProject);
				            });

			_sut.SaveProject(newProject);

			_slugCreator.Verify(x => x.CreateSlug(new[] {existingProject.Slug}), Times.Once);
			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Once);
			_jsonPersist.Verify(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()), Times.Once);
		}

		[Test]
		public void Start_with_two_project_and_delete_one()
		{
			var survivingProject = new ProjectModel("slug-1",
			                                        "Surviving project",
			                                        "Surviving subtitle", 3, BuildServiceProvider.Failing);
			var survivingHash = HashFromProject(survivingProject);
			var doomedProject = new ProjectModel("slug-2",
			                                     "Doomed project",
			                                     "Doomed subtitle",
			                                     5, BuildServiceProvider.Succeeding);
			var doomedHash = HashFromProject(doomedProject);

			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json"))
			            .Returns(new Hashtable
				            {
					            {survivingProject.Slug, survivingHash},
					            {doomedProject.Slug, doomedHash}
				            });

			_jsonPersist.Setup(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable hash) =>
				            {
					            Assert.AreEqual(1, hash.Count, "Count of projects being saved");
					            AssertContainsProject(hash, survivingProject.Slug, survivingProject);
				            });

			_sut.DeleteProject(doomedProject.Slug);

			_slugCreator.Verify(x => x.CreateSlug(It.IsAny<string[]>()), Times.Never);
			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Once);
			_jsonPersist.Verify(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()), Times.Once);
		}

		private static void AssertContainsProject(IDictionary hash, string slug, ProjectModel project)
		{
			Assert.IsTrue(hash.Contains(slug), "Slug exists in hash");
			var pro = (Hashtable) hash[slug];
			Assert.IsNotNull(pro, "Project exists");
			Assert.AreEqual(project.Title, pro["title"], "title");
			Assert.AreEqual(project.Subtitle, pro["subtitle"], "subtitle");
			Assert.AreEqual(project.Rank, pro["rank"], "rank");
			Assert.AreEqual((int) project.Provider, pro["provider"], "provider");
		}

		private Hashtable HashFromProject(ProjectModel proj)
		{
			return new Hashtable
				{
					{"title", proj.Title},
					{"subtitle", proj.Subtitle},
					{"rank", proj.Rank},
					{"provider", proj.Provider},
				};
		}
	}
}