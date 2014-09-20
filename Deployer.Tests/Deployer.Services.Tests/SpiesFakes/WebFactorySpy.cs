using Deployer.Services.Micro;

namespace Deployer.Tests.SpiesFakes
{
	public class WebFactorySpy : IWebRequestFactory
	{
		public WebRequestSpy LastWebRequest;

		public IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method)
		{
			LastWebRequest = new WebRequestSpy(apiRoot, apiEndpoint, method);
			return LastWebRequest;
		}
	}
}