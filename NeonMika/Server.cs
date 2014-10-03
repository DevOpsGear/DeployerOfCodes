using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NeonMika.Webserver.Responses;

namespace NeonMika.Webserver
{
	/// <summary>
	/// Main class of NeonMika.Webserver
	/// </summary>
	public class Server
	{
		public int Port { get; private set; }

		private Socket listeningSocket = null;
		private Hashtable responses = new Hashtable();


		/// <summary>
		/// Creates an NeonMika.Webserver instance running in a seperate thread
		/// </summary>
		/// <param name="portNumber">The port to listen for incoming requests</param>
		public Server(int port = 80)
		{
			Debug.Print("\n\n---------------------------");
			Debug.Print("THANKS FOR USING NeonMika.Webserver");
			Debug.Print("Version: " + Settings.SERVER_VERSION);
			Debug.Print("---------------------------");

			this.Port = port;

			SocketSetup();

			var webserverThread = new Thread(WaitingForRequest);
			webserverThread.Start();

			Debug.Print("\n\n---------------------------");
			Debug.Print("Webserver is now up and running");
		}

		/// <summary>
		/// Creates the socket that will listen for incoming requests
		/// </summary>
		private void SocketSetup()
		{
			listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listeningSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
			listeningSocket.Listen(5);
		}

		/// <summary>
		/// Waiting for client to connect.
		/// When bytes were read they get wrapped to a "Reqeust"
		/// </summary>
		private void WaitingForRequest()
		{
			while (true)
			{
				try
				{
					using (Socket clientSocket = listeningSocket.Accept())
					{
						//Wait to get the bytes in the sockets "available buffer"
						int availableBytes = AwaitAvailableBytes(clientSocket);

						if (availableBytes > 0)
						{
							byte[] buffer = new byte[availableBytes > Settings.MAX_REQUESTSIZE ? Settings.MAX_REQUESTSIZE : availableBytes];
							byte[] header = FilterHeader(clientSocket, buffer);

							//reqeust created, checking the response possibilities
							using (Request tempRequest = new Request(Encoding.UTF8.GetChars(header), clientSocket))
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
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message);
				}
			}
		}

		/// <summary>
		/// Reads in the data from the socket and seperates the header from the rest of the request.
		/// </summary>
		/// <param name="clientSocket"></param>
		/// <param name="buffer">Will get filled with the incoming data</param>
		/// <returns>The header</returns>
		private byte[] FilterHeader(Socket clientSocket, byte[] buffer)
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
			int availableBytes = 0;
			int newAvBytes;

			do
			{
				//Wait if bytes come in
				Thread.Sleep(15);
				newAvBytes = clientSocket.Available - availableBytes;

				// breaks the "always true loop" if no new bytes got available
				if (newAvBytes == 0)
					break;

				availableBytes += newAvBytes;
				newAvBytes = 0;
			} while (true); //repeat as long as new bytes were received

			return availableBytes;
		}

		/// <summary>
		/// Checks what Response has to be executed.
		/// It compares the requested page URL with the URL set for the coded responses 
		/// </summary>
		/// <param name="e"></param>
		private void SendResponse(Request e)
		{
			Response response = null;


			if (responses.Contains(e.URL))
				response = (Response) responses[e.URL];
			else
				response = (Response) responses["FileResponse"];


			if (response != null)
			{
				using (response)
				{
					if (response.ConditionsCheckAndDataFill(e))
					{
						if (!response.SendResponse(e))
							Debug.Print("Sending response failed");
					}
					else
					{
						response.Send404_NotFound(e.Client);
					}
				}
			}
		}

		//-------------------------------------------------------------
		//-------------------------------------------------------------
		//---------------Webserver expansion---------------------------
		//-------------------------------------------------------------
		//-------------------------------------------------------------
		//-------------------Basic methods-----------------------------

		/// <summary>
		/// Adds a Response
		/// </summary>
		/// <param name="response">XMLResponse that has to be added</param>
		public void AddResponse(Response response)
		{
			if (!responses.Contains(response.URL))
			{
				responses.Add(response.URL, response);
			}
		}

		/// <summary>
		/// Removes a Response
		/// </summary>
		/// <param name="ResponseName">XMLResponse that has to be deleted</param>
		public void RemoveResponse(String ResponseName)
		{
			if (responses.Contains(ResponseName))
			{
				responses.Remove(ResponseName);
			}
		}
	}
}