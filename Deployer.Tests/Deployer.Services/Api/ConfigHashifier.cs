using System.Text;
using Deployer.Services.Models;
using System.Collections;
using Json.NETMF;

namespace Deployer.Services.Api
{
	public static class ConfigHashifier
	{
		public static byte[] Bytify(ProjectModel[] projects)
		{
			var hash = Hashify(projects);
			var json = JsonSerializer.SerializeObject(hash);
			var bytes = Encoding.UTF8.GetBytes(json);
			return bytes;
		}

		public static byte[] Bytify(ProjectModel proj)
		{
			var hash = Hashify(proj);
			var json = JsonSerializer.SerializeObject(hash);
			var bytes = Encoding.UTF8.GetBytes(json);
			return bytes;
		}

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