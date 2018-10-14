using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Removes characters from a string.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <param name="oldChars">Characters which should be removed from <paramref name="value"/>.</param>
        /// <returns>A new string which is the same as <paramref name="value"/>, but with all occurances of characters in <paramref name="oldChars"/> removed.</returns>
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

        /// <summary>
        /// Removes substrings from a string.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <param name="substring">Substrings which should be removed from <paramref name="value"/>.</param>
        /// <returns>A new string which is the same as <paramref name="value"/>, but with all occurances of substrings in <paramref name="substring"/> removed.</returns>
        public static string Remove(this string value, string[] substring)
        {
            return Remove(value, substring, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes substrings from a string.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <param name="substring">Substrings which should be removed from <paramref name="value"/>. Casing is ignored when searching for substrings.</param>
        /// <returns>A new string which is the same as <paramref name="value"/>, but with all occurances of substrings in <paramref name="substring"/> removed.</returns>
        public static string RemoveIgnoreCase(this string value, string[] substring)
        {
            return Remove(value, substring, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes substrings from a string.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <param name="substring">Substrings which should be removed from <paramref name="value"/>.</param>
        /// <param name="comparisonType">Defines how substrings are searched.</param>
        /// <returns>A new string which is the same as <paramref name="value"/>, but with all occurances of substrings in <paramref name="substring"/> removed.</returns>
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

        /// <summary>
        /// Removes all occurances of substrings that matches the regular expression specified.
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <param name="searchExpr">The regular expression to search.</param>
        /// <returns>All substrings that matches <paramref name="searchExpr"/> removed.</returns>
        public static string Remove(this string value, Regex searchExpr)
        {
            return searchExpr.Replace(value, string.Empty);
        }
    }
}
