using System;
using System.Windows.Threading;
using Deployer.Services.Hardware;
using System.Windows.Controls;

namespace Deployer.Wpf.Hardware
{
    public class CharDisplay : ICharDisplay
    {
        private readonly TextBlock _lineOne;
        private readonly TextBlock _lineTwo;
        private readonly Dispatcher _dispatcher;

        public CharDisplay(TextBlock lineOne, TextBlock lineTwo, Dispatcher dispatcher)
        {
            _lineOne = lineOne;
            _lineTwo = lineTwo;
            _dispatcher = dispatcher;
        }

        public void Write(string line1, string line2 = "")
        {
            _dispatcher.Invoke(() =>
                {
                    _lineOne.Text = line1;
                    _lineTwo.Text = line2;
                });
        }
    }
}