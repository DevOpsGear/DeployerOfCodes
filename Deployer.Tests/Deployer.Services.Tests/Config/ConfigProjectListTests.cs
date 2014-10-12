using System.Collections;
using Deployer.Services.Api;
using Deployer.Services.Builders;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
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
		private ConfigurationService _sut;
		private ProjectModel _newProject;
		private ProjectModel _survivingProject;
		private ProjectModel _doomedProject;

		#region Tests

		[SetUp]
		public void BeforeEachTest()
		{
			_jsonPersist = new Mock<IJsonPersistence>();
			_slugCreator = new Mock<ISlugCreator>();

			_newProject = new ProjectModel("", "New project", "New subtitle", 5, BuildServiceProvider.Succeeding);
			_survivingProject = new ProjectModel("slug-1",
			                                     "Surviving project",
			                                     "Surviving subtitle", 3, BuildServiceProvider.Failing);
			_doomedProject = new ProjectModel("slug-2",
			                                  "Doomed project",
			                                  "Doomed subtitle",
			                                  5, BuildServiceProvider.Succeeding);

			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json")).Returns(new Hashtable());

			_sut = new ConfigurationService(@"\root\", _jsonPersist.Object, _slugCreator.Object);
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

			_slugCreator.Setup(x => x.CreateSlug(new string[0])).Returns(slug);
			_jsonPersist.Setup(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable hash) =>
				            {
					            Assert.AreEqual(1, hash.Count, "Count of projects being saved");
					            AssertContainsProject(hash, slug, _newProject);
				            });

			_sut.SaveProject(_newProject);

			_slugCreator.Verify(x => x.CreateSlug(new string[0]), Times.Once);
			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Once);
			_jsonPersist.Verify(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()), Times.Once);
		}

		[Test]
		public void Start_with_one_project_and_add_one()
		{
			var existingProject = MockReadExistingProject("slug-ex");

			const string newSlug = "slug-new";
			_slugCreator.Setup(x => x.CreateSlug(new[] {existingProject.Slug})).Returns(newSlug);
			_jsonPersist.Setup(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable hash) =>
				            {
					            Assert.AreEqual(2, hash.Count, "Count of projects being saved");
					            AssertContainsProject(hash, existingProject.Slug, existingProject);
					            AssertContainsProject(hash, newSlug, _newProject);
				            });

			_sut.SaveProject(_newProject);

			_slugCreator.Verify(x => x.CreateSlug(new[] {existingProject.Slug}), Times.Once);
			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Once);
			_jsonPersist.Verify(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()), Times.Once);
		}


		[Test]
		public void Modify_and_save_one_that_already_exists()
		{
			MockReadExistingProject("slug-ex");

			_jsonPersist.Setup(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable projects) =>
				            {
					            Assert.AreEqual(1, projects.Count, "Count of projects being saved");
					            var projHash = projects["slug-ex"] as Hashtable;
					            Assert.IsNotNull(projHash);
					            Assert.AreEqual("Updated title", projHash["title"]);
					            Assert.AreEqual("Updated subtitle", projHash["subtitle"]);
					            Assert.AreEqual(9999, projHash["rank"]);
					            Assert.AreEqual((int) BuildServiceProvider.TeamCity, projHash["provider"]);
				            });

			var proj = _sut.GetProject("slug-ex");
			proj.Title = "Updated title";
			proj.Subtitle = "Updated subtitle";
			proj.Rank = 9999;
			proj.Provider = BuildServiceProvider.TeamCity;
			_sut.SaveProject(proj);

			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Exactly(2));
			_jsonPersist.Verify(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()), Times.Once);
		}

		[Test]
		public void Start_with_two_project_and_delete_one()
		{
			var survivingHash = ConfigHashifier.Hashify(_survivingProject);
			var doomedHash = ConfigHashifier.Hashify(_doomedProject);

			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json"))
			            .Returns(new Hashtable
				            {
					            {_survivingProject.Slug, survivingHash},
					            {_doomedProject.Slug, doomedHash}
				            });

			_jsonPersist.Setup(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()))
			            .Callback((string path, Hashtable hash) =>
				            {
					            Assert.AreEqual(1, hash.Count, "Count of projects being saved");
					            AssertContainsProject(hash, _survivingProject.Slug, _survivingProject);
				            });

			_sut.DeleteProject(_doomedProject.Slug);

			_slugCreator.Verify(x => x.CreateSlug(It.IsAny<string[]>()), Times.Never);
			_jsonPersist.Verify(x => x.Read(@"\root\config\projects.json"), Times.Once);
			_jsonPersist.Verify(x => x.Write(@"\root\config\projects.json", It.IsAny<Hashtable>()), Times.Once);
		}

		[Test]
		[ExpectedException(typeof(ProjectDoesNotExistException))]
		public void Delete_non_existent_project()
		{
			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json"))
			            .Returns(new Hashtable());

			_sut.DeleteProject("bad-slug");
		}

		[Test]
		[ExpectedException(typeof(ProjectDoesNotExistException))]
		public void Get_one_that_does_not_exist()
		{
			MockReadExistingProject("slug-ex");

			_sut.GetProject("slug-non-exist");
		}

		[Test]
		public void Skip_hash_with_null_value()
		{
			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json"))
			            .Returns(new Hashtable
				            {
					            {"slug", null}
				            });
			_sut.GetProjects();
		}

		[Test]
		public void Get_one_that_does_exist()
		{
			var expected = MockReadExistingProject("slug-ex");

			var actual = _sut.GetProject("slug-ex");

			Assert.AreEqual(expected.Slug, actual.Slug);
			Assert.AreEqual(expected.Slug, actual.Slug);
			Assert.AreEqual(expected.Slug, actual.Slug);
		}

		#endregion

		#region Private

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

		private ProjectModel MockReadExistingProject(string slug)
		{
			var existingProject = new ProjectModel(slug,
			                                       "Existing project",
			                                       "Existing subtitle",
			                                       3, BuildServiceProvider.Failing);

			var existingHash = ConfigHashifier.Hashify(existingProject);
			_jsonPersist.Setup(x => x.Read(@"\root\config\projects.json"))
			            .Returns(new Hashtable
				            {
					            {existingProject.Slug, existingHash}
				            });
			return existingProject;
		}

		#endregion
	}
}