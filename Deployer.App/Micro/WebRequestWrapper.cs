using System.IO;
using System.Net;
using Deployer.Services.Micro.Web;

namespace Deployer.App.Micro
{
	public class WebRequestWrapper : IWebRequest
	{
		private readonly HttpWebRequest _request;

		public WebRequestWrapper(HttpWebRequest request)
		{
			_request = request;
		}

		public string ContentType
		{
			get { return _request.ContentType; }
			set { _request.ContentType = value; }
		}

		public long ContentLength
		{
			get { return _request.ContentLength; }
			set { _request.ContentLength = value; }
		}

		public void AddHeader(string key, string value)
		{
			_request.Headers.Add(key, value);
		}

		public IHttpWebResponse GetResponse()
		{
			var resp = _request.GetResponse();
			return new HttpWebResponseWrapper(resp);
		}

		public Stream GetRequestStream()
		{
			return _request.GetRequestStream();
		}
	}
}