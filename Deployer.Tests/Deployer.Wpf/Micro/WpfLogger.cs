using NeonMika.Interfaces;
using System;

namespace Deployer.Wpf.Micro
{
    public class WpfLogger : ILogger
    {
        public void Debug(string text)
        {
            Console.WriteLine(text);
        }
    }
}