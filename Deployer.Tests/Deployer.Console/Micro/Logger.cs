using NeonMika.Interfaces;
using System;

namespace Deployer.Text.Micro
{
    public class Logger : ILogger
    {
        public void Debug(string text)
        {
            Console.WriteLine("Debug: {0}", text);
        }
    }
}