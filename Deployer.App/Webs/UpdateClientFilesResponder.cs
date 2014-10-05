using System;
using System.IO;
using System.Text;
using Gadgeteer;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Util;

namespace Deployer.App.Webs
{
	public class UpdateClientFilesResponder : Response
	{
		private readonly string _rootDirectory;

		public UpdateClientFilesResponder(VolumeInfo vi)
		{
			_rootDirectory = vi.RootDirectory;
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
				//var fileHandle = _sd.Open(filePath, FileMode.Create, FileAccess.Write);
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

		private void EstablishDirectory(string filePath)
		{
			var dir = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir); // Directory.CreateDirectory(dir);
		}
	}
}