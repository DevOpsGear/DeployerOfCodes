using System.Text;
using Deployer.Services.Micro;
using System.Collections.Generic;
using System.IO;

namespace Deployer.Tests.SpiesFakes
{
	public class WebRequestSpy : IWebRequest
	{
		public string ContentType { get; set; }
		public long ContentLength { get; set; }

		public readonly string ApiRoot;
		public readonly string ApiEndpoint;
		public readonly string Method;
		public Dictionary<string, string> Headers { get; private set; }
		public WebResponseSpy Response { get; set; }
		public MemoryStream RequestStream { get; private set; }

		public WebRequestSpy(string apiRoot, string apiEndpoint, string method)
		{
			ApiRoot = apiRoot;
			ApiEndpoint = apiEndpoint;
			Method = method;
			Headers = new Dictionary<string, string>();

			Response = new WebResponseSpy();
			RequestStream = new MemoryStream();
		}

		public void AddHeader(string key, string value)
		{
			Headers.Add(key, value);
		}

		public IHttpWebResponse GetResponse()
		{
			return Response;
		}

		public Stream GetRequestStream()
		{
			return RequestStream;
		}
	}
}