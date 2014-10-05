using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
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
			var filePath = @"\SD\" + UrlToPath(e.Url);

			if (!DoesFileExist(filePath))
			{
				RequestHelper.Send404_NotFound(e.Client);
				return true;
			}

			var mimeType = RequestHelper.GetMimeType(filePath);

			//Debug.GC(true);
			using (var inputStream = new FileStream(filePath, FileMode.Open))
			{
				RequestHelper.Send200_OK(mimeType, (int) inputStream.Length, e.Client);

				// Send it in chunks so we don't exhaust memory
				var readBuffer = new byte[256];
				var sentBytes = 0;

				while (sentBytes < inputStream.Length)
				{
					try
					{
						var bytesRead = inputStream.Read(readBuffer, 0, readBuffer.Length);
						if (bytesRead <= 0)
							break;
						var now = e.Client.Send(readBuffer, bytesRead, SocketFlags.None);
						sentBytes += now;
					}
					catch (Exception ex1)
					{
						Debug.Print("Error sending bytes - " + ex1);
						return false;
					}
				}
				Debug.Print("Sent bytes - " + sentBytes);
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