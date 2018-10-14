using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Determines whether a string is convertable to a decimal number.
        /// </summary>
        /// <param name="value">A string which may be convertable into a decimal number.</param>
        /// <returns>`true` if <paramref name="value"/> is convertable into a decimal number. Otherwise, `false.</returns>
        public static bool IsDecimal(this string value)
        {
            return IsNumeric(value, new char[] {'+', '-', '.'});
        }

        /// <summary>
        /// Determines whether a string is convertable to an unsigned decimal number.
        /// </summary>
        /// <param name="value">A string which may be convertable into an unsigned decimal number.</param>
        /// <returns>`true` if <paramref name="value"/> is convertable into an unsigned decimal number. Otherwise, `false.</returns>
        public static bool IsUDecimal(this string value)
        {
            return IsNumeric(value, new char[] {'.'});
        }

        /// <summary>
        /// Determines whether a string is convertable to an integer number.
        /// </summary>
        /// <param name="value">A string which may be convertable into an integer number.</param>
        /// <returns>`true` if <paramref name="value"/> is convertable into an integer number. Otherwise, `false.</returns>
        public static bool IsInteger(this string value)
        {
            return IsNumeric(value, new char[] {'+', '-'});
        }

        /// <summary>
        /// Determines whether a string is convertable to an unsigned integer number.
        /// </summary>
        /// <param name="value">A string which may be convertable into an unsigned integer number.</param>
        /// <returns>`true` if <paramref name="value"/> is convertable into an unsigned integer number. Otherwise, `false.</returns>
        public static bool IsUInteger(this string value)
        {
            return IsNumeric(value, new char[] {});
        }

        /// <summary>
        /// Determines whether a string is convertable to a number.
        /// </summary>
        /// <param name="value">A string which may be convertable into a number.</param>
        /// <returns>`true` if <paramref name="value"/> is convertable into a number. Otherwise, `false.</returns>
        public static bool IsNumeric(this string value)
        {
            return IsNumeric(value, new char[] {'.', '+', '-', ','});
        }

        /// <summary>
        /// Determines whether a string is convertable to a number.
        /// </summary>
        /// <param name="value">A string which may be convertable into a number.</param>
        /// <param name="ignoreSymbols">Characters in <paramref name="value"/> which should be ignored by this function.</param>
        /// <returns>`true` if <paramref name="value"/> is convertable into a number. Otherwise, `false.</returns>
        public static bool IsNumeric(this string value, char[] ignoreSymbols)
        {
            if (string.IsNullOrEmpty(value))
                return false;
                
            value = value.Trim();
            for (int i = 0; i < value.Length; i++)
            {
                if (CharArrayExtension.Contains(ignoreSymbols, value[i]))
                    continue;

                int digit = 0;
                bool success = Int32.TryParse(value[i].ToString(), out digit);
                if (!success) return false;
            }
            return true;
        }
    }
}
