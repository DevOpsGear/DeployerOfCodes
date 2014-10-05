using Deployer.Services.Builders;

namespace Deployer.Services.Models
{
	public class ProjectDomainModel
	{
		public string Slug { get; set; }
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public int Rank { get; set; }
		public BuildServiceProvider Provider { get; set; }
	}
}