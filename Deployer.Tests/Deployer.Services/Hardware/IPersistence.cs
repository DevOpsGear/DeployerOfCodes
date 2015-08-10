namespace Deployer.Services.Hardware
{
    public interface IPersistence
    {
        bool DoesDirectoryExist(string directoryName);
        void CreateDirectory(string directoryPath);
        byte[] ReadFile(string filePath);
        void WriteFile(string filePath, byte[] data);
    }
}