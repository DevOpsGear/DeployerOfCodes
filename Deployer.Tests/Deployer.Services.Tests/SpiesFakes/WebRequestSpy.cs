using Deployer.Services.Micro.Web;
using System.Collections.Generic;
using System.IO;

namespace Deployer.Tests.SpiesFakes
{
	public class WebRequestSpy : IWebRequest
	{
		public string ContentType { get; set; }
	    public string Accept { get; set; }
	    public long ContentLength { get; set; }

		public string ApiRoot { get; set; }
		public string ApiEndpoint { get; set; }
		public string Method { get; set; }
		public WebResponseSpy SpyResponse { get; set; }
		public MemoryStream SpyRequestStream { get; private set; }
		public Dictionary<string, string> SpyHeaders { get; private set; }

		public WebRequestSpy()
		{
			SpyHeaders = new Dictionary<string, string>();

			SpyResponse = new WebResponseSpy();
			SpyRequestStream = new MemoryStream();
		}

		public void AddHeader(string key, string value)
		{
			SpyHeaders.Add(key, value);
		}

		public IHttpWebResponse GetResponse()
		{
			return SpyResponse;
		}

		public Stream GetRequestStream()
		{
			return SpyRequestStream;
		}
	}
}