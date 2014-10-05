using System.Collections;
using System.Net.Sockets;
using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Util;

namespace Deployer.App.WebResponders
{
	public interface IApiService
	{
		bool SendResponse(ApiRequest request);
	}

	public interface IApiSocket
	{
		int Send(byte[] buffer, int size);
	}

	public class ApiSocketWrapper : IApiSocket
	{
		private readonly Socket _socket;

		public ApiSocketWrapper(Socket socket)
		{
			_socket = socket;
		}

		public int Send(byte[] buffer, int size)
		{
			return _socket.Send(buffer, size, SocketFlags.None);
		}
	}

	public interface IApiReadBody
	{
		int ReadBytes(byte[] buffer);
	}

	public class ApiReadBodyWrapper : IApiReadBody
	{
		private readonly ClientRequestBody _body;

		public ApiReadBodyWrapper(ClientRequestBody body)
		{
			_body = body;
		}

		public int ReadBytes(byte[] buffer)
		{
			return _body.ReadBytes(buffer);
		}
	}

	public class ApiRequest
	{
		public Hashtable Headers { get; set; }
		public Hashtable GetArguments { get; set; }
		public IApiReadBody Body { get; set; }
		public string HttpMethod { get; set; }
		public string Url { get; set; }
		public IApiSocket Client { get; set; }
	}

	public class DeployerServiceResponder : Responder
	{
		private readonly string _endpoint;
		private readonly IApiService _apiService;

		public DeployerServiceResponder(string endpoint, IApiService apiService)
		{
			_endpoint = endpoint;
			_apiService = apiService;
		}

		public override bool CanRespond(Request e)
		{
			return e.Url.StartsWith(_endpoint);
		}

		public override bool SendResponse(Request e)
		{
			var text = "DeployerServiceResponder!!!! http-method=" + e.HttpMethod;
			RequestHelper.SendTextUtf8("text/plain", text, e.Client);

			var request = new ApiRequest
				{
					Client = new ApiSocketWrapper(e.Client),
					HttpMethod = e.HttpMethod,
					Url = e.Url,
					Headers = e.Headers,
					GetArguments = e.GetArguments,
					Body = new ApiReadBodyWrapper(e.Body)
				};

			return _apiService.SendResponse(request);
		}
	}
}