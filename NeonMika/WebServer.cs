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
	/// <summary>
	/// Main class of NeonMika.Webserver
	/// </summary>
	public class WebServer
	{
		public int Port { get; private set; }

		private Socket _listeningSocket;
		private readonly ArrayList _responses;
		private Response _defaultResponse;

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
						//Wait to get the bytes in the sockets "available buffer"
						var availableBytes = AwaitAvailableBytes(clientSocket);
						if (availableBytes <= 0) continue;

						var buffer = new byte[availableBytes > Settings.MAX_REQUESTSIZE ? Settings.MAX_REQUESTSIZE : availableBytes];
						var header = ReadOnlyHeader(clientSocket, buffer);

						//reqeust created, checking the response possibilities
						using (var tempRequest = new Request(Encoding.UTF8.GetChars(header), clientSocket))
						{
							Debug.Print("\n\nClient connected\nURL: " + tempRequest.URL + "\nFinal byte count: " + availableBytes + "\n");

							//Let's check if we have to take some action or if it is a file-response 
							SendResponse(tempRequest);
						}

						try
						{
							//Close client, otherwise the browser / client won't work properly
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

		/// <summary>
		/// Reads in the data from the socket and seperates the header from the rest of the request.
		/// </summary>
		/// <param name="clientSocket"></param>
		/// <param name="buffer">Will get filled with the incoming data</param>
		/// <returns>The header</returns>
		private byte[] ReadOnlyHeader(Socket clientSocket, byte[] buffer)
		{
			byte[] header = new byte[0];
			int readByteCount = clientSocket.Receive(buffer, buffer.Length, SocketFlags.None);

			for (int headerend = 0; headerend < buffer.Length - 3; headerend++)
			{
				if (buffer[headerend] == '\r' && buffer[headerend + 1] == '\n' && buffer[headerend + 2] == '\r' &&
				    buffer[headerend + 3] == '\n')
				{
					header = new byte[headerend + 4];
					Array.Copy(buffer, 0, header, 0, headerend + 4);
					break;
				}
			}

			return header;
		}

		/// <summary>
		/// Returns the number of available bytes.
		/// Waits till all bytes from one request are received.
		/// </summary>
		/// <param name="clientSocket"></param>
		/// <returns></returns>
		private int AwaitAvailableBytes(Socket clientSocket)
		{
			var availableBytes = 0;
			do
			{
				Thread.Sleep(15);
				var newAvBytes = clientSocket.Available - availableBytes;
				if (newAvBytes == 0)
					break;

				availableBytes += newAvBytes;
			} while (true); //repeat as long as new bytes were received

			return availableBytes;
		}

		private void SendResponse(Request e)
		{
			foreach (Response resp in _responses)
			{
				if (!resp.ConditionsCheckAndDataFill(e)) continue;
				if (!resp.SendResponse(e))
					Debug.Print("Sending response failed");
				return;
			}

			_defaultResponse.SendResponse(e);
		}
	}
}