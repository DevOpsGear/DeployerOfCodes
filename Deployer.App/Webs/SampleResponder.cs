using NeonMika.Requests;
using NeonMika.Responses;

namespace Deployer.App.Webs
{
	public class SampleResponder : Response
	{
		public override bool CanRespond(Request e)
		{
			return true;
		}

		public override bool SendResponse(Request e)
		{
			var text = "Yoo ho!!!! method=" + e.Method + " / body=" + e.Body;
			RequestHelper.SendTextUtf8("text/plain", text, e.Client);
			return true;
		}
	}
}