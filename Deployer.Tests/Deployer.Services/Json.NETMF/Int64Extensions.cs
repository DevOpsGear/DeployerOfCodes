using System;

namespace Json.NETMF
{
	internal static class Int64Extensions
	{
		public static long Parse(string str)
		{
			long result;
			if (TryParse(str, out result))
			{
				return result;
			}
			throw new Exception();
		}

		public static long Parse(string str, NumberStyle style)
		{
			if (style == NumberStyle.Hexadecimal)
			{
				return ParseHex(str);
			}

			return Parse(str);
		}

		public static bool TryParse(string str, out long result)
		{
			result = 0;
			ulong r;
			bool sign;
			if (Helper.TryParseUInt64Core(str, false, out r, out sign))
			{
				if (!sign)
				{
					if (r <= 9223372036854775807)
					{
						result = unchecked((long)r);
						return true;
					}
				}
				else
				{
					if (r <= 9223372036854775808)
					{
						result = unchecked(-((long)r));
						return true;
					}
				}
			}
			return false;
		}

		private static long ParseHex(string str)
		{
			ulong result;
			if (TryParseHex(str, out result))
			{
				return (long)result;
			}
			throw new Exception();
		}

		private static bool TryParseHex(string str, out ulong result)
		{
			bool sign;
			return Helper.TryParseUInt64Core(str, true, out result, out sign);
		}

	}
}