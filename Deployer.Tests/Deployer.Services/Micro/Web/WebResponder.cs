using System.Collections;
using System.IO;
using Json.NETMF;
using System.Text;

namespace Deployer.Services.Micro.Web
{
	public class WebResponder : IWebResponder
	{
		public void SendJson(object obj)
		{
			var data = JsonSerializer.SerializeObject(obj);
			var encoded = Encoding.UTF8.GetBytes(data);
			//_responderWrapper.Respond(encoded, "application/json");
		}

		public void SendFile(Stream content, string filename)
		{
			// What the heck - no way to stream?
			// Just byte arrays?
		}

		public void Send404()
		{
			var nothingFound = new Hashtable
				{
					{"error", "Endpoint does not exist"}
				};
			SendJson(nothingFound);
		}
	}
}