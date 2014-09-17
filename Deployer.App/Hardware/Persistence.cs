using System.IO;
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

		public bool DoesRootDirectoryExist(string directoryName)
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

		public void DeleteDirectory(string directoryPath)
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
		}

		public string ReadText(string filePath)
		{
			var str = _storageDevice.OpenRead(filePath);
			var rdr = new StreamReader(str);
			return rdr.ReadToEnd();
		}
	}
}