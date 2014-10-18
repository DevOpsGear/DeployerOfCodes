using Deployer.Services.Hardware;
using System;

namespace Deployer.Services.Micro
{
    public class TimeService : ITimeService
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}