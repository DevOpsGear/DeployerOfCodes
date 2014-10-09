using Deployer.Services.Builders;

namespace Deployer.Services.Models
{
	public class ProjectModel
	{
		public ProjectModel()
		{
			Slug = "";
			Title = "";
			Subtitle = "";
			Rank = 0;
			Provider = BuildServiceProvider.Failing;
		}

		public ProjectModel(string slug, string title, string subtitle, int rank, BuildServiceProvider provider)
		{
			Slug = slug;
			Title = title;
			Subtitle = subtitle;
			Rank = rank;
			Provider = provider;
		}

		public string Slug { get; set; }
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public int Rank { get; set; }
		public BuildServiceProvider Provider { get; set; }
	}
}