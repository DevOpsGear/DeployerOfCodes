using System.IO;
using System.Reflection;

namespace ProjectSync
{
    internal class Program
    {
        private static void Main()
        {
            var startingDir = Assembly.GetExecutingAssembly().Location;
            var rootDir = GetRootProjectDirectory(startingDir);
            var ctrl = new ProjectController(rootDir, @"Deployer.Tests");

            ctrl.Sync(@"Deployer.Services\Deployer.Services.csproj", @"..\Deployer.Tests\Deployer.Services\");
            ctrl.Sync(@"NeonMika\NeonMika.csproj", @"..\Deployer.Tests\NeonMika\");
        }

        private static string GetRootProjectDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(dir)) return string.Empty;
            var currentDir = new DirectoryInfo(dir);
            while (true)
            {
                var testsDir = Path.Combine(currentDir.FullName, "Deployer.Tests");
                if (Directory.Exists(testsDir))
                    return currentDir.FullName;
                currentDir = currentDir.Parent;
                if (currentDir == null) return string.Empty;
            }
        }
    }
}