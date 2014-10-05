using System.Collections;
using Deployer.Services.Models;

namespace Deployer.Services.Config
{
	public interface IConfigurationService
	{
		Project[] GetProjects();
		ProjectDomainModel[] GetProjectList();
		void DeleteProject(string slug);
		void SaveProjectInfo(ProjectDomainModel project);
		Hashtable GetProjectConfig(string slug);
		void SaveProjectConfig(string slug, Hashtable config);
	}
}