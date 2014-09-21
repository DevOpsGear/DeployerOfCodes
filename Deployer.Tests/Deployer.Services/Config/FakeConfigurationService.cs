using System.Collections;
using Deployer.Services.Builders;
using Deployer.Services.Models;
using Json.NETMF;

namespace Deployer.Services.Config
{
	public class FakeConfigurationService : IConfigurationService
	{
		private readonly Project[] _projects;

		public FakeConfigurationService()
		{
			var antiShaunJson = AntiShaunJson();
			var teamCitynJson = TeamCityJson();

			_projects = new[]
				{
					new Project("Failer", "-Fails after 2s", BuildServiceProvider.Failing, string.Empty),
					new Project("Succeeder", "-Works after 2s", BuildServiceProvider.Succeeding, string.Empty),
					new Project("AntiShaun", "-AppVeyor", BuildServiceProvider.AppVeyor, antiShaunJson),
					new Project("BogusTeamCity", "-TeamCity", BuildServiceProvider.TeamCity, teamCitynJson)
				};
		}

		public Project[] GetProjects()
		{
			return _projects;
		}

		private static string AntiShaunJson()
		{
			var antiShaunConfig = new Hashtable
				{
					{"apiToken", "ertwertwertwertwert"},
					{"accountName", "wertwertwert"},
					{"projectSlug", "ewrtwertwerter"},
					{"branch", "master"}
				};
			return JsonSerializer.SerializeObject(antiShaunConfig);
		}

		private string TeamCityJson()
		{
			var antiShaunConfig = new Hashtable
				{
					{"url", "http://wertwertwert:8080"},
					{"buildId", "ertwertwertw"},
					{"username", "ertwertwert"},
					{"password", "wertwertwertwert"},
				};
			return JsonSerializer.SerializeObject(antiShaunConfig);
		}
	}
}