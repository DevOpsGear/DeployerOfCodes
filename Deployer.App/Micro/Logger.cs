using Deployer.Services.Micro;
using Microsoft.SPOT;
using NeonMika.Interfaces;

namespace Deployer.App.Micro
{
    public class Logger : INeonLogger, IDeployerLogger
    {
        void INeonLogger.Debug(string text)
        {
            Debug.Print(text);
        }

        void IDeployerLogger.Debug(string text)
        {
            Debug.Print(text);
        }
    }
}