using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Split a string into substrings, using the character specified as the delimiter. All zero-length substrings are not returned in the result.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <param name="separator">The delimiter that separates each substring.</param>
        /// <returns>A string array consisting of substrings in <paramref name="value"/>, but excluding zero-length substrings.</returns>
        public static string[] SplitRemoveEmpty(this string value, char separator)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Split a string into substrings, using the substring specified as the delimiter. All zero-length substrings are not returned in the result.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <param name="separator">The delimiter that separates each substring.</param>
        /// <returns>A string array consisting of substrings in <paramref name="value"/>, but excluding zero-length substrings.</returns>
        public static string[] SplitRemoveEmpty(this string value, string separator)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentNullException(nameof(separator));

            return value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        // <devdoc>
        // </devdoc>

        /// <summary>
        /// Split a string into substrings, using the character specified as the delimiter.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <param name="separator">The delimiter that separates each substring.</param>
        /// <param name="options">Options to control the split operation.</param>
        /// <returns>A string array consisting of substrings in <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function exists as a workaround for BCL, which only accept char[] or string[] as separator (when using StringSplitOptions).
        /// </remarks>
        public static string[] Split(this string value, char separator, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return value.Split(new char[] { separator }, options);
        }

        /// <summary>
        /// Split a string into substrings, using the substring specified as the delimiter.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <param name="separator">The delimiter that separates each substring.</param>
        /// <param name="options">Options to control the split operation.</param>
        /// <returns>A string array consisting of substrings in <paramref name="value"/>.</returns>
        public static string[] Split(this string value, string separator, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentNullException(nameof(separator));

            return value.Split(new string[] { separator }, options);
        }

        /// <summary>
        /// Split a string into substrings, using newline characters in the string as the delimiter.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <returns>A string array consisting of substrings in <paramref name="value"/>.</returns>
        public static string[] SplitLine(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            List<string> result = new List<string>();

            using (StringReader sr = new StringReader(value))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    result.Add(line);
                }
            }
            return result.ToArray();
        }
    }
}