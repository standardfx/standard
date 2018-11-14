using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.StringParsing
{
    internal static class StringExtension
    {
        public static IEnumerable<char> ToEnumerable(this string str)
        {
#if NET40
            if (str == null) 
                throw new ArgumentNullException(nameof(str));

            for (int i = 0; i < str.Length; ++i)
            {
                yield return str[i];
            }
#else
            return str;
#endif
        }
    }
}
