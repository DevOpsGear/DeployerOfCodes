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
		/*
		private readonly ProjectModel[] _projects;

		public FakeConfigurationService()
		{
			_projects = new[]
				{
					new ProjectModel("fail-1", "Failer", "-Fails after 2s", 1, BuildServiceProvider.Failing),
					new ProjectModel("succeed-1", "Succeeder", "-Works after 2s", 2, BuildServiceProvider.Succeeding),
					new ProjectModel("appvey-1", "AntiShaun", "-AppVeyor", 3, BuildServiceProvider.AppVeyor),
					new ProjectModel("tc-f", "TeamCity fails", "-TeamCity", 4, BuildServiceProvider.TeamCity),
					new ProjectModel("tc-s", "TeamCity succeeds", "-TeamCity", 4, BuildServiceProvider.TeamCity),
					new ProjectModel("tc-x", "TeamCity missing", "-TeamCity", 4, BuildServiceProvider.TeamCity)
				};
		}
		*/

		public ProjectModel[] GetProjects()
		{
			//return _projects;
			return new[]
				{
					new ProjectModel("fail-1", "Failer", "-Fails after 2s", 1, BuildServiceProvider.Failing),
					new ProjectModel("succeed-1", "Succeeder", "-Works after 2s", 2, BuildServiceProvider.Succeeding),
					new ProjectModel("appvey-1", "AntiShaun", "-AppVeyor", 3, BuildServiceProvider.AppVeyor),
					new ProjectModel("tc-f", "TeamCity fails", "-TeamCity", 4, BuildServiceProvider.TeamCity),
					new ProjectModel("tc-s", "TeamCity succeeds", "-TeamCity", 4, BuildServiceProvider.TeamCity),
					new ProjectModel("tc-x", "TeamCity missing", "-TeamCity", 4, BuildServiceProvider.TeamCity)
				};
		}

		public void DeleteProject(string slug)
		{
		}

		public void SaveProject(ProjectModel newProject)
		{
		}

		public Hashtable GetBuildParams(string slug)
		{
			switch(slug)
			{
				case "appvey-1":
					return AntiShaunJson();

				case "tc-f":
					return TeamCityJsonFail();

				case "tc-s":
					return TeamCityJsonSucceed();

				case "tc-x":
					return TeamCityJsonMissing();
				default:
					return new Hashtable();
			}
		}

		public void SaveBuildParams(string slug, Hashtable config)
		{
		}

		public ProjectModel GetProject(string slug)
		{
			return GetProjects()[0];
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

		private static Hashtable TeamCityJsonFail()
		{
			return new Hashtable
				{
					{"url", "http://192.168.0.31:8111"},
					{"buildId", "TestProject_Fail10Sec"},
					{"username", "spamagnet"},
					{"password", "kjs*11301"},
				};
		}

		private static Hashtable TeamCityJsonSucceed()
		{
			return new Hashtable
				{
					{"url", "http://192.168.0.31:8111"},
					{"buildId", "TestProject_Succeeds10Sec"},
					{"username", "spamagnet"},
					{"password", "kjs*11301"},
				};
		}

		private static Hashtable TeamCityJsonMissing()
		{
			return new Hashtable
				{
					{"url", "http://192.168.0.31:8111"},
					{"buildId", "sdfjdfgkjdfkj"},
					{"username", "spamagnet"},
					{"password", "kjs*11301"},
				};
		}
	}
}