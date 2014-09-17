using System;

namespace Deployer.Services.Input
{
	public class SimultaneousKeys : ISimultaneousKeys
	{
		private readonly bool[] _keyState;
		private readonly DateTime[] _keyWhen;
		private const int ThresholdMilliseconds = 200;

		public SimultaneousKeys(bool stateA, bool stateB)
		{
			_keyState = new bool[2];
			_keyState[0] = stateA;
			_keyState[1] = stateB;

			_keyWhen = new DateTime[2];
			_keyWhen[0] = DateTime.Now;
			_keyWhen[1] = DateTime.Now;
		}

		public void KeyOn(KeySwitch whichKey)
		{
			_keyState[(int) whichKey] = true;
			_keyWhen[(int) whichKey] = DateTime.Now;
		}

		public void KeyOff(KeySwitch whichKey)
		{
			_keyState[(int) whichKey] = false;
			_keyWhen[(int) whichKey] = DateTime.Now;
		}

		public bool SwitchedSimultaneously
		{
			get
			{
				if (!AreBothOn)
					return false;
				var timeDiff = _keyWhen[0] - _keyWhen[1];
				var ms = timeDiff.Milliseconds;
				if (ms < 0)
					ms = -ms;
				return ms < ThresholdMilliseconds;
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
	}
}