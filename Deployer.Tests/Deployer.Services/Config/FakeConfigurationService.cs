using System.Collections;
using Deployer.Services.Build;
using Deployer.Services.Models;
using Json.NETMF;

namespace Deployer.Services.Config
{
	public class FakeConfigurationService : IConfigurationService
	{
		private readonly Project[] _projects;

		public FakeConfigurationService()
		{
			var antiShaunConfig = new Hashtable
				{
					{"apiToken", "5cysxk229kyjpq16lcsd"},
					{"accountName", "ebopensource"},
					{"projectSlug", "AntiShaun"},
					{"branch", "master"}
				};
			var antiShaunJson = JsonSerializer.SerializeObject(antiShaunConfig);

			_projects = new[]
				{
					new Project("Failer", "-Fails after 2s", BuildServiceProvider.Failing, string.Empty),
					new Project("Succeeder", "-Works after 2s", BuildServiceProvider.Succeeding, string.Empty),
					new Project("AntiShaun", "-AppVeyor", BuildServiceProvider.AppVeyor, antiShaunJson),
					new Project("BogusTeamCity", "-TeamCity", BuildServiceProvider.TeamCity, string.Empty)
				};
		}

		public Project[] GetProjects()
		{
			return _projects;
		}
	}
}