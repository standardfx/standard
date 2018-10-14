using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        // Match

        /// <summary>
        /// Determines whether a string matches the regular expression specified.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="pattern">The regular expression to match <paramref name="value"/>.</param>
        public static bool IsMatch(this string value, string pattern)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));

            return Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// Determines whether a string matches the regular expression specified.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="pattern">The regular expression to match <paramref name="value"/>.</param>
        /// <param name="options">Options to control the regular expression engine.</param>
        public static bool IsMatch(this string value, string pattern, RegexOptions options)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));

            return Regex.IsMatch(value, pattern, options);
        }

        /// <summary>
        /// Determines whether a string matches the regular expression specified.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="pattern">The regular expression to match <paramref name="value"/>.</param>
        /// <param name="options">Options to control the regular expression engine.</param>
        /// <param name="matchTimeout">The maximum time limit when conducting the regular expression matching operation.</param>
        /// <returns></returns>
        public static bool IsMatch(this string value, string pattern, RegexOptions options, TimeSpan matchTimeout)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));

            return Regex.IsMatch(value, pattern, options, matchTimeout);
        }


        // Like

        /// <summary>
        /// Compares a string against a wildcard pattern. The comparison is case sensitive.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="wildcard">The wildcard pattern which <paramref name="value"/> should match.</param>
        public static bool IsLike(this string value, string wildcard)
            => IsLike(value, wildcard, false);

        /// <summary>
        /// Compares a string against a wildcard pattern.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="wildcard">The wildcard pattern which <paramref name="value"/> should match.</param>
        /// <param name="ignoreCase">Determines whether casing should be considered when comparing.</param>
        /// <remarks>
        /// Wildcard pattern match has less features than regular expressions, but performs significantly faster.
        /// </remarks>
        public static bool IsLike(this string value, string wildcard, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
            { 
                if (string.IsNullOrEmpty(wildcard))
                    return true;
                else if (wildcard == "*")
                    return true;
            }
            
            if (string.IsNullOrEmpty(wildcard))
                return false;

            if (ignoreCase)
            {
                value = value.ToUpperInvariant();
                wildcard = wildcard.ToUpperInvariant();
            }
            
            // characters matched so far
            int matched = 0;

            // loop through pattern string
            for (int i = 0; i < wildcard.Length;)
            {
                // check for end of string
                if (matched > value.Length)
                    return false;

                // get next pattern character
                char c = wildcard[i++];
                if (c == '[') // character list
                {
                    // test for exclude character
                    bool exclude = (i < wildcard.Length && wildcard[i] == '!');
                    if (exclude)
                        i++;
                        
                    // build character list
                    int j = wildcard.IndexOf(']', i);
                    if (j < 0)
                        j = value.Length;
                        
                    HashSet<char> charList = CharListToSet(wildcard.Substring(i, j - i));
                    i = j + 1;

                    if (charList.Contains(value[matched]) == exclude)
                        return false;
                    matched++;
                }
                else if (c == '?') // any single character
                {
                    matched++;
                }
                else if (c == '#') // any single digit
                {
                    if (!char.IsDigit(value[matched]))
                        return false;
                        
                    matched++;
                }
                else if (c == '*') // zero or more characters
                {
                    if (i < wildcard.Length)
                    {
                        // matches all characters until next character in pattern
                        char next = wildcard[i];
                        int j = value.IndexOf(next, matched);
                        if (j < 0)
                            return false;

                        matched = j;
                    }
                    else
                    {
                        // matches all remaining characters
                        matched = value.Length;
                        break;
                    }
                }
                else // exact character
                {
                    if (matched >= value.Length || c != value[matched])
                        return false;
                    matched++;
                }
            }
            
            // return true if all characters matched
            return (matched == value.Length);
        }

        /// <summary>
        /// Converts a string of characters to a HashSet of characters. If the string
        /// contains character ranges, such as A-Z, all characters in the range are
        /// also added to the returned set of characters.
        /// </summary>
        /// <param name="charList">Character list string</param>
        private static HashSet<char> CharListToSet(string charList)
        {
            HashSet<char> set = new HashSet<char>();

            for (int i = 0; i < charList.Length; i++)
            {
                if ((i + 1) < charList.Length && charList[i + 1] == '-')
                {
                    // character range
                    char startChar = charList[i++];
                    i++; // hyphen
                    char endChar = (char)0;
                    if (i < charList.Length)
                        endChar = charList[i++];
                    for (int j = startChar; j <= endChar; j++)
                        set.Add((char)j);
                }
                else
                {
                    set.Add(charList[i]);
                }
            }
            return set;
        }
    }
}
