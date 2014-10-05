using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Util;

namespace Deployer.App.Webs
{
	public class AuthResponder : Response
	{
		public override bool CanRespond(Request e)
		{
			return e.Url.StartsWith("auth");
		}

		public override bool SendResponse(Request e)
		{
			var text = "AuthResponder!!!! http-method=" + e.HttpMethod;
			RequestHelper.SendTextUtf8("text/plain", text, e.Client);

			return true;
		}
	}
}