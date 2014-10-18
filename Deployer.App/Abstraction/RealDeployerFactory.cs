using System;
using System.Threading;
using Deployer.App.Hardware;
using Deployer.App.Micro;
using Deployer.Services.Abstraction;
using Deployer.Services.Hardware;
using GHIElectronics.Gadgeteer;
using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NeonMika.Interfaces;

namespace Deployer.App.Abstraction
{
    public class RealDeployerFactory : CommonFactory
    {
        private readonly FEZCerbuinoNet _mainboard;
        private readonly BreakoutTB10 _breakout;
        private readonly CharacterDisplay _characterDisplay;
        private readonly Tunes _tunes;

        public RealDeployerFactory( FEZCerbuinoNet mainboard, BreakoutTB10 breakout, CharacterDisplay characterDisplay, Tunes tunes)
        {
            _mainboard = mainboard;
            _breakout = breakout;
            _characterDisplay = characterDisplay;
            _tunes = tunes;
        }

        public override ILed CreateIndicatorKeyA()
        {
            return SetupHeaderOutput(PinsCerbuino.D6);
        }

        public override ILed CreateIndicatorKeyB()
        {
            return SetupHeaderOutput(PinsCerbuino.D7);
        }

        public override ILed CreateIndicatorSelect()
        {
            return SetupHeaderOutput(PinsCerbuino.D8);
        }

        public override ILed CreateIndicatorArm()
        {
            return SetupHeaderOutput(PinsCerbuino.D9);
        }

        public override ILed CreateIndicatorFire()
        {
            return SetupHeaderOutput(PinsCerbuino.D10);
        }

        public override ILed CreateIndicatorRunning()
        {
            return SetupBreakoutOutput(Socket.Pin.Three);
        }

        public override ILed CreateIndicatorSucceeded()
        {
            return SetupBreakoutOutput(Socket.Pin.Four);
        }

        public override ILed CreateIndicatorFailed()
        {
            return SetupBreakoutOutput(Socket.Pin.Five);
        }

        public override IGarbage CreateGarbage()
        {
            return new GarbageCollector();
        }

        public override ILogger CreateLogger()
        {
            return new Logger();
        }

        public override ICharDisplay CreateCharacterDisplay()
        {
            return new CharDisplay(_characterDisplay);
        }

        public override ISound CreateSound()
        {
            return new Sound(_tunes);
        }

        // Try mIP? http://mip.codeplex.com/
        public override INetwork CreateNetworkWrapper()
        {
            try
            {
                _characterDisplay.Clear();
                _characterDisplay.SetCursorPosition(0, 0);
                _characterDisplay.Print("Getting IP address...");

                //NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
                //NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

                var eth = _mainboard.Ethernet;
                eth.Open();
                eth.EnableDhcp();
                eth.EnableDynamicDns();
                while (eth.IPAddress == "0.0.0.0")
                {
                    Debug.Print("Waiting for DHCP");
                    Thread.Sleep(250);
                }
                return new NetworkWrapper(eth);
            }
            catch (Exception ex)
            {
                Debug.Print("Could not set up Ethernet - " + ex);
                throw;
            }
        }

        #region Indicator outputs

        private Led SetupHeaderOutput(Cpu.Pin pin)
        {
            try
            {
                var port = new OutputPort(pin, false);
                return new Led(port);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
            return new Led();
        }

        private LedDigital SetupBreakoutOutput(Socket.Pin pin)
        {
            var output = _breakout.CreateDigitalOutput(pin, false);
            return new LedDigital(output);
        }

        #endregion
    }
}