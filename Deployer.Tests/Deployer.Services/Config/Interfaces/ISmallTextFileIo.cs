namespace Deployer.Services.Config.Interfaces
{
	public interface ISmallTextFileIo
	{
		string Read(string filePath);
		void Write(string filePath, string content);
	}
}