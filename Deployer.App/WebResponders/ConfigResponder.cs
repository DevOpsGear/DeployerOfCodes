using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Util;

namespace Deployer.App.WebResponders
{
	public class ConfigResponder : Responder
	{
		public override bool CanRespond(Request e)
		{
			return e.Url.StartsWith("config");
		}

		public override bool SendResponse(Request e)
		{
			var text = "ConfigResponder!!!! http-method=" + e.HttpMethod;
			RequestHelper.SendTextUtf8("text/plain", text, e.Client);

			return true;
		}
	}
}