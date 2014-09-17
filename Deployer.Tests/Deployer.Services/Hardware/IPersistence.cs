using System.IO;

namespace Deployer.Services.Hardware
{
	public interface IPersistence
	{
		bool DoesRootDirectoryExist(string directoryPath);
		//void CreateDirectory(string directoryPath);
		//void Delete(string filePath);
		//string[] ListRootDirectorySubdirectories();
		//FileStream OpenRead(string filePath);
		//FileStream OpenWrite(string filePath);
	}
}