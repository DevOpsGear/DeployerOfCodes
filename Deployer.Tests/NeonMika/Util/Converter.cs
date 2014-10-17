using System.Collections;

namespace NeonMika.Util
{
	public static class Converter
	{
		public static Hashtable ToHashtable(string[] lines, string seperator, int startAtLine = 0)
		{
			var toReturn = new Hashtable();
			for (var i = startAtLine; i < lines.Length; i++)
			{
				var line = lines[i].EasySplit(seperator);
				if (line.Length > 1)
					toReturn.Add(line[0], line[1]);
			}
			return toReturn;
		}
	}
}