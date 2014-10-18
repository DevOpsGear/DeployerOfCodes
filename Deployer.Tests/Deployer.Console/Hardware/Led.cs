using System;
using Deployer.Services.Hardware;

namespace Deployer.Text.Hardware
{
    public class Led : ILed
    {
        private readonly string _nameIndicator;
        private bool _currentState;

        public Led(string nameIndicator)
        {
            _nameIndicator = nameIndicator;
        }

        public void Write(bool state)
        {
            if (_currentState == state) return;
            var msg = state ? "ON" : "off";
            Console.WriteLine("Indicator {0} => {1}", _nameIndicator, msg);
            _currentState = state;
        }
    }
}