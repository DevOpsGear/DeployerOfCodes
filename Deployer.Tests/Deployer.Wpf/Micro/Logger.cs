﻿using Deployer.Services.Micro;
using NeonMika.Interfaces;
using System;

namespace Deployer.Wpf.Micro
{
    public class Logger : INeonLogger, IDeployerLogger
    {
        void INeonLogger.Debug(string text)
        {
            Console.WriteLine(text);
        }

        void IDeployerLogger.Debug(string text)
        {
            Console.WriteLine(text);
        }
    }
}