using System;
using Microsoft.SPOT;
using NeonMika.Interfaces;

namespace Deployer.App.Micro
{
    public class Logger : INeonLogger
    {
        public void Debug(string text)
        {
            Microsoft.SPOT.Debug.Print(text);
        }
    }
}
