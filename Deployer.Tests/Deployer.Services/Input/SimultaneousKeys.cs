using System;
using Deployer.Services.Hardware;

namespace Deployer.Services.Input
{
	public class SimultaneousKeys : ISimultaneousKeys
	{
		private readonly ITimeService _timeService;
		private readonly bool[] _keyState;
		private readonly DateTime[] _keyWhen;
		private const int ThresholdMilliseconds = 200;

		public SimultaneousKeys(bool stateA, bool stateB, ITimeService timeService)
		{
			_timeService = timeService;
			_keyState = new bool[2];
			_keyState[0] = stateA;
			_keyState[1] = stateB;

			_keyWhen = new DateTime[2];
			_keyWhen[0] = _timeService.Now();
			_keyWhen[1] = _timeService.Now();
		}

		public void KeyOn(KeySwitch whichKey)
		{
			_keyState[(int) whichKey] = true;
			_keyWhen[(int) whichKey] = _timeService.Now();
		}

		public void KeyOff(KeySwitch whichKey)
		{
			_keyState[(int) whichKey] = false;
			_keyWhen[(int) whichKey] = _timeService.Now();
		}

		public bool SwitchedSimultaneously
		{
			get
			{
				if (!AreBothOn)
					return false;
				var durationMs = GetDurationMilliseconds();
				return durationMs < ThresholdMilliseconds;
			}
		}

		public bool AreBothOn
		{
			get { return _keyState[0] && _keyState[1]; }
		}

		public bool AreBothOff
		{
			get { return !_keyState[0] && !_keyState[1]; }
		}

		public long GetDurationMilliseconds()
		{
				var timeDiff = _keyWhen[0] - _keyWhen[1];
				var ms = (long)( timeDiff.Ticks / 10000.0); //  TODO: TotalMilliseconds missing from NETMF. Differences in API
				if (ms < 0)
					ms = -ms;
			return ms;
		}
	}
}