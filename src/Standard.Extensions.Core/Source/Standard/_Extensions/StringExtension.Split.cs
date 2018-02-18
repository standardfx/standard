using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Standard
{
    partial class StringExtension
    {
        public static string[] SplitRemoveEmpty(this string value, char separator)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitRemoveEmpty(this string value, string separator)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentNullException(nameof(separator));

            return value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <devdoc>
        /// BCL only accept char[] or string[] as separator (when using StringSplitOptions). This is a workaround.
        /// </devdoc>
        public static string[] Split(this string value, char separator, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return value.Split(new char[] { separator }, options);
        }

        public static string[] Split(this string value, string separator, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(separator))
                throw new ArgumentNullException(nameof(separator));

            return value.Split(new string[] { separator }, options);
        }

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