using System;
using System.Collections.Generic;
using System.Globalization;

namespace Standard
{
    partial class StringExtension
    {
        public static bool IsDecimal(this string value)
        {
            return IsNumeric(value, new char[] {'+', '-', '.'});
        }

        public static bool IsUDecimal(this string value)
        {
            return IsNumeric(value, new char[] {'.'});
        }

        public static bool IsInteger(this string value)
        {
            return IsNumeric(value, new char[] {'+', '-'});
        }

        public static bool IsUInteger(this string value)
        {
            return IsNumeric(value, new char[] {});
        }

        public static bool IsNumeric(this string value)
        {
            return IsNumeric(value, new char[] {'.', '+', '-', ','});
        }

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
