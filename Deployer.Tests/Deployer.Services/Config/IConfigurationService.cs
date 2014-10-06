using System.Collections;
using Deployer.Services.Models;

namespace Deployer.Services.Config
{
	public interface IConfigurationService
	{
		ProjectModel[] GetProjects();
		void DeleteProject(string slug);
		void SaveProject(ProjectModel newProject);
		Hashtable GetBuildParams(string slug);
		void SaveBuildParams(string slug, Hashtable config);
	}
}