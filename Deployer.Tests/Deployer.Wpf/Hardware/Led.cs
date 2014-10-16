using Deployer.Services.Hardware;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Deployer.Wpf.Hardware
{
    public class Led : ILed
    {
        private readonly Rectangle _rect;
        private readonly Dispatcher _dispatcher;
        private readonly Brush _brushBlack;
        private readonly Brush _brushBlue;

        public Led(Rectangle rect, Dispatcher dispatcher, Color? activeColor = null)
        {
            if (activeColor == null)
                activeColor = Colors.DodgerBlue;

            _rect = rect;
            _dispatcher = dispatcher;
            _brushBlack = new SolidColorBrush(Colors.Black);
            _brushBlue = new SolidColorBrush(activeColor.Value);
        }

        public void Write(bool state)
        {
            _dispatcher.Invoke(() => { _rect.Fill = state ? _brushBlue : _brushBlack; });
        }
    }
}