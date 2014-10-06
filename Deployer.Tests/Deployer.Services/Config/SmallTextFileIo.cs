using System.IO;
using System.Text;
using Deployer.Services.Config.Interfaces;

namespace Deployer.Services.Config
{
	public class SmallTextFileIo : ISmallTextFileIo
	{
		public string Read(string filePath)
		{
			var bytes = File.ReadAllBytes(filePath);
			var chars = Encoding.UTF8.GetChars(bytes);
			return new string(chars);
		}

		public void Write(string filePath, string content)
		{
			var bytes = Encoding.UTF8.GetBytes(content);
			File.WriteAllBytes(filePath, bytes);
		}
	}
}