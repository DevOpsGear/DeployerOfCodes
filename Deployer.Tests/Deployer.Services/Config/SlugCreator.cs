using System;

namespace Deployer.Services.Config
{
	public interface ISlugCreator
	{
		string CreateSlug(string[] existingSlugs);
	}

	public class SlugCreator : ISlugCreator
	{
		private const string Prefix = "project";

		public string CreateSlug(string[] existingSlugs)
		{
			var maxExisting = 0;
			foreach (var slug in existingSlugs)
			{
				var idxSep = slug.IndexOf("-");
				try
				{
					var value = Int32.Parse(slug.Substring(idxSep + 1));
					maxExisting = Math.Max(maxExisting, value);
				}
				catch
				{
				}
			}
			return Prefix + "-" + (maxExisting + 1).ToString();
		}
	}
}