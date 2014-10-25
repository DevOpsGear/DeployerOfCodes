using NeonMika.Interfaces;
using NeonMika.Requests;
using NeonMika.Responses;
using System;
using System.IO;
using System.Net.Sockets;

namespace Deployer.Services.WebResponders
{
	public class FileGetResponder : Responder
	{
		private readonly string _rootDirectory;
		private readonly string _folder;
        private readonly ILogger _logger;
	    private readonly int _bufferSize;

        public FileGetResponder(string rootDirectory, string folder, ILogger logger, int bufferSize = 256)
		{
			_rootDirectory = rootDirectory;
			_folder = folder;
		    _logger = logger;
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
						_logger.Debug("FileResponder - error sending - " + ex);
						return false;
					}
				}
				_logger.Debug("FileResponder - sent bytes - " + sentBytes);
			}

			return true;
		}

		private static string UrlToPath(string url)
		{
			return url.Replace('/', '\\');
		}

		private bool DoesFileExist(string filePath)
		{
			try
			{
				return filePath != string.Empty && File.Exists(filePath);
			}
			catch (Exception ex)
			{
				_logger.Debug("Error accessing file - " + ex);
				return false;
			}
		}
	}
}