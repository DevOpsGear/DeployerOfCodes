using System.Collections;

namespace Deployer.Services.Config.Interfaces
{
	public interface IJsonPersistence
	{
		Hashtable Read(string filePath);
		void Write(string filePath, Hashtable content);
	}
}