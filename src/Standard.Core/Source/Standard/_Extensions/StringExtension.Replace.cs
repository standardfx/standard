using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Security;
using Standard.Core;

namespace Standard
{
    // Ordinal compare by default. In line with .NET "Foo".Replace(...)

    partial class StringExtension
    {
        // No need to implement this because .NET has already implemented:
        // Replace(this string value, string oldValue, string newValue)

        /// <summary>
        /// Replace the specified number of occurances of a substring with the value specified.
        /// </summary>
        /// <param name="value">The string being searched.</param>
        /// <param name="oldValue">The substring which should be replaced.</param>
        /// <param name="newValue">The replacement value for <paramref name="oldValue"/>.</param>
        /// <param name="count">The number of occurances to replace.</param>
        /// <returns>The value of <paramref name="value"/>, with the first <paramref name="count"/> occurances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        public static string Replace(this string value, string oldValue, string newValue, int count)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                return value;

            if (count == -1)
                return value.Replace(oldValue, newValue);
            else
                return Replace(value, oldValue, newValue, StringComparison.Ordinal, count);
        }

        /// <summary>
        /// Replace the specified number of occurances of a substring with the value specified, using case-insensitive comparison.
        /// </summary>
        /// <param name="value">The string being searched.</param>
        /// <param name="oldValue">The substring which should be replaced. Case-insensitive comparison is used.</param>
        /// <param name="newValue">The replacement value for <paramref name="oldValue"/>.</param>
        /// <param name="count">The number of occurances to replace. Defaults to -1, which means only the first occurance is replaced.</param>
        /// <returns>The value of <paramref name="value"/>, with the first <paramref name="count"/> occurances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        public static string ReplaceIgnoreCase(this string value, string oldValue, string newValue, int count = -1)
        {
            return Replace(value, oldValue, newValue, StringComparison.OrdinalIgnoreCase, count);
        }

        /// <summary>
        /// Replace the specified number of occurances of a substring with the value specified.
        /// </summary>
        /// <param name="value">The string being searched.</param>
        /// <param name="oldValue">The substring which should be replaced.</param>
        /// <param name="newValue">The replacement value for <paramref name="oldValue"/>.</param>
        /// <param name="comparisonType">Controls how <paramref name="oldValue"/> is being searched.</param>
        /// <param name="count">The number of occurances to replace.</param>
        /// <returns>The value of <paramref name="value"/>, with the first <paramref name="count"/> occurances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        public static string Replace(this string value, string oldValue, string newValue, StringComparison comparisonType, int count)
        {
            return ReplaceInternal(value, oldValue, newValue, comparisonType, -1, count);
        }

        // Replace from right

        /// <summary>
        /// Replaces the last occurance of a substring with the value specified.
        /// </summary>
        /// <param name="value">The string being searched.</param>
        /// <param name="oldValue">The substring which should be replaced.</param>
        /// <param name="newValue">The replacement value for <paramref name="oldValue"/>.</param>
        /// <returns>The value of <paramref name="value"/>, with the last occurance of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        public static string ReplaceLast(this string value, string oldValue, string newValue)
        {
            return ReplaceLast(value, oldValue, newValue, StringComparison.Ordinal, -1);
        }

        /// <summary>
        /// Replaces occurances of a substring with the value specified, starting from the last occurance.
        /// </summary>
        /// <param name="value">The string being searched.</param>
        /// <param name="oldValue">The substring which should be replaced.</param>
        /// <param name="newValue">The replacement value for <paramref name="oldValue"/>.</param>
        /// <param name="count">The number of occurances to replace.</param>
        /// <returns>The value of <paramref name="value"/>, with the last <paramref name="count"/> occurances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        public static string ReplaceLast(this string value, string oldValue, string newValue, int count)
        {
            return ReplaceLast(value, oldValue, newValue, StringComparison.Ordinal, count);
        }

        /// <summary>
        /// Replaces occurances of a substring with the value specified, starting from the last occurance and using case-insensitive comparison.
        /// </summary>
        /// <param name="value">The string being searched.</param>
        /// <param name="oldValue">The substring which should be replaced. Case insensitive comparison is used.</param>
        /// <param name="newValue">The replacement value for <paramref name="oldValue"/>.</param>
        /// <param name="count">The number of occurances to replace. Defaults to -1, which means the last occurance only.</param>
        /// <returns>The value of <paramref name="value"/>, with the last <paramref name="count"/> occurances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        public static string ReplaceLastIgnoreCase(this string value, string oldValue, string newValue, int count = -1)
        {
            return ReplaceLast(value, oldValue, newValue, StringComparison.OrdinalIgnoreCase, count);
        }

        /// <summary>
        /// Replaces occurances of a substring with the value specified, starting from the last occurance.
        /// </summary>
        /// <param name="value">The string being searched.</param>
        /// <param name="oldValue">The substring which should be replaced.</param>
        /// <param name="newValue">The replacement value for <paramref name="oldValue"/>.</param>
        /// <param name="comparisonType">Controls how <paramref name="oldValue"/> is being searched.</param>
        /// <param name="count">The number of occurances to replace.</param>
        /// <returns>The value of <paramref name="value"/>, with the last <paramref name="count"/> occurances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        public static string ReplaceLast(this string value, string oldValue, string newValue, StringComparison comparisonType, int count)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException(nameof(oldValue));

            string reverseNewValue;
            if (!string.IsNullOrEmpty(newValue))
                reverseNewValue = newValue.Reverse();
            else
                reverseNewValue = null;

            return ReplaceInternal(value.Reverse(), oldValue.Reverse(), reverseNewValue, comparisonType, -1, count).Reverse();
        }

        // Internal helper

        private static string ReplaceInternal(string value, string oldValue, string newValue, StringComparison comparisonType, int bufferInitSize, int maxLoops = -1)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                return value;

            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException(nameof(oldValue));

            if (maxLoops == 0)
                return value;

            int loopCount = 0;
            int currentIndex = 0;
            int oldValueLength = oldValue.Length;
            int nextIndex = value.IndexOf(oldValue, comparisonType);
            StringBuilder sb = new StringBuilder(
                bufferInitSize < 0 
                    ? Math.Min(4096, value.Length) 
                    : bufferInitSize);

            while ((nextIndex >= 0) && (maxLoops == -1 ? true : loopCount < maxLoops))
            {
                sb.Append(value, currentIndex, nextIndex - currentIndex);
                if (!string.IsNullOrEmpty(newValue)) 
                    sb.Append(newValue);
                currentIndex = nextIndex + oldValueLength;
                nextIndex = value.IndexOf(oldValue, currentIndex, comparisonType);

                loopCount += 1;
            }

            sb.Append(value, currentIndex, value.Length - currentIndex);

            return sb.ToString();
        }
    }
}