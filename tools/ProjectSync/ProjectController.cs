using System.IO;

namespace ProjectSync
{
    public class ProjectController
    {
        private readonly string _sourceDir;
        private readonly string _targetDir;
        private readonly ProjectSynchronizer _synch;

        public ProjectController(string root, string sourceFolder)
        {
            _sourceDir = Path.Combine(root, sourceFolder);
            _targetDir = root;
            _synch = new ProjectSynchronizer();
        }

        public void Sync(string projectFileName, string relative)
        {
            var sourcePath = Path.Combine(_sourceDir, projectFileName);
            var targetPath = Path.Combine(_targetDir, projectFileName);
            _synch.Sync(sourcePath, targetPath, relative);
        }
    }
}