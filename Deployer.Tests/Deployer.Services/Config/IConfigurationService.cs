using Deployer.Services.Models;

namespace Deployer.Services.Config
{
	public interface IConfigurationService
	{
		Project[] GetProjects();
	}
}