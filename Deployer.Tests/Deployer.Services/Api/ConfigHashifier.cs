using Deployer.Services.Models;
using System.Collections;

namespace Deployer.Services.Api
{
	public static class ConfigHashifier
	{
		public static Hashtable Hashify(ProjectModel proj)
		{
			var hash = new Hashtable();

			hash["title"] = proj.Title;
			hash["subtitle"] = proj.Subtitle;
			hash["rank"] = proj.Rank;
			hash["provider"] = (int) proj.Provider;

			return hash;
		}

		public static Hashtable Hashify(ProjectModel[] projects)
		{
			var hashList = new Hashtable();
			foreach(var proj in projects)
			{
				var hash = Hashify(proj);
				hashList.Add(proj.Slug, hash);
			}
			return hashList;
		}
	}
}