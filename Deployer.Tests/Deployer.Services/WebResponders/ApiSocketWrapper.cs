using System.Net.Sockets;
using Deployer.Services.Api.Interfaces;
using NeonMika.Requests;

namespace Deployer.App.WebResponders
{
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

		public void Send200_OK(string mimeType)
		{
			RequestHelper.Send200_OK(_socket, mimeType);
		}

		public void Send400_BadRequest()
		{
			RequestHelper.Send400_BadRequest(_socket);
		}

		public void Send200_OK(string mimeType, int contentLength)
		{
			RequestHelper.Send200_OK(_socket, mimeType, contentLength);
		}

		public void Send404_NotFound()
		{
			RequestHelper.Send404_NotFound(_socket);
		}

		public void Send405_MethodNotAllowed()
		{
			RequestHelper.Send405_MethodNotAllowed(_socket);
		}

		public void Send500_Failure(string message = "")
		{
			RequestHelper.Send500_Failure(_socket, message);
		}
	}
}