using Deployer.App.Hardware;
using Deployer.App.Micro;
using Deployer.Services.Abstraction;
using Deployer.Services.Hardware;
using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Networking;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NeonMika.Interfaces;
using System;
using System.Threading;

namespace Deployer.App.Abstraction
{
    public class RealDeployerFactory : CommonFactory
    {
        private readonly EthernetENC28J60 _ethernet;
        private readonly StorageDevice _storageDevice;
        private readonly BreakoutTB10 _breakout;
        private readonly CharacterDisplay _characterDisplay;
        private readonly Tunes _tunes;
        private INetwork _network;
        private Led _indicatorKeyA;
        private Led _indicatorKeyB;
        private Led _indicatorSelect;
        private Led _indicatorArm;
        private Led _indicatorFire;
        private LedDigital _indicatorRunning;
        private LedDigital _indicatorSucceeded;
        private LedDigital _indicatorFailed;

        public RealDeployerFactory(EthernetENC28J60 ethernet, StorageDevice storageDevice, BreakoutTB10 breakout,
                                   CharacterDisplay characterDisplay,
                                   Tunes tunes)
        {
            _ethernet = ethernet;
            _storageDevice = storageDevice;
            _breakout = breakout;
            _characterDisplay = characterDisplay;
            _tunes = tunes;
        }

        public override void Initialize()
        {
            _network = SetupEthernet();
            _indicatorKeyA = SetupHeaderOutput(PinsCerbuino.D6);
            _indicatorKeyB = SetupHeaderOutput(PinsCerbuino.D7);
            _indicatorSelect = SetupHeaderOutput(PinsCerbuino.D8);
            _indicatorArm = SetupHeaderOutput(PinsCerbuino.D9);
            _indicatorFire = SetupHeaderOutput(PinsCerbuino.D10);
            _indicatorRunning = SetupBreakoutOutput(Socket.Pin.Three);
            _indicatorSucceeded = SetupBreakoutOutput(Socket.Pin.Four);
            _indicatorFailed = SetupBreakoutOutput(Socket.Pin.Five);
        }

        public override ILed CreateIndicatorKeyA()
        {
            return _indicatorKeyA;
        }

        public override ILed CreateIndicatorKeyB()
        {
            return _indicatorKeyB;
        }

        public override ILed CreateIndicatorSelect()
        {
            return _indicatorSelect;
        }

        public override ILed CreateIndicatorArm()
        {
            return _indicatorArm;
        }

        public override ILed CreateIndicatorFire()
        {
            return _indicatorFire;
        }

        public override ILed CreateIndicatorRunning()
        {
            return _indicatorRunning;
        }

        public override ILed CreateIndicatorSucceeded()
        {
            return _indicatorSucceeded;
        }

        public override ILed CreateIndicatorFailed()
        {
            return _indicatorFailed;
        }

        public override IGarbage CreateGarbage()
        {
            return new GarbageCollector();
        }

        public override ILogger CreateLogger()
        {
            return new Logger();
        }

        public override IPersistence CreatePersistence()
        {
            return new Persistence(_storageDevice);
        }

        public override ICharDisplay CreateCharacterDisplay()
        {
            return new CharDisplay(_characterDisplay);
        }

        public override ISound CreateSound()
        {
            return new Sound(_tunes);
        }

        public override INetwork CreateNetworkWrapper()
        {
            return _network;
        }

        public override int WebServerPort
        {
            get { return 80; }
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

        #region Ethernet

        private INetwork SetupEthernet()
        {
            try
            {
                _characterDisplay.Clear();
                _characterDisplay.SetCursorPosition(0, 0);
                _characterDisplay.Print("Getting IP address...");

                //NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
                //NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

                // Try mIP? http://mip.codeplex.com/
                _ethernet.Open();
                _ethernet.EnableDhcp();
                _ethernet.EnableDynamicDns();
                while (_ethernet.IPAddress == "0.0.0.0")
                {
                    Debug.Print("Waiting for DHCP");
                    Thread.Sleep(250);
                }
                return new NetworkWrapper(_ethernet);
            }
            catch (Exception ex)
            {
                Debug.Print("Could not set up Ethernet - " + ex);
                throw;
            }
        }

        #endregion
    }
}