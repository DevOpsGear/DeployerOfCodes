using Deployer.Services.Micro.Wrappers;

namespace Deployer.Services.Micro.Web
{
	public class WebResponderFactory : IWebResponderFactory
	{
		public IWebResponder CreateResponder(IResponderWrapper ghir)
		{
			return new WebResponder(ghir);
		}
	}
}