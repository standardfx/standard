using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    // Ordinal compare by default. In line with .NET "Foo".Equals(...)

    partial class StringExtension
    {
        /// <summary>
        /// Determines whether two specified <see cref="string"/> object have the same value. Comparison is done using <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="value">The first string value to compare.</param>
        /// <param name="compareTo">The second string value to compare.</param>
        /// <returns>`true` if <paramref name="value"/> is equal to <paramref name="compareTo"/>, using <see cref="StringComparison.OrdinalIgnoreCase"/>. Otherwise, `false`.</returns>
        public static bool EqualsIgnoreCase(this string value, string compareTo)
        {
            if (value == null)
                return false;
                
            return string.Equals(value, compareTo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
