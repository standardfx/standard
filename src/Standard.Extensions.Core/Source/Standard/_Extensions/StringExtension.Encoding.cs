using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Converts non-ASCII characters in a string to the question mark character '?'.
        /// </summary>
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
