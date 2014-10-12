using System.IO;

namespace Deployer.Services.Micro.Web
{
	public interface IWebRequest
	{
		long ContentLength { get; set; }
		string ContentType { get; set; }
		string Accept { get; set; }
		void AddHeader(string key, string value);
		IHttpWebResponse GetResponse();
		Stream GetRequestStream();
	}
}