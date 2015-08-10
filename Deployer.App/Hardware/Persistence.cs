using Deployer.Services.Hardware;
using Gadgeteer;

namespace Deployer.App.Hardware
{
    public class Persistence : IPersistence
    {
        private readonly StorageDevice _storageDevice;

        public Persistence(StorageDevice storageDevice)
        {
            _storageDevice = storageDevice;
        }

        public bool DoesDirectoryExist(string directoryName)
        {
            var dirs = _storageDevice.ListRootDirectorySubdirectories();
            foreach (var dir in dirs)
            {
                if (dir == directoryName)
                    return true;
            }
            return false;
        }

        public void CreateDirectory(string directoryPath)
        {
            _storageDevice.CreateDirectory(directoryPath);
        }

        public byte[] ReadFile(string filePath)
        {
            return _storageDevice.ReadFile(filePath);
        }

        public void WriteFile(string filePath, byte[] data)
        {
            _storageDevice.WriteFile(filePath, data);
        }

        /* public void DeleteDirectory(string directoryPath)
        {
            _storageDevice.DeleteDirectory(directoryPath, true);
        }

        public bool DoesFileExist(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            var files = _storageDevice.ListFiles(dirPath);
            foreach (var file in files)
            {
                if (file == filePath)
                    return true;
            }
            return false;
        } */
    }
}