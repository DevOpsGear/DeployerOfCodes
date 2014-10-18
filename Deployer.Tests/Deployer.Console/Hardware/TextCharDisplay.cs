using Deployer.Services.Hardware;

namespace Deployer.Text.Hardware
{
    public class TextCharDisplay : ICharDisplay
    {
        private string _previousLine1;
        private string _previousLine2;

        public void Write(string line1, string line2 = "")
        {
            if (_previousLine1 == line1 && _previousLine2 == line2)
                return;

            System.Console.WriteLine("LCD: {0} / {1}", line1, line2);
            _previousLine1 = line1;
            _previousLine2 = line2;
        }
    }
}