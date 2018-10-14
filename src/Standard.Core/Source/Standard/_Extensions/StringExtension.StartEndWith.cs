using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    // Using **CurrentCulture** as default...in line with .NET "foo".StartsWith("f");
    
    partial class StringExtension
    {
        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string, using <see cref="StringComparison.CurrentCultureIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The string to compare.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is `null`.</exception>
        /// <returns>`true` if the <paramref name="prefix"/> parameter matches the beginning of <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool StartsWithIgnoreCase(this string value, string prefix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The string to compare.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is `null`.</exception>
        /// <returns>`true` if the <paramref name="prefix"/> parameter matches the beginning of <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool StartsWithOrdinal(this string value, string prefix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.StartsWith(prefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The string to compare.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is `null`.</exception>
        /// <returns>`true` if the <paramref name="prefix"/> parameter matches the beginning of <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool StartsWithOrdinalIgnoreCase(this string value, string prefix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches the specified string, using <see cref="StringComparison.CurrentCultureIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The string to compare.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is `null`.</exception>
        /// <returns>`true` if the <paramref name="suffix"/> parameter matches the ending of <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool EndsWithIgnoreCase(this string value, string suffix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.EndsWith(suffix, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches the specified string, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The string to compare.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is `null`.</exception>
        /// <returns>`true` if the <paramref name="suffix"/> parameter matches the ending of <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool EndsWithOrdinal(this string value, string suffix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.EndsWith(suffix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches the specified string, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The string to compare.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is `null`.</exception>
        /// <returns>`true` if the <paramref name="suffix"/> parameter matches the ending of <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool EndsWithOrdinalIgnoreCase(this string value, string suffix)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }


        // Ensure starts with...

        /// <summary>
        /// Ensures that the string begins with the specified prefix, using <see cref="StringComparison.CurrentCulture"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The substring that will be prepended to <paramref name="value"/> if <paramref name="value"/> does not already begin with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="prefix"/> prepended if <paramref name="value"/> does not already begin with <paramref name="prefix"/>.</returns>
        public static string EnsureStartsWith(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Ensures that the string begins with the specified prefix, using <see cref="StringComparison.CurrentCultureIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The substring that will be prepended to <paramref name="value"/> if <paramref name="value"/> does not already begin with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="prefix"/> prepended if <paramref name="value"/> does not already begin with <paramref name="prefix"/>.</returns>
        public static string EnsureStartsWithIgnoreCase(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Ensures that the string begins with the specified prefix, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The substring that will be prepended to <paramref name="value"/> if <paramref name="value"/> does not already begin with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="prefix"/> prepended if <paramref name="value"/> does not already begin with <paramref name="prefix"/>.</returns>
        public static string EnsureStartsWithOrdinal(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Ensures that the string begins with the specified prefix, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The substring that will be prepended to <paramref name="value"/> if <paramref name="value"/> does not already begin with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="prefix"/> prepended if <paramref name="value"/> does not already begin with <paramref name="prefix"/>.</returns>
        public static string EnsureStartsWithOrdinalIgnoreCase(this string value, string prefix)
        {
            return EnsureStartsWith(value, prefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Ensures that the string begins with the specified prefix, using the specified comparison option.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="prefix">The substring that will be prepended to <paramref name="value"/> if <paramref name="value"/> does not already begin with this value.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="value"/> and <paramref name="prefix"/> are compared.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="prefix"/> prepended if <paramref name="value"/> does not already begin with <paramref name="prefix"/>.</returns>
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

        /// <summary>
        /// Ensures that the string ends with the specified prefix, using <see cref="StringComparison.CurrentCulture"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The substring that will be appended to <paramref name="value"/> if <paramref name="value"/> does not already ends with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="suffix"/> appended if <paramref name="value"/> does not already end with <paramref name="suffix"/>.</returns>
        public static string EnsureEndsWith(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Ensures that the string ends with the specified prefix, using <see cref="StringComparison.CurrentCultureIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The substring that will be appended to <paramref name="value"/> if <paramref name="value"/> does not already ends with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="suffix"/> appended if <paramref name="value"/> does not already end with <paramref name="suffix"/>.</returns>
        public static string EnsureEndsWithIgnoreCase(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Ensures that the string ends with the specified prefix, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The substring that will be appended to <paramref name="value"/> if <paramref name="value"/> does not already ends with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="suffix"/> appended if <paramref name="value"/> does not already end with <paramref name="suffix"/>.</returns>
        public static string EnsureEndsWithOrdinal(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Ensures that the string ends with the specified prefix, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The substring that will be appended to <paramref name="value"/> if <paramref name="value"/> does not already ends with this value.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="suffix"/> appended if <paramref name="value"/> does not already end with <paramref name="suffix"/>.</returns>
        public static string EnsureEndsWithOrdinalIgnoreCase(this string value, string suffix)
        {
            return EnsureEndsWith(value, suffix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Ensures that the string ends with the specified prefix, using the specified comparison option.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="suffix">The substring that will be appended to <paramref name="value"/> if <paramref name="value"/> does not already ends with this value.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="value"/> and <paramref name="suffix"/> are compared.</param>
        /// <returns>The value of <paramref name="value"/>, with <paramref name="suffix"/> appended if <paramref name="value"/> does not already end with <paramref name="suffix"/>.</returns>
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
