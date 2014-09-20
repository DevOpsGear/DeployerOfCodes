// Source code is modified from Mike Jones's JSON Serialization and Deserialization library (https://www.ghielectronics.com/community/codeshare/entry/357)

using System;

namespace Json.NETMF
{
	internal static class UInt32Extensions
	{
		public static bool TryParse(string str, NumberStyle style, out UInt32 result)
		{
			bool sign;
			ulong tmp;

			bool bresult = Helper.TryParseUInt64Core(str, style == NumberStyle.Hexadecimal ? true : false, out tmp, out sign);
			result = (UInt32)tmp;

			return bresult && !sign;
		}
	}

	#region Private Static Helper Methods

	#endregion
}
