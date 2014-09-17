using Deployer.Services.Hardware;
using Gadgeteer.Modules.GHIElectronics;

namespace Deployer.App.Hardware
{
	public class CharDisplay : ICharDisplay
	{
		private readonly CharacterDisplay _cd;
		private string _previousLine1;
		private string _previousLine2;

		public CharDisplay(CharacterDisplay cd)
		{
			_cd = cd;
			_previousLine1 = "";
			_previousLine2 = "";
		}

		public void Write(string line1, string line2 = "")
		{
			if (line1 == _previousLine1 && line2 == _previousLine2)
				return;

			_cd.Clear();
			_cd.SetCursorPosition(0, 0);
			_cd.Print(line1);
			_cd.SetCursorPosition(1, 0);
			_cd.Print(line2);

			_previousLine1 = line1;
			_previousLine2 = line2;
		}
	}
}