using Deployer.Services.Hardware;

namespace Deployer.Text.Hardware
{
    public class CharDisplay : ICharDisplay
    {
        public void Write(string line1, string line2 = "")
        {
	        System.Console.WriteLine("LCD: {0} / {1}", line1, line2);
        }
    }
}