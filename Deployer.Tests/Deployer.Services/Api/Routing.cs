using Deployer.Services.Micro;
using Deployer.Services.Micro.Web;
using Deployer.Services.Micro.Wrappers;

namespace Deployer.Services.Api
{
	// http://mftoolkit.codeplex.com/SourceControl/latest ?
	public class Routing
	{
		private readonly IWebResponderFactory _responderFactory;

		public Routing(IWebResponderFactory responderFactory)
		{
			_responderFactory = responderFactory;
		}

		public void HandleRequest(string path, HttpMethod method, IResponderWrapper ghir)
		{
			var responder = _responderFactory.CreateResponder(ghir);
			var parts = path.Split(new[] {'/'});
			if (parts.Length > 0)
			{
				var root = parts[0];
				switch (root)
				{
					case "":
						// Root: Return HTML
						break;

					case "client":
						// Return files (JS, CSS, etc.)
						break;

					case "api":
						// REST/JSON API
						break;
				}
			}

			responder.Send404();
		}
	}
}