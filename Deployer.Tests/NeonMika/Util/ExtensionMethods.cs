// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace NeonMika.Util
{
    public static class ExtensionMethods
    {
        public static string[] EasySplit(this string s, string separator)
        {
            int pos = s.IndexOf(separator);
            if (pos != -1)
                return new[]
                    {
                        s.Substring(0, pos).Trim(new[] {' ', '\n', '\r'}),
                        s.Substring(pos + separator.Length, s.Length - pos - separator.Length)
                         .Trim(new[] {' ', '\n', '\r'})
                    };
            return new[] {s.Trim(new[] {' ', '\n', '\r'})};
        }

        public static bool StartsWith(this string s, string start)
        {
            for (var i = 0; i < start.Length; i++)
                if (s[i] != start[i])
                    return false;

            return true;
        }

        public static string Replace(this string s, char replaceThis, char replaceWith)
        {
            var temp = string.Empty;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == replaceThis)
                    temp += replaceWith;
                else
                    temp += s[i];
            }
            return temp;
        }
    }
}

// ReSharper restore StringIndexOfIsCultureSpecific.1