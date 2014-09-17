using System;
using Deployer.Services.Hardware;

namespace Deployer.App.Micro
{
	public class TimeService : ITimeService
	{
		public DateTime Now()
		{
			return DateTime.Now;
		}
	}
}