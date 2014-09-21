using Deployer.Services.Micro;

namespace Deployer.Tests.SpiesFakes
{
	public class WebFactorySpy : IWebRequestFactory
	{
		public WebRequestSpy SpyWebRequest { get; set; }

		public WebFactorySpy()
		{
			SpyWebRequest = new WebRequestSpy();
		}

		public IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method)
		{
			SpyWebRequest.ApiRoot = apiRoot;
			SpyWebRequest.ApiEndpoint = apiEndpoint;
			SpyWebRequest.Method = method;
			return SpyWebRequest;
		}
	}
}