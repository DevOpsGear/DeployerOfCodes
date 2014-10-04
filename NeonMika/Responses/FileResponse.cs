using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.SPOT;
using NeonMika.Requests;
using NeonMika.Util;

namespace NeonMika.Responses
{
	public class FileResponse : Response
	{
		public override bool CanRespond(Request e)
		{
			// Always returns true since it's the default reponder.
			return true;
		}

		public override bool SendResponse(Request e)
		{
			var filePath = UrlToPath(e.Url);

			if (!DoesFileExist(filePath))
				RequestHelper.Send404_NotFound(e.Client);

			var mimeType = RequestHelper.GetMimeType(filePath);

			using (var inputStream = new FileStream(filePath, FileMode.Open))
			{
				RequestHelper.Send200_OK(mimeType, (int) inputStream.Length, e.Client);

				// Send it in chunks so we don't exhaust memory
				var readBuffer = new byte[Settings.FILE_BUFFERSIZE];
				var sentBytes = 0;

				while (sentBytes < inputStream.Length)
				{
					var bytesRead = inputStream.Read(readBuffer, 0, readBuffer.Length);
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
					catch (Exception ex1)
					{
						Debug.Print("Error sending bytes - " + ex1);
						try
						{
							e.Client.Close();
						}
						catch (Exception ex2)
						{
							Debug.Print("Error closing socket - " + ex2);
						}

						return false;
					}
				}
			}

			return true;
		}

		private static string UrlToPath(string url)
		{
			return url.Replace('/', '\\');
		}

		private static bool DoesFileExist(string filePath)
		{
			try
			{
				return filePath != string.Empty && File.Exists(filePath);
			}
			catch (Exception ex)
			{
				Debug.Print("Error accessing file - " + ex);
				return false;
			}
		}
	}
}