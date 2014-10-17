using Deployer.Services.Hardware;
using System.IO;

namespace Deployer.Text.Hardware
{
    public class Persistence : IPersistence
    {
        private readonly string _rootDir;

        public Persistence(string rootDir)
        {
            _rootDir = rootDir;
        }

        public bool DoesRootDirectoryExist(string directoryPath)
        {
            var dir = Path.Combine(_rootDir, directoryPath);
            return Directory.Exists(dir);
        }
    }
}