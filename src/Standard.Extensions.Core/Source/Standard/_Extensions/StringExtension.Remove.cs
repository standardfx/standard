using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;

namespace Standard
{
    partial class StringExtension
    {
        [SecuritySafeCritical]
        public static unsafe string Remove(this string value, char[] oldChars)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (oldChars == null | oldChars.Length == 0)
                return value;

            int len = value.Length;
            int subLen = oldChars.Length;
            char* newChars = stackalloc char[len];
            char* currentChar = newChars;
            int i = 0;
            int j = 0;
            
            while (i < len)
            {
                char c = value[i];
                
                j = 0;
                while (j < subLen)
                {
                    if (c == oldChars[j])
                        goto NEXTCHAR;                        
                    
                    j++;
                }
                
                *currentChar++ = c;
                
                NEXTCHAR:
                i++;
            }
            
            return new string(newChars, 0, (int)(currentChar - newChars));
        }

        public static string Remove(this string value, string[] substring)
        {
            return Remove(value, substring, StringComparison.Ordinal);
        }

        public static string RemoveIgnoreCase(this string value, string[] substring)
        {
            return Remove(value, substring, StringComparison.OrdinalIgnoreCase);
        }

        public static string Remove(this string value, string[] substring, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (substring == null || substring.Length == 0)
                return value;

            if (substring.Length == 1)
                return StringExtension.Replace(value, substring[0], string.Empty, comparisonType, -1);

            substring = StringArrayExtension.RemoveNullOrEmpty(substring);
            substring = StringArrayExtension.Unique(substring, comparisonType);

            string newValue = value;
            foreach (string sub in substring)
            {
                newValue = StringExtension.Replace(newValue, sub, string.Empty, comparisonType, -1);
            }
            return newValue;
        }

        public static string Remove(this string value, Regex searchExpr)
        {
            return searchExpr.Replace(value, string.Empty);
        }
    }
}
