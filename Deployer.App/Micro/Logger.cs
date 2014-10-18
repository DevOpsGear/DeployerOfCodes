using NeonMika.Interfaces;

namespace Deployer.App.Micro
{
    public class Logger : ILogger
    {
        public void Debug(string text)
        {
            Microsoft.SPOT.Debug.Print(text);
        }
    }
}