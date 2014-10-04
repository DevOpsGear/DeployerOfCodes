using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using NeonMika.Responses;

namespace NeonMika
{
	public class WebServer
	{
		public int Port { get; private set; }

		private Socket _listeningSocket;
		private readonly ArrayList _responses;
		private readonly Response _defaultResponse;

		public WebServer(int port = 80)
		{
			Debug.Print("\n\n---------------------------");
			Debug.Print("THANKS FOR USING NeonMika.Webserver");
			Debug.Print("Version: " + Settings.SERVER_VERSION);
			Debug.Print("---------------------------");

			Port = port;
			_responses = new ArrayList();
			_defaultResponse = new FileResponse();

			SetupListeningSocket();

			var webserverThread = new Thread(WaitForNetworkRequest);
			webserverThread.Start();

			Debug.Print("\n\n---------------------------");
			Debug.Print("Webserver is now up and running");
		}

		public void AddResponse(Response response)
		{
			_responses.Add(response);
		}

		private void SetupListeningSocket()
		{
			_listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_listeningSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
			_listeningSocket.Listen(5);
		}

		private void WaitForNetworkRequest()
		{
			while (true)
			{
				try
				{
					using (var clientSocket = _listeningSocket.Accept())
					{
						var availableBytes = AwaitAvailableBytes(clientSocket);
						if (availableBytes <= 0) continue;

						var buffer = new byte[availableBytes > Settings.MAX_REQUESTSIZE ? Settings.MAX_REQUESTSIZE : availableBytes];
						var headerBody = ReadAndBreakApart(clientSocket, buffer);

						using (
							var request = new Request(Encoding.UTF8.GetChars(headerBody.Header), Encoding.UTF8.GetChars(headerBody.Body),
							                          clientSocket))
						{
							Debug.Print("\n\nClient connected\nURL: " + request.Url + "\nFinal byte count: " + availableBytes + "\n");
							SendResponse(request);
						}

						try
						{
							clientSocket.Close();
						}
						catch (Exception ex)
						{
							Debug.Print(ex.ToString());
						}

						Debug.Print("Reqeust finished");
					}
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message);
				}
			}
// ReSharper disable FunctionNeverReturns
		}

// ReSharper restore FunctionNeverReturns

		private static int AwaitAvailableBytes(Socket clientSocket)
		{
			var availableBytes = 0;
			do
			{
				Thread.Sleep(15);
				var newAvBytes = clientSocket.Available - availableBytes;
				if (newAvBytes == 0)
					break;

				availableBytes += newAvBytes;
			} while (true); // Repeat until all bytes received

			return availableBytes;
		}

		private static HeaderBody ReadAndBreakApart(Socket clientSocket, byte[] buffer)
		{
			clientSocket.Receive(buffer, buffer.Length, SocketFlags.None);
			return new HeaderBody(buffer);
		}

		private void SendResponse(Request e)
		{
			foreach (Response resp in _responses)
			{
				if (!resp.CanRespond(e))
					continue;
				if (!resp.SendResponse(e))
					Debug.Print("Sending response failed");
				return;
			}

			_defaultResponse.SendResponse(e);
		}
	}
}