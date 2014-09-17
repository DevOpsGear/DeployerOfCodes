using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectSync
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var startingDir = Assembly.GetExecutingAssembly().Location;
			var rootDir = GetRootProjectDirectory(startingDir);

			var pathSource = Path.Combine(rootDir, @"Deployer.Tests\Deployer.Services\Deployer.Services.csproj");
			var pathTarget = Path.Combine(rootDir, @"Deployer.Services\Deployer.Services.csproj");

			var th = new Synchronizer();
			th.DoStuff(pathSource, pathTarget);
		}

		private static string GetRootProjectDirectory(string path)
		{
			var dir = Path.GetDirectoryName(path);
			var currentDir = new DirectoryInfo(dir);
			while (true)
			{
				var testsDir = Path.Combine(currentDir.FullName, "Deployer.Tests");
				if (Directory.Exists(testsDir))
					return currentDir.FullName;
				currentDir = currentDir.Parent;
			}
		}
	}
}