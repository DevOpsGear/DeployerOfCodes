using Deployer.Services.Builders;

namespace Deployer.Services.Models
{
	public class Project
	{
		public string Title { get; private set; }
		public string Subtitle { get; private set; }
		public BuildServiceProvider BuildServiceProvider { get; private set; }
		public string CiConfig { get; private set; }

		public Project(string title, string subtitle, BuildServiceProvider buildServiceProvider, string ciConfig)
		{
			Title = title;
			Subtitle = subtitle;
			BuildServiceProvider = buildServiceProvider;
			CiConfig = ciConfig;
		}
	}
}