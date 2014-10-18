using System;
using System.IO;
using Deployer.Services.Micro;
using NeonMika.Requests;
using NeonMika.Responses;

namespace Deployer.Services.WebResponders
{
    public class FilePutResponder : Responder
    {
        private readonly string _rootDirectory;
        private readonly string _folder;
        private readonly IDeployerLogger _logger;

        public FilePutResponder(string rootDirectory, string folder, IDeployerLogger logger)
        {
            _rootDirectory = rootDirectory;
            _folder = folder;
            _logger = logger;
        }

        public override bool CanRespond(Request e)
        {
            return e.HttpMethod == "PUT" && e.Url.StartsWith(_folder);
        }

        public override bool SendResponse(Request e)
        {
            try
            {
                var partialPath = e.Url.Replace('/', '\\');
                var filePath = Path.Combine(_rootDirectory, partialPath);

                EstablishDirectory(filePath);

                var receivedBytes = 0;
                var fileHandle = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                var buffer = new byte[256];
                while (true)
                {
                    var countBytes = e.Body.ReadBytes(buffer);
                    if (countBytes == 0)
                        break;
                    fileHandle.Write(buffer, 0, countBytes);
                    receivedBytes += countBytes;
                }

                fileHandle.Close();
                RequestHelper.Send200_OK(e.Client, "text/plain");
                _logger.Debug("Received bytes = " + receivedBytes);
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