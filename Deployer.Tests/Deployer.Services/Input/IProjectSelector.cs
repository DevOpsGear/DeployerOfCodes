using Deployer.Services.Models;

namespace Deployer.Services.Input
{
	public interface IProjectSelector
	{
		void Reset();
		void Up();
		void Down();
		bool IsProjectSelected { get; }
		ProjectModel SelectedProject { get; }
		string SelectedProjectName { get; }
	}
}