using System;
using System.IO;
using Microsoft.SPOT;
using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Util;

namespace Deployer.App.WebResponders
{
	public class UpdateClientFilesResponder : Responder
	{
		private readonly string _rootDirectory;

		public UpdateClientFilesResponder(string rootDirectory)
		{
			_rootDirectory = rootDirectory;
		}

		public override bool CanRespond(Request e)
		{
			return e.HttpMethod == "PUT" && e.Url.StartsWith("client");
		}

		public override bool SendResponse(Request e)
		{
			try
			{
				var memory = Debug.GC(true);
				Debug.Print("Memory = " + memory);
				var partialPath = e.Url.Replace('/', '\\');
				var filePath = Path.Combine(_rootDirectory, partialPath);

				EstablishDirectory(filePath);

				var receivedBytes = 0;
				var fileHandle = new FileStream(filePath, FileMode.Create, FileAccess.Write);
				var buffer = new byte[512];
				while (true)
				{
					var countBytes = e.Body.ReadBytes(buffer);
					if (countBytes == 0)
						break;
					fileHandle.Write(buffer, 0, countBytes);
					receivedBytes += countBytes;
				}

				fileHandle.Close();
				RequestHelper.Send200_OK("text/plain", e.Client);
				Debug.Print("Received bytes = " + receivedBytes);
			}
			catch (Exception ex)
			{
				RequestHelper.Send500_Failure(e.Client, ex.ToString());
			}

			return true;
		}

		private static void EstablishDirectory(string filePath)
		{
			var dir = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}
	}
}