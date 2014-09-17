using Deployer.Services.Hardware;
using Microsoft.SPOT.Hardware;

namespace Deployer.App.Hardware
{
	public class Led : ILed
	{
		private readonly OutputPort _outputPort;

		public Led()
		{
		}

		public Led(OutputPort outputPort)
		{
			_outputPort = outputPort;
		}

		public void Write(bool state)
		{
			if (_outputPort == null)
				return;
			_outputPort.Write(state);
		}
	}
}