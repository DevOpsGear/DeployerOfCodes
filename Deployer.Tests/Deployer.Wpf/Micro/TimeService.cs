using Deployer.Services.Hardware;
using System;

namespace Deployer.Wpf.Micro
{
    public class TimeService : ITimeService
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
