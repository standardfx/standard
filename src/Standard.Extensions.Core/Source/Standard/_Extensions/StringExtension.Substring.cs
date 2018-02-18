using System;
using System.Collections.Generic;
using System.Globalization;

namespace Standard
{
    partial class StringExtension
    {
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

        public static string Before(this string value, string substring)
        {
            return Before(value, substring, StringComparison.Ordinal);
        }

        public static string BeforeIgnoreCase(this string value, string substring)
        {
            return Before(value, substring, StringComparison.OrdinalIgnoreCase);
        }

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

        public static string After(this string value, string substring)
        {
            return After(value, substring, StringComparison.Ordinal);
        }

        public static string AfterIgnoreCase(this string value, string substring)
        {
            return After(value, substring, StringComparison.OrdinalIgnoreCase);
        }

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

        public static string[] Between(this string value, string leading, string enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.Ordinal);
        }

        public static string[] Between(this string value, string[] leading, string[] enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.Ordinal);
        }

        public static string FirstBetween(this string value, string leading, string enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.Ordinal);
        }

        public static string FirstBetween(this string value, string[] leading, string[] enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.Ordinal);
        }

        public static string[] BetweenIgnoreCase(this string value, string leading, string enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

        public static string[] BetweenIgnoreCase(this string value, string[] leading, string[] enclosing)
        {
            return Between(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

        public static string FirstBetweenIgnoreCase(this string value, string leading, string enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

        public static string FirstBetweenIgnoreCase(this string value, string[] leading, string[] enclosing)
        {
            return FirstBetween(value, leading, enclosing, StringComparison.OrdinalIgnoreCase);
        }

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
