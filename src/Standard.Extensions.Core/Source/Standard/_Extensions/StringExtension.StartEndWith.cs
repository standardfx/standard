using System;
using System.Collections.Generic;
using System.Globalization;

namespace Standard
{
    // Using **CurrentCulture** as default...in line with .NET "foo".StartsWith("f");
    
    partial class StringExtension
    {
        public static bool StartsWithIgnoreCase(this string value, string prefix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool StartsWithOrdinal(this string value, string prefix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.StartsWith(prefix, StringComparison.Ordinal);
        }

        public static bool StartsWithOrdinalIgnoreCase(this string value, string prefix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(this string value, string suffix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool EndsWithOrdinal(this string value, string suffix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.EndsWith(suffix, StringComparison.Ordinal);
        }

        public static bool EndsWithOrdinalIgnoreCase(this string value, string suffix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }


        // Ensure starts with...

        public static string EnsureStartsWith(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.CurrentCulture);
        }

        public static string EnsureStartsWithIgnoreCase(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string EnsureStartsWithOrdinal(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.Ordinal);
        }

        public static string EnsureStartsWithOrdinalIgnoreCase(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.OrdinalIgnoreCase);
        }

        public static string EnsureStartsWith(this string value, string prefix, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(prefix))
                return value;

            if (!value.StartsWith(prefix, comparisonType))
                return string.Concat(prefix, value);
            else
                return value;
        }


        // Ensure ends with...

        public static string EnsureEndsWith(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.CurrentCulture);
        }

        public static string EnsureEndsWithIgnoreCase(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string EnsureEndsWithOrdinal(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.Ordinal);
        }

        public static string EnsureEndsWithOrdinalIgnoreCase(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.OrdinalIgnoreCase);
        }

        public static string EnsureEndsWith(this string value, string suffix, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(suffix))
                return value;

            if (!value.EndsWith(suffix, comparisonType))
                return string.Concat(value, suffix);
            else
                return value;
        }
    }
}
