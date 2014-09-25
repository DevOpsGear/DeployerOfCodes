namespace Deployer.Services.Micro.Web
{
	public interface IWebRequestFactory
	{
		IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method);
	}
}