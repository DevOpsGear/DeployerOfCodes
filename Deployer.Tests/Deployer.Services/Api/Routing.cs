using Deployer.Services.Micro.Web;

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

		public void HandleRequest(string path, string method )
		{
			var responder = _responderFactory.CreateResponder();
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