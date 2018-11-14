using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.StringParsing
{
    internal static class StringHelper
    {
        public static string Join<T>(string separator, IEnumerable<T> values)
        {
#if NET35
            return string.Join(separator, values.Select(v => v.ToString()).ToArray());
#else
            return string.Join(separator, values);
#endif
        }
    }
}
