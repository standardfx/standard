using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    // Ordinal compare by default. In line with .NET "Foo".Equals(...)

    partial class StringExtension
    {
        public static bool EqualsIgnoreCase(this string value, string compareTo)
        {
            if (value == null)
                return false;
                
            return string.Equals(value, compareTo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
