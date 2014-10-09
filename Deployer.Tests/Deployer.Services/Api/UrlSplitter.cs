using Deployer.Services.Util;

namespace Deployer.Services.Api
{
	public class UrlSplitter
	{
		public string Endpoint { get; set; }
		public string Id { get; set; }
		public string Option { get; set; }
		public string Moar { get; set; }

		public UrlSplitter(string url)
		{
			Endpoint = "";
			Id = "";
			Option = "";
			Moar = "";

			var segments = url.EasySplit("/");
			Endpoint = segments[0];

			if(segments.Length <= 1) return;
			segments = segments[1].EasySplit("/");
			Id = segments[0];

			if(segments.Length <= 1) return;
			segments = segments[1].EasySplit("/");
			Option = segments[0];

			if(segments.Length <= 1) return;
			segments = segments[1].EasySplit("/");
			Moar = segments[0];
		}
	}
}