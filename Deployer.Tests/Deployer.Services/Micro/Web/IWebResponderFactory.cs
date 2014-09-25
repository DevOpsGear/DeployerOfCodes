using Deployer.Services.Micro.Wrappers;

namespace Deployer.Services.Micro.Web
{
	public interface IWebResponderFactory
	{
		IWebResponder CreateResponder(IResponderWrapper ghir);
	}
}