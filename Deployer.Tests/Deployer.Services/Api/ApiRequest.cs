using System.Collections;
using Deployer.Services.Api.Interfaces;

namespace Deployer.Services.Api
{
	public class ApiRequest
	{
		public Hashtable Headers { get; set; }
		public Hashtable GetArguments { get; set; }
		public IApiReadBody Body { get; set; }
		public string HttpMethod { get; set; }
		public string Url { get; set; }
		public IApiSocket Client { get; set; }
	}
}