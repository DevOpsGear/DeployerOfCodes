namespace Deployer.Services.Micro
{
	public interface IWebRequestFactory
	{
		IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method);
	}
}