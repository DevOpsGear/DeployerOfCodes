namespace Json.NETMF
{
	internal static class CharExtensions
	{
		/// <summary>
		/// Converts a Unicode character to a string of its ASCII equivalent.
		/// Very simple, it works only on ordinary characters.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static string ConvertFromUtf32(int p)
		{
			char c = (char)p;
			return c.ToString();
		}
	}
}