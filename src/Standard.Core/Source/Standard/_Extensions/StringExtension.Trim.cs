using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    // Ordinal compare by default. In line with .NET "Foo".Trim([char[]]...)

    partial class StringExtension
    {
        // Trim

        /// <summary>
        /// Removes all leading and trailing substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="trimStrings">An array of substrings that should be removed from the leading and trailing positions of <paramref name="value"/>.</param>
        /// <returns>All leading and trailing substrings specified by <paramref name="trimStrings"/> removed from <paramref name="value"/>.</returns>
        public static string Trim(this string value, params string[] trimStrings)
        {
            return Trim(value, trimStrings, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes all leading and trailing substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="trimStrings">An array of substrings that should be removed from the leading and trailing positions of <paramref name="value"/>. The substring comparision operation is case insensitive.</param>
        /// <returns>All leading and trailing substrings specified by <paramref name="trimStrings"/> removed from <paramref name="value"/>.</returns>
        public static string TrimIgnoreCase(this string value, params string[] trimStrings)
        {
            return Trim(value, trimStrings, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes all leading and trailing substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="trimStrings">An array of substrings that should be removed from the leading and trailing positions of <paramref name="value"/>.</param>
        /// <param name="comparisonType">Controls how substrings are being compared.</param>
        /// <returns>All leading and trailing substrings specified by <paramref name="trimStrings"/> removed from <paramref name="value"/>.</returns>
        public static string Trim(this string value, string[] trimStrings, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (trimStrings == null || trimStrings.Length == 0)
                return value.Trim();

            int trimStartLength = 0;
            int trimEndLength = 0;
            foreach (string item in trimStrings)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                if (value.StartsWith(item, comparisonType) && (item.Length > trimStartLength))
                    trimStartLength = item.Length;

                if (value.EndsWith(item, comparisonType) && (item.Length > trimEndLength))
                    trimEndLength = item.Length;
            }

            if (trimStartLength == 0 && trimEndLength == 0)
                return value;
            else
                return value.Substring(trimStartLength, value.Length - trimEndLength - trimStartLength);
        }


        // TrimStart

        /// <summary>
        /// Removes all leading substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="prefix">An array of substrings that should be removed from the leading positions of <paramref name="value"/>.</param>
        /// <returns>All leading substrings specified by <paramref name="prefix"/> removed from <paramref name="value"/>.</returns>
        public static string TrimStart(this string value, params string[] prefix)
        {
            return TrimStart(value, prefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes all leading substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="prefix">An array of substrings that should be removed from the leading positions of <paramref name="value"/>. The substring comparision operation is case insensitive.</param>
        /// <returns>All leading substrings specified by <paramref name="prefix"/> removed from <paramref name="value"/>.</returns>
        public static string TrimStartIgnoreCase(this string value, params string[] prefix)
        {
            return TrimStart(value, prefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes all leading substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="prefix">An array of substrings that should be removed from the leading positions of <paramref name="value"/>.</param>
        /// <param name="comparisonType">Controls how substrings are being compared.</param>
        /// <returns>All leading substrings specified by <paramref name="prefix"/> removed from <paramref name="value"/>.</returns>
        public static string TrimStart(this string value, string[] prefix, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            // validate
            if (prefix == null || prefix.Length == 0)
                return value.TrimStart();

            // find the longest prefix that matches
            int trimLength = 0;
            foreach (string prefixItem in prefix)
            {
                if (string.IsNullOrEmpty(prefixItem))
                    continue;

                if (value.StartsWith(prefixItem, comparisonType) && (prefixItem.Length > trimLength))
                    trimLength = prefixItem.Length;
            }

            // clip off
            if (trimLength == 0)
                return value;
            else
                return value.Substring(trimLength);
        }


        // TrimEnd

        /// <summary>
        /// Removes all leading substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="suffix">An array of substrings that should be removed from the trailing positions of <paramref name="value"/>.</param>
        /// <returns>All trailing substrings specified by <paramref name="suffix"/> removed from <paramref name="value"/>.</returns>
        public static string TrimEnd(this string value, params string[] suffix)
        {
            return TrimEnd(value, suffix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes all leading substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="suffix">An array of substrings that should be removed from the trailing positions of <paramref name="value"/>.</param>
        /// <returns>All trailing substrings specified by <paramref name="suffix"/> removed from <paramref name="value"/>. The substring comparision operation is case insensitive.</returns>
        public static string TrimEndIgnoreCase(this string value, params string[] suffix)
        {
            return TrimEnd(value, suffix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes all leading substrings specified from the current <see cref="string"/> object. 
        /// </summary>
        /// <param name="value">The string object.</param>
        /// <param name="suffix">An array of substrings that should be removed from the trailing positions of <paramref name="value"/>.</param>
        /// <param name="comparisonType">Controls how substrings are being compared.</param>
        /// <returns>All trailing substrings specified by <paramref name="suffix"/> removed from <paramref name="value"/>.</returns>
        public static string TrimEnd(this string value, string[] suffix, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (suffix == null || suffix.Length == 0)
                return value.TrimEnd();

            int trimLength = 0;
            foreach (string suffixItem in suffix)
            {
                if (string.IsNullOrEmpty(suffixItem))
                    continue;

                if (value.EndsWith(suffixItem, comparisonType) && (suffixItem.Length > trimLength))
                    trimLength = suffixItem.Length;
            }

            if (trimLength == 0)
                return value;
            else
                return value.Substring(0, value.Length - trimLength);
        }
    }
}
