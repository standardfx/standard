using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using Standard.Core;

namespace Standard
{
    public static partial class StringExtension
    {
        // Truncate

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string Truncate(this string value, int maxLength)
            => Truncate(value, maxLength, null, false);

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string Truncate(this string value, int maxLength, bool rtl)
            => Truncate(value, maxLength, null, rtl);

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string Truncate(this string value, int maxLength, string tail)
            => Truncate(value, maxLength, tail, false);

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        /// <remarks>
        /// <code>
        /// string message = "this is an overlong string";
        /// string trunated = message.Truncate(4); // "this..."
        /// </code>
        /// </remarks>
        public static string Truncate(this string value, int maxLength, string tail, bool rtl)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), RS.Err_RequireGeZero);

            if (value.Length == 0)
                return string.Empty;

            if (maxLength > value.Length)
                maxLength = value.Length;

            // if tail is > maxLength, we crop the string to maxLength and ignore the tail.
            if (tail == null || tail.Length > maxLength)
                return !rtl
                    ? value.Substring(0, maxLength)
                    : value.Substring(value.Length - maxLength);
            
            if (rtl)
            {
                // ...foobar | foobar
                return value.Length > maxLength
                    ? tail + value.Substring(value.Length - maxLength + tail.Length)
                    : value;
            }
            else
            {
                // foobar... | foobar
                return value.Length > maxLength
                    ? value.Substring(0, maxLength - tail.Length) + tail
                    : value;                
            }
        }


        // TruncateChars

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string TruncateChars(this string value, int maxLength)
            => TruncateChars(value, maxLength, null, false);

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string TruncateChars(this string value, int maxLength, bool rtl)
            => TruncateChars(value, maxLength, null, rtl);

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string TruncateChars(this string value, int maxLength, string tail)
            => TruncateChars(value, maxLength, tail, false);

        /// <summary>
        /// Truncate a string to a fixed number of letters or digits.
        /// </summary>
        /// <remarks>
        /// The function <code>Char.IsLetterOrDigit()</code> is used internally to validate each character in the string specified.
        /// </remarks>
        public static string TruncateChars(this string value, int maxLength, string tail, bool rtl)
        {
            if (value == null)
                return null;

            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), RS.Err_RequireGeZero);

            if (value.Length == 0)
                return string.Empty;

            if (tail == null)
                tail = string.Empty;

            // if tail is > maxLength, we crop the string to maxLength and ignore the tail.
            if (tail.Length > maxLength)
            {
                return !rtl 
                    ? value.Substring(0, maxLength) 
                    : value.Substring(value.Length - maxLength);
            }

            int alphanumCharsProcessed = 0;

            if (value.ToCharArray().Count(char.IsLetterOrDigit) <= maxLength)
                return value;

            if (!rtl)
            {
                for (int i = value.Length - 1; i > 0; i--)
                {
                    if (char.IsLetterOrDigit(value[i]))
                        alphanumCharsProcessed++;

                    if (alphanumCharsProcessed + tail.Length == maxLength)
                        return tail + value.Substring(i);
                }
            }

            for (int i = 0; i < value.Length - tail.Length; i++)
            {
                if (char.IsLetterOrDigit(value[i]))
                    alphanumCharsProcessed++;

                if (alphanumCharsProcessed + tail.Length == maxLength)
                    return value.Substring(0, i + 1) + tail;
            }

            return value;
        }


        // TruncateWords

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string TruncateWords(this string value, int maxLength)
            => TruncateWords(value, maxLength, null, false);

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string TruncateWords(this string value, int maxLength, bool rtl)
            => TruncateWords(value, maxLength, null, rtl);

        /// <summary>
        /// Truncates a string that exceeds the specified length.
        /// </summary>
        public static string TruncateWords(this string value, int maxLength, string tail)
            => TruncateWords(value, maxLength, tail, false);

        /// <summary>
        /// Truncates a string that exceeds the specified number of words.
        /// </summary>
        public static string TruncateWords(this string value, int maxLength, string tail, bool rtl)
        {
            if (value == null)
                return null;

            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), RS.Err_RequireGeZero);

            if (value.Length == 0)
                return string.Empty;

            int numberOfWords = value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Count();
            if (numberOfWords <= maxLength)
                return value;

            return !rtl
                ? TruncateFromLeft(value, maxLength, tail)
                : TruncateFromRight(value, maxLength, tail);
        }

        private static string TruncateFromRight(string value, int length, string tail)
        {
            bool lastCharsWasWhiteSpace = true;
            int numOfWordsProcessed = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsWhiteSpace(value[i]))
                {
                    if (!lastCharsWasWhiteSpace)
                        numOfWordsProcessed++;

                    lastCharsWasWhiteSpace = true;

                    if (numOfWordsProcessed == length)
                        return value.Substring(0, i) + tail;
                }
                else
                {
                    lastCharsWasWhiteSpace = false;
                }
            }

            return value + tail;
        }

        private static string TruncateFromLeft(string value, int length, string tail)
        {
            bool lastCharsWasWhiteSpace = true;
            int numOfWordsProcessed = 0;
            
            for (int i = value.Length - 1; i > 0; i--)
            {
                if (char.IsWhiteSpace(value[i]))
                {
                    if (!lastCharsWasWhiteSpace)
                        numOfWordsProcessed++;

                    lastCharsWasWhiteSpace = true;

                    if (numOfWordsProcessed == length)
                        return tail + value.Substring(i + 1).TrimEnd();
                }
                else
                {
                    lastCharsWasWhiteSpace = false;
                }
            }
            return tail + value;
        }
    }
}
