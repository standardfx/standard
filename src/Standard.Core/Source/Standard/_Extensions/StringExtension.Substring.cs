using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Returns the specified number of characters from the beginning of a string.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="length">The number of characters to return.</param>
        /// <returns>A substring of <paramref name="value"/>.</returns>
        /// <remarks>
        /// If the value of <paramref name="length"/> is zero, a zero-length string is returned. If negative, it is the same as 
        /// the <see cref="FromEnd(string, int)"/> function, using the absolute value of this parameter as the `length` parameter.
        /// 
        /// The entire <paramref name="value"/> is returned if <paramref name="length"/> is larger or equal to the length of <paramref name="value"/>.
        /// </remarks>
        public static string FromStart(this string value, int length)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((value.Length <= length) || value.Equals(string.Empty))
                return value;

            if (length == 0)
                return string.Empty;
            else if (length < 0)
                return value.FromEnd(length * -1);
            else
                return value.Substring(0, length);
        }

        /// <summary>
        /// Returns the specified number of characters from the end of a string.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="length">The number of characters to return. </param>
        /// <returns>A substring of <paramref name="value"/>.</returns>
        /// <remarks>
        /// If the value of <paramref name="length"/> is zero, a zero-length string is returned. If negative, it is the same as 
        /// the <see cref="FromStart(string, int)"/> function, using the absolute value of this parameter as the `length` parameter.
        /// 
        /// The entire <paramref name="value"/> is returned if <paramref name="length"/> is larger or equal to the length of <paramref name="value"/>.
        /// </remarks>
        public static string FromEnd(this string value, int length)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if ((value.Length <= length) || value.Equals(string.Empty))
                return value;

            if (length == 0)
                return string.Empty;
            else if (length < 0)
                return value.FromStart(length * -1);
            else
                return value.Substring(value.Length - length);
        }

        /// <summary>
        /// Return all characters before the occurance of the specified substring, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="substring">A substring of <paramref name="value"/>.</param>
        /// <returns>
        /// All characters before the first occurance of <paramref name="substring"/> in <paramref name="value"/>. If <paramref name="value"/> does not 
        /// contain <paramref name="substring"/>, the entire <paramref name="value"/> is returned.
        /// </returns>
        public static string Before(this string value, string substring)
        {
            return Before(value, substring, StringComparison.Ordinal);
        }

        /// <summary>
        /// Return all characters before the occurance of the specified substring, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="substring">A substring of <paramref name="value"/>.</param>
        /// <returns>
        /// All characters before the first occurance of <paramref name="substring"/> in <paramref name="value"/>. If <paramref name="value"/> does not 
        /// contain <paramref name="substring"/>, the entire <paramref name="value"/> is returned.
        /// </returns>
        public static string BeforeIgnoreCase(this string value, string substring)
        {
            return Before(value, substring, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Return all characters before the occurance of the specified substring.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="substring">A substring of <paramref name="value"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="substring"/> is searched in <paramref name="value"/>.</param>
        /// <returns>
        /// All characters before the first occurance of <paramref name="substring"/> in <paramref name="value"/>. If <paramref name="value"/> does not 
        /// contain <paramref name="substring"/>, the entire <paramref name="value"/> is returned.
        /// </returns>
        public static string Before(this string value, string substring, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(substring))
                throw new ArgumentNullException(nameof(substring));

            int cutIndex = value.IndexOf(substring, comparisonType);
            if (cutIndex == -1)
                return value;
            else
                return value.Substring(0, cutIndex);
        }

        /// <summary>
        /// Return all characters after the occurance of the specified substring, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="substring">A substring of <paramref name="value"/>.</param>
        /// <returns>
        /// All characters after the last occurance of <paramref name="substring"/> in <paramref name="value"/>. If <paramref name="value"/> does not 
        /// contain <paramref name="substring"/>, the entire <paramref name="value"/> is returned.
        /// </returns>
        public static string After(this string value, string substring)
        {
            return After(value, substring, StringComparison.Ordinal);
        }

        /// <summary>
        /// Return all characters after the occurance of the specified substring, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="substring">A substring of <paramref name="value"/>.</param>
        /// <returns>
        /// All characters after the last occurance of <paramref name="substring"/> in <paramref name="value"/>. If <paramref name="value"/> does not 
        /// contain <paramref name="substring"/>, the entire <paramref name="value"/> is returned.
        /// </returns>
        public static string AfterIgnoreCase(this string value, string substring)
        {
            return After(value, substring, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Return all characters after the occurance of the specified substring.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="substring">A substring of <paramref name="value"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="substring"/> is searched in <paramref name="value"/>.</param>
        /// <returns>
        /// All characters after the last occurance of <paramref name="substring"/> in <paramref name="value"/>. If <paramref name="value"/> does not 
        /// contain <paramref name="substring"/>, the entire <paramref name="value"/> is returned.
        /// </returns>
        public static string After(this string value, string substring, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(substring))
                throw new ArgumentNullException(nameof(substring));

            int cutIndex = value.IndexOf(substring, comparisonType);
            if (cutIndex == -1)
                return value;
            else
                return value.Substring(cutIndex + substring.Length);
        }

        /// <summary>
        /// Returns the substring between the all occurance of the specified strings, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">The first substring to search in <paramref name="value"/>.</param>
        /// <param name="enclosing">The second substring to search in <paramref name="value"/>.</param>
        /// <returns>All substrings between the occurances of <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string[] Between(this string value, string leading, string enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns all substrings between the occuranceS of the specified string sequences, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">An array of substrings in <paramref name="value"/> that precedes each item in the returned result.</param>
        /// <param name="enclosing">An array of substrings in <paramref name="value"/> that follows each item in the returned result.</param>
        /// <returns>All substrings between the last item of each <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string[] Between(this string value, string[] leading, string[] enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns the substring between the first occurance of the specified strings, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">The first substring to search in <paramref name="value"/>.</param>
        /// <param name="enclosing">The second substring to search in <paramref name="value"/>.</param>
        /// <returns>The substring between <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string FirstBetween(this string value, string leading, string enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns the first substring between the first occurance of the specified string sequences, using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">An array of substrings in <paramref name="value"/> that precedes each item in the returned result.</param>
        /// <param name="enclosing">An array of substrings in <paramref name="value"/> that follows each item in the returned result.</param>
        /// <returns>The first substring between the last item of each <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function only returns the first substring. Use the <see cref="Between(string, string[], string[])"/> function if you want to return all substrings.
        /// </remarks>
        public static string FirstBetween(this string value, string[] leading, string[] enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns the substring between the all occurance of the specified strings, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">The first substring to search in <paramref name="value"/>.</param>
        /// <param name="enclosing">The second substring to search in <paramref name="value"/>.</param>
        /// <returns>All substrings between the occurances of <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string[] BetweenIgnoreCase(this string value, string leading, string enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns all substrings between the occuranceS of the specified string sequences, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">An array of substrings in <paramref name="value"/> that precedes each item in the returned result.</param>
        /// <param name="enclosing">An array of substrings in <paramref name="value"/> that follows each item in the returned result.</param>
        /// <returns>All substrings between the last item of each <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string[] BetweenIgnoreCase(this string value, string[] leading, string[] enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the substring between the first occurance of the specified strings, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">The first substring to search in <paramref name="value"/>.</param>
        /// <param name="enclosing">The second substring to search in <paramref name="value"/>.</param>
        /// <returns>The substring between <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string FirstBetweenIgnoreCase(this string value, string leading, string enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the first substring between the first occurance of the specified string sequences, using <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">An array of substrings in <paramref name="value"/> that precedes each item in the returned result.</param>
        /// <param name="enclosing">An array of substrings in <paramref name="value"/> that follows each item in the returned result.</param>
        /// <returns>The first substring between the last item of each <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function only returns the first substring. Use the <see cref="Between(string, string[], string[])"/> function if you want to return all substrings.
        /// </remarks>
        public static string FirstBetweenIgnoreCase(this string value, string[] leading, string[] enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the substring between all occurance of the specified strings.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">The first substring to search in <paramref name="value"/>.</param>
        /// <param name="enclosing">The second substring to search in <paramref name="value"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="leading"/> and <paramref name="enclosing"/> are searched in <paramref name="value"/>.</param>
        /// <returns>All substrings between the occurances of <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string[] Between(this string value, string leading, string enclosing, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(leading) && string.IsNullOrEmpty(enclosing))
                return new string[] { value };
            else if (string.IsNullOrEmpty(leading))
                return new string[] { value.Before(leading, comparisonType) };
            else if (string.IsNullOrEmpty(enclosing))
                return new string[] { value.After(enclosing, comparisonType) };
            else
                return BetweenInternal(value, new string[] { leading }, new string[] { enclosing }, false, comparisonType).ToArray();
        }

        /// <summary>
        /// Returns all substrings between the occuranceS of the specified string sequences.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">An array of substrings in <paramref name="value"/> that precedes each item in the returned result.</param>
        /// <param name="enclosing">An array of substrings in <paramref name="value"/> that follows each item in the returned result.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="leading"/> and <paramref name="enclosing"/> are searched in <paramref name="value"/>.</param>
        /// <returns>All substrings between the last item of each <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        /// <remarks>
        /// Consider the following string: `a~`b=-c+++hello#!#@x^y$__z$^`.
        /// 
        /// We can see that the text `hello` is preceded by `a`, `b` and `c`, and followed by `x`, `y`, and `z`. We can use the <see cref="Between(string, string[], string[])"/> function to extract 
        /// the `hello` substring:
        /// ```C#
        /// string rawText = "a~`b=-c+++hello#!#@x^y$__z$^";
        /// string[] message = rawText.Between(new string[] { "a", "b", "c" }, new string[] { "x", "y", "z" });
        /// Console.WriteLine(message[0]);
        /// ```
        /// </remarks>
        public static string[] Between(this string value, string[] leading, string[] enclosing, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (leading == null || leading.Length == 0)
                throw new ArgumentNullException(nameof(leading));

            if (enclosing == null || enclosing.Length == 0)
                throw new ArgumentNullException(nameof(enclosing));

            return BetweenInternal(value, leading, enclosing, false, comparisonType).ToArray();
        }

        /// <summary>
        /// Returns the first substring between the first occurance of the specified string sequences.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">An array of substrings in <paramref name="value"/> that precedes each item in the returned result.</param>
        /// <param name="enclosing">An array of substrings in <paramref name="value"/> that follows each item in the returned result.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="leading"/> and <paramref name="enclosing"/> are searched in <paramref name="value"/>.</param>
        /// <returns>The first substring between the last item of each <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        /// <remarks>
        /// This function only returns the first substring. Use the <see cref="Between(string, string[], string[])"/> function if you want to return all substrings.
        /// </remarks>
        public static string FirstBetween(this string value, string[] leading, string[] enclosing, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (leading == null || leading.Length == 0)
                throw new ArgumentNullException(nameof(leading));

            if (enclosing == null || enclosing.Length == 0)
                throw new ArgumentNullException(nameof(enclosing));

            return BetweenInternal(value, leading, enclosing, true, comparisonType)[0];
        }

        /// <summary>
        /// Returns the substring between the first occurance of the specified strings.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object instance.</param>
        /// <param name="leading">The first substring to search in <paramref name="value"/>.</param>
        /// <param name="enclosing">The second substring to search in <paramref name="value"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="leading"/> and <paramref name="enclosing"/> are searched in <paramref name="value"/>.</param>
        /// <returns>The substring between <paramref name="leading"/> and <paramref name="enclosing"/> in <paramref name="value"/>.</returns>
        public static string FirstBetween(this string value, string leading, string enclosing, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrEmpty(leading) && string.IsNullOrEmpty(enclosing))
                return value;
            else if (string.IsNullOrEmpty(leading))
                return value.Before(leading, comparisonType);
            else if (string.IsNullOrEmpty(enclosing))
                return value.After(enclosing, comparisonType);
            else
                return BetweenInternal(value, new string[] { leading }, new string[] { enclosing }, true, comparisonType)[0];
        }

        internal static List<string> BetweenInternal(string value, string[] leading, string[] enclosing, bool getFirst, StringComparison comparisonType)
        {
            List<string> output = new List<string>();

            int currentIndex = 0;
            int foundIndex = 0;
            int x, y = -1;

            while (true)
            {
                // find start tags
                for (int i = 0; i < leading.Length; i++)
                {
                    foundIndex = value.IndexOf(leading[i], currentIndex, comparisonType);
                    if (foundIndex == -1)
                        break;
                    else
                        currentIndex = foundIndex + leading[i].Length;
                }

                if (foundIndex == -1)
                    return output;
                else
                    x = currentIndex;

                // find end tags
                for (int i = 0; i < enclosing.Length; i++)
                {
                    foundIndex = value.IndexOf(enclosing[i], currentIndex, comparisonType);
                    if (foundIndex == -1)
                        break;
                    else
                        currentIndex = foundIndex + enclosing[i].Length;

                    // exit if not all start tags found, else get target start position
                    if (foundIndex == -1)
                        return output;
                    else
                        y = value.IndexOf(enclosing[0], x, comparisonType);
                }

                // add to list
                output.Add(value.Substring(x, y - x));
                if (getFirst)
                    return output;                
            }
        }
    }
}
