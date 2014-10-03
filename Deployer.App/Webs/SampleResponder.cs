using System.Net.Sockets;
using System.Text;
using NeonMika.Webserver;
using NeonMika.Webserver.Responses;

namespace Deployer.App.Webs
{
	public class SampleResponder : Response
	{
		public SampleResponder(string Name) : base(Name)
		{
		}


		public override bool SendResponse(Request e)
		{
			byte[] bytes = Encoding.UTF8.GetBytes("Yooo hoooo!");

			var byteCount = bytes.Length;
			Send200_OK("text/plain", byteCount, e.Client);
			e.Client.Send(bytes, byteCount, SocketFlags.None);
			return true;
		}

		public override bool ConditionsCheckAndDataFill(Request e)
		{
			return true;
		}
	}
}