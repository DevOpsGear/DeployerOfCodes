using Deployer.Services.Hardware;
using System;

namespace Deployer.Tests.SpiesFakes
{
	public class TimeServiceFake : ITimeService
	{
		private DateTime _value;

		public TimeServiceFake(DateTime initialValue)
		{
			_value = initialValue;
		}

		public void AddSeconds(int sec)
		{
			_value = _value.AddSeconds(sec);
		}

		public void AddMilliseconds(int ms)
		{
			_value = _value.AddMilliseconds(ms);
		}

		public DateTime Now()
		{
			return _value;
		}
	}
}