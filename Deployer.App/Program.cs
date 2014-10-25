using Deployer.App.Abstraction;
using Deployer.Services.Input;
using Deployer.Services.RunModes;
using Deployer.Services.StateMachine;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.IO;

namespace Deployer.App
{
    public partial class Program
    {
        private InterruptPort _keySwitchA;
        private InterruptPort _keySwitchB;
        private InterruptPort _buttonUp;
        private InterruptPort _buttonDown;
        private InterruptPort _buttonArm;
        private InterruptPort _buttonDeploy;

        private string _rootDir;
        private Gadgeteer.Timer _timerBlink;

        private ModeRunner _modeRunner;

        private void ProgramStarted()
        {
            var memory = Debug.GC(true);
            Debug.Print("Memory at startup = " + memory);

            SetupPersistence();
            SetupInputs();

            var factory = new RealDeployerFactory(Mainboard.Ethernet, breakoutTB10, characterDisplay, tunes);
            factory.Initialize();
            _modeRunner = new ModeRunner(factory, _rootDir);
            _modeRunner.Start();

            SetupInterrupts();
            SetupTimers();
        }

        #region Setup methods

        private void SetupInputs()
        {
            _keySwitchA = SetupInterruptOffAndOn(PinsCerbuino.A0);
            _keySwitchB = SetupInterruptOffAndOn(PinsCerbuino.A1);

            _buttonUp = SetupInterruptRelease(PinsCerbuino.D2);
            _buttonDown = SetupInterruptRelease(PinsCerbuino.A2);
            _buttonArm = SetupInterruptRelease(PinsCerbuino.D4);
            _buttonDeploy = SetupInterruptRelease(PinsCerbuino.D5);
        }

        private void SetupInterrupts()
        {
            _keySwitchA.OnInterrupt += KeySwitchAOnInterrupt;
            _keySwitchB.OnInterrupt += KeySwitchBOnInterrupt;

            _buttonUp.OnInterrupt += ButtonUpOnInterrupt;
            _buttonDown.OnInterrupt += ButtonDownOnInterrupt;

            _buttonArm.OnInterrupt += ButtonArmOnInterrupt;
            _buttonDeploy.OnInterrupt += ButtonDeployOnInterrupt;
        }

        private void SetupTimers()
        {
            _timerBlink = new Gadgeteer.Timer(500);
            _timerBlink.Tick += BlinkTick;
            _timerBlink.Start();
        }

        private void SetupPersistence()
        {
            try
            {
                //var storage = new Persistence(Mainboard.SDCardStorageDevice);
                var isStorage = Mainboard.IsSDCardInserted;
                if (!isStorage)
                    throw new Exception("No SD card has been inserted");
                _rootDir = Mainboard.SDCardStorageDevice.Volume.RootDirectory;
                var authDir = Path.Combine(_rootDir, "auth");
                var configDir = Path.Combine(_rootDir, "config");
                var clientDir = Path.Combine(_rootDir, "client");
                if (!Directory.Exists(authDir))
                    Directory.CreateDirectory(authDir);
                if (!Directory.Exists(configDir))
                    Directory.CreateDirectory(configDir);
                if (!Directory.Exists(clientDir))
                    Directory.CreateDirectory(clientDir);
            }
            catch (Exception ex)
            {
                Debug.Print("Could not initialize storage - " + ex);
            }
        }

        #endregion

        #region Key switches

        private bool ReversedSwitchA
        {
            get { return !_keySwitchA.Read(); }
        }

        private bool ReversedSwitchB
        {
            get { return !_keySwitchB.Read(); }
        }

        private void KeySwitchAOnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (ReversedSwitchA)
                _modeRunner.KeyOnEvent(KeySwitch.KeyA);
            else
                _modeRunner.KeyOffEvent(KeySwitch.KeyA);
        }

        private void KeySwitchBOnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (ReversedSwitchB)
                _modeRunner.KeyOnEvent(KeySwitch.KeyB);
            else
                _modeRunner.KeyOffEvent(KeySwitch.KeyB);
        }

        #endregion

        #region Push buttons

        private void ButtonUpOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _modeRunner.UpPressedEvent();
        }

        private void ButtonDownOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _modeRunner.DownPressedEvent();
        }


        private void ButtonArmOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _modeRunner.ArmPressedEvent();
        }

        private void ButtonDeployOnInterrupt(uint data1, uint data2, DateTime time)
        {
            _modeRunner.DeployPressedEvent();
        }

        #endregion

        #region Interrupt setup

        private InterruptPort SetupInterruptOffAndOn(Cpu.Pin pin)
        {
            return new InterruptPort(pin, true,
                                     Port.ResistorMode.PullUp,
                                     Port.InterruptMode.InterruptEdgeBoth);
        }

        private InterruptPort SetupInterruptRelease(Cpu.Pin pin)
        {
            return new InterruptPort(pin, true,
                                     Port.ResistorMode.PullUp,
                                     Port.InterruptMode.InterruptEdgeLow);
        }

        #endregion

        #region Blink

        private void BlinkTick(Gadgeteer.Timer timer)
        {
            var memory = Debug.GC(false);
            Debug.Print("Tick! Memory = " + memory);

            _modeRunner.Tick();
        }

        #endregion
    }
}