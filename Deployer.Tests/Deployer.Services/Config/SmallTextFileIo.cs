using System.Text;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Hardware;

namespace Deployer.Services.Config
{
    public class SmallTextFileIo : ISmallTextFileIo
    {
        private readonly IPersistence _persistence;

        public SmallTextFileIo(IPersistence persistence)
        {
            _persistence = persistence;
        }

        public string Read(string filePath)
        {
            try
            {
                var bytes = _persistence.ReadFile(filePath); // File.ReadAllBytes(filePath);
                var chars = Encoding.UTF8.GetChars(bytes);
                return new string(chars);
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Write(string filePath, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            _persistence.WriteFile(filePath, bytes);

            // Write to a temporary file first
            //var tempPath = GetTempPath(filePath);
            //File.WriteAllBytes(tempPath, bytes);

            // If it succeeds, delete the old one
            //File.Delete(filePath);

            // ... and rename the temp file
            //File.Move(tempPath, filePath);
        }

        /* private static string GetTempPath(string filePath)
        {
            var tempPath = filePath + ".temp";
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            return tempPath;
        } */
    }
}