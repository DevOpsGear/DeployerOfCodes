using Deployer.Services.Hardware;
using Gadgeteer.SocketInterfaces;

namespace Deployer.App.Hardware
{
	class LedDigital : ILed
	{
		private readonly DigitalOutput _outputPort;

		public LedDigital()
		{
		}

		public LedDigital(DigitalOutput outputPort)
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
