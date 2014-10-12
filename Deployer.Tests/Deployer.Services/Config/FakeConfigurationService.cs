using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Deployer.Services.Builders;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Models;

namespace Deployer.Services.Config
{
	[ExcludeFromCodeCoverage]
	public class FakeConfigurationService : IConfigurationService
	{
		private readonly ProjectModel[] _projects;

		public FakeConfigurationService()
		{
			_projects = new[]
				{
					new ProjectModel("fail-1", "Failer", "-Fails after 2s", 1, BuildServiceProvider.Failing),
					new ProjectModel("succeed-1", "Succeeder", "-Works after 2s", 2, BuildServiceProvider.Succeeding),
					new ProjectModel("appvey-1", "AntiShaun", "-AppVeyor", 3, BuildServiceProvider.AppVeyor),
					new ProjectModel("tc-1", "BogusTeamCity", "-TeamCity", 4, BuildServiceProvider.TeamCity)
				};
		}

		public ProjectModel[] GetProjects()
		{
			return _projects;
		}

		public void DeleteProject(string slug)
		{
		}

		public void SaveProject(ProjectModel newProject)
		{
		}

		public Hashtable GetBuildParams(string slug)
		{
			switch (slug)
			{
				case "appvey-1":
					return AntiShaunJson();

				case "tc-1":
					return TeamCityJson();

				default:
					return new Hashtable();
			}
		}

		public void SaveBuildParams(string slug, Hashtable config)
		{
		}

		public ProjectModel GetProject(string slug)
		{
			return _projects[0];
		}

		private static Hashtable AntiShaunJson()
		{
			return new Hashtable
				{
					{"apiToken", "ertwertwertwertwert"},
					{"accountName", "wertwertwert"},
					{"projectSlug", "ewrtwertwerter"},
					{"branch", "master"}
				};
		}

		private static Hashtable TeamCityJson()
		{
			return new Hashtable
				{
					{"url", "http://192.168.0.31:8111"},
					{"buildId", "TestProject_Fail2"},
					{"username", "spamagnet"},
					{"password", "kjs*11301"},
				};
		}
	}
}