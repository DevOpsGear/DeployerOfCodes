using System.IO;
using System.Text;
using Deployer.Services.Config.Interfaces;

namespace Deployer.Services.Config
{
	public class SmallTextFileIo : ISmallTextFileIo
	{
		public string Read(string filePath)
		{
			try
			{
				var bytes = File.ReadAllBytes(filePath);
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
			File.WriteAllBytes(filePath, bytes);
		}
	}
}