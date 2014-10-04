using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;
using NeonMika.Requests;
using NeonMika.Util;

namespace NeonMika.Responses
{
	/// <summary>
	/// Standard response sending file to client
	/// If filename is a directory, a directory-overview will be displayed
	/// </summary>
	public class FileResponse : Response
	{
		/// <summary>
		/// File response has no conditions
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public override bool CanRespond(Request e)
		{
			string filePath = e.Url;
			bool isDirectory;
			return CheckFileDirectoryExist(ref filePath, out isDirectory);
		}

		/// <summary>
		/// Depending on the requested path, a file, a directory-overview or a 404-error will be returned
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public override bool SendResponse(Request e)
		{
			var filePath = e.Url.Replace('/', '\\');
			bool isDirectory;

			//File found check
			if (!CheckFileDirectoryExist(ref filePath, out isDirectory))
				RequestHelper.Send404_NotFound(e.Client);

			if (isDirectory)
			{
				var interf = NetworkInterface.GetAllNetworkInterfaces()[0];

				RequestHelper.Send200_OK(RequestHelper.GetMimeType(".html"), 0, e.Client);

				var uppath = ((filePath.LastIndexOf("\\") >= 0)
					                 ? interf.IPAddress + ((filePath[0] != '\\') ? "\\" : "") +
					                   filePath.Substring(0, filePath.LastIndexOf("\\"))
					                 : interf.IPAddress + ((filePath[0] != '\\') ? "\\" : "") + filePath);

				var send = "<html><head><title>" + e.Url + "</title>" +
				              "<style type=\"text/css\">a.a1{background-color:#ADD8E6;margin:0;padding:0;font-weight:bold;}a.a2{background-color:#87CEEB;margin:0;padding:0;font-weight:bold;}</style>" +
				              "</head><body><a href=\"http:\\\\" + uppath + "\">One level up</a><br/>" +
				              "<h1>" + e.Url + "</h1><h2>Directories:</h2>";
				if (RequestHelper.SendData(e.Client, Encoding.UTF8.GetBytes(send)) == 0)
					return false;

				foreach (var d in Directory.GetDirectories(filePath))
				{
					send = "<a href=\"http:\\\\" + interf.IPAddress + d + "\" class=\"a1\">" + d + "</a><br/>";
					if (RequestHelper.SendData(e.Client, Encoding.UTF8.GetBytes(send)) == 0)
						return false;
				}

				RequestHelper.SendData(e.Client, Encoding.UTF8.GetBytes("<h2>Files:</h2>"));

				foreach (var f in Directory.GetFiles(filePath))
				{
					send = "<a href=\"http:\\\\" + interf.IPAddress + f + "\" class=\"a2\">" + f + "</a><br/>";
					if (RequestHelper.SendData(e.Client, Encoding.UTF8.GetBytes(send)) == 0)
						return false;
				}

				send = "</body></html>";
				if (RequestHelper.SendData(e.Client, Encoding.UTF8.GetBytes(send)) == 0)
					return false;
			}
			else
			{
				var mimeType = RequestHelper.GetMimeType(filePath);

				using (var inputStream = new FileStream(filePath, FileMode.Open))
				{
					RequestHelper.Send200_OK(mimeType, (int) inputStream.Length, e.Client);

					byte[] readBuffer = new byte[Settings.FILE_BUFFERSIZE];
					int sentBytes = 0;

					//Sending parts in size of "Settings.FILE_BUFFERSIZE"
					while (sentBytes < inputStream.Length)
					{
						int bytesRead = inputStream.Read(readBuffer, 0, readBuffer.Length);
						try
						{
							if (RequestHelper.IsSocketConnected(e.Client))
							{
								sentBytes += e.Client.Send(readBuffer, bytesRead, SocketFlags.None);
							}
							else
							{
								e.Client.Close();
								return false;
							}
						}
						catch (Exception ex)
						{
							Debug.Print("Error at sending bytes");
							try
							{
								e.Client.Close();
							}
							catch (Exception ex2)
							{
								Debug.Print("Error at closing socket");
							}

							return false;
						}
					}
				}
			}

			return true;
		}

		private static bool CheckFileDirectoryExist(ref string filePath, out bool isDirectory)
		{
			isDirectory = false;

			filePath = filePath.Replace('/', '\\');

			//File found check
			try
			{
				if (filePath == "")
					return false;

				if (!File.Exists(filePath))
					if (!Directory.Exists(filePath))
					{
						return false;
					}
					else
						isDirectory = true;
				return true;
			}
			catch (Exception ex)
			{
				Debug.Print("Error accessing file/directory");
				Debug.Print(ex.ToString());
				return false;
			}
		}
	}
}