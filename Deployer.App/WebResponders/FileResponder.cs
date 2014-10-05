using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.SPOT;
using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Util;

namespace Deployer.App.WebResponders
{
	public class FileResponder : Responder
	{
		private readonly string _rootDirectory;
		private readonly string _folder;
		private readonly int _bufferSize;

		public FileResponder(string rootDirectory, string folder, int bufferSize = 256)
		{
			_rootDirectory = rootDirectory;
			_folder = folder;
			_bufferSize = bufferSize;
		}

		public override bool CanRespond(Request e)
		{
			return e.HttpMethod == "GET" && e.Url.StartsWith(_folder);
		}

		public override bool SendResponse(Request e)
		{
			var filePath = Path.Combine(_rootDirectory, UrlToPath(e.Url));

			if (!DoesFileExist(filePath))
			{
				RequestHelper.Send404_NotFound(e.Client);
				return true;
			}

			var mimeType = RequestHelper.GetMimeType(filePath);
			using (var inputStream = new FileStream(filePath, FileMode.Open))
			{
				RequestHelper.Send200_OK(e.Client, mimeType, (int) inputStream.Length);

				// Send it in chunks to conserve RAM
				var sentBytes = 0;
				var readBuffer = new byte[_bufferSize];
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
					catch (Exception ex)
					{
						Debug.Print("FileResponder - error sending - " + ex);
						return false;
					}
				}
				Debug.Print("FileResponder - sent bytes - " + sentBytes);
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