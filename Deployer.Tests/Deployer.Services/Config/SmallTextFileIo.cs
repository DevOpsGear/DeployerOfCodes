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

			// Write to a temporary file first
			var tempPath = GetTempPath(filePath);
			File.WriteAllBytes(tempPath, bytes);

			// If it succeeds, delete the old one
			File.Delete(filePath);

			// ... and rename the temp file
			File.Move(tempPath, filePath);
		}

		private static string GetTempPath(string filePath)
		{
			var tempPath = filePath + ".temp";
			if(File.Exists(tempPath))
				File.Delete(tempPath);
			return tempPath;
		}
	}
}