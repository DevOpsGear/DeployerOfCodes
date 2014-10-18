using Deployer.Services.Micro;
using NeonMika.Interfaces;
using System;

namespace Deployer.Text.Micro
{
    public class Logger : INeonLogger, IDeployerLogger
    {
        void INeonLogger.Debug(string text)
        {
            Console.WriteLine("Debug: {0}", text);
        }

        void IDeployerLogger.Debug(string text)
        {
            Console.WriteLine("Debug: {0}", text);
        }
    }
}