using System;
using Deployer.Services.Builders;
using Deployer.Services.Config;
using Deployer.Services.Input;
using Deployer.Services.Models;
using Deployer.Tests.SpiesFakes;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests.Input
{
	[TestFixture]
	public class ProjectSelectorTests
	{
		private CharDisplaySpy _display;
		private Mock<IConfigurationService> _config;
		private Project _projectOne;
		private Project _projectTwo;
		private ProjectSelector _sut;

		[SetUp]
		public void BeforeEachTest()
		{
			_display = new CharDisplaySpy();
			_config = new Mock<IConfigurationService>();
			_projectOne = new Project("Project 1", "Subtitle 1", BuildServiceProvider.Succeeding, string.Empty);
			_projectTwo = new Project("Project 2", "Subtitle 2", BuildServiceProvider.Failing, string.Empty);
			_sut = new ProjectSelector(_display, _config.Object);
		}

		[Test]
		public void Starts_with_no_project_aelected()
		{
			Assert.IsFalse(_sut.IsProjectSelected);
		}

		[Test]
		[ExpectedException(typeof (Exception))]
		public void Fails_when_attempting_to_get_project_model_with_none_is_selected()
		{
			Assert.IsFalse(_sut.IsProjectSelected);
			var proj = _sut.SelectedProject;
		}

		[Test]
		[ExpectedException(typeof (Exception))]
		public void Fails_when_attempting_to_get_project_name_with_none_is_selected()
		{
			Assert.IsFalse(_sut.IsProjectSelected);
			var proj = _sut.SelectedProjectName;
		}

		[Test]
		public void Down_selects_first_project()
		{
			MockConfigOneProject();

			_sut.Down();

			AssertProjectSelected(_projectOne);
		}

		[Test]
		public void Down_twice_with_only_one_project_stays_on_first()
		{
			MockConfigOneProject();

			_sut.Down();
			_sut.Down();

			AssertProjectSelected(_projectOne);
		}

		[Test]
		public void Up_immediately_does_nothing()
		{
			MockConfigTwoProjects();

			_sut.Up();

			AssertNothingSelected();
		}

		[Test]
		public void Down_selects_first_project_then_up_remains()
		{
			MockConfigTwoProjects();

			_sut.Down();
			_sut.Up();

			AssertProjectSelected(_projectOne);
		}

		[Test]
		public void Down_selects_first_project_then_up_twice_remains()
		{
			MockConfigTwoProjects();

			_sut.Down();
			_sut.Up();
			_sut.Up();

			AssertProjectSelected(_projectOne);
		}

		[Test]
		public void Down_first_down_second_up_first()
		{
			MockConfigTwoProjects();

			_sut.Down();
			AssertProjectSelected(_projectOne);
			_sut.Down();
			AssertProjectSelected(_projectTwo);
			_sut.Up();
			AssertProjectSelected(_projectOne);
		}

		[Test]
		public void With_no_projects_down_does_nothing()
		{
			MockConfigZeroProjects();

			_sut.Down();

			AssertNothingSelected();
		}

		private void MockConfigZeroProjects()
		{
			var models = new Project[] {};
			_config.Setup(x => x.GetProjects()).Returns(models);
		}

		private void MockConfigOneProject()
		{
			var models = new Project[]
				{
					_projectOne
				};
			_config.Setup(x => x.GetProjects()).Returns(models);
		}

		private void MockConfigTwoProjects()
		{
			var models = new Project[]
				{
					_projectOne,
					_projectTwo
				};
			_config.Setup(x => x.GetProjects()).Returns(models);
		}

		private void AssertNothingSelected()
		{
			Assert.IsFalse(_sut.IsProjectSelected, "Nothing selected");
			Assert.AreEqual(string.Empty, _display.Line1, "Display line 1");
			Assert.AreEqual(string.Empty, _display.Line2, "Display line 2");
		}

		private void AssertProjectSelected(Project proj)
		{
			Assert.IsTrue(_sut.IsProjectSelected, "Is selected");
			Assert.AreEqual(proj.Title, _sut.SelectedProjectName, "Name");
			Assert.AreEqual(proj, _sut.SelectedProject, "Model");
			Assert.AreEqual(proj.Title, _display.Line1, "Display line 1");
			Assert.AreEqual(proj.Subtitle, _display.Line2, "Display line 2");
		}
	}
}