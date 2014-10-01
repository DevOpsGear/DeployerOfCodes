
namespace Deployer.Services.Micro.Web
{
	public class WebResponderFactory : IWebResponderFactory
	{
		public IWebResponder CreateResponder()
		{
			return new WebResponder();
		}
	}
}