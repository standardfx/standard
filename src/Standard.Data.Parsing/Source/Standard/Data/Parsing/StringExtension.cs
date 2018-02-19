using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.Parsing
{
    internal static class StringExtension
    {
        public static IEnumerable<char> ToEnumerable(this string str)
        {
#if NET35 || NET4X || SL5
            return str;
#else
            if (str == null) 
                throw new ArgumentNullException(nameof(str));

            for (int i = 0; i < str.Length; ++i)
            {
                yield return str[i];
            }
#endif
        }
    }
}
