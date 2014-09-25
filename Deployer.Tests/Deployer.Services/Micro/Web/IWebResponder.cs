namespace Deployer.Services.Micro.Web
{
	public interface IWebResponder
	{
		void SendJson(object obj);
		void Send404();
	}
}