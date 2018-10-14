using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Converts non-ASCII characters in a string to the question mark character '?'.
        /// </summary>
        /// <param name="value">A string which may contain non-ASCII characters.</param>
        /// <returns>All non-ASCII characters replaced by the question mark character '?'.</returns>
        public static string ToASCII(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;
                
            ASCIIEncoding ascii = new ASCIIEncoding();
            return ascii.GetString(ascii.GetBytes(value));
        }
    }
}
