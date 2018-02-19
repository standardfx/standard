using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.Parsing
{
    internal static class StringHelper
    {
        public static string Join<T>(string separator, IEnumerable<T> values)
        {
#if NET4X || NETSTANDARD || SL5
            return string.Join(separator, values);
#else
            return string.Join(separator, values.Select(v => v.ToString()).ToArray());
#endif
        }
    }
}
