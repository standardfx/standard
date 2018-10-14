using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Standard
{
    /// <summary>
    /// Extension methods for string arrays or enumerable collection of strings.
    /// </summary>
    public static class StringArrayExtension
    {
        /// <summary>
        /// Run the <see cref="StringExtension.NormalizeNewLine(string)"/> on each item in an enumerable string collection.
        /// </summary>
        /// <param name="input">An enumerable string collection.</param>
        /// <returns>An enumerable string collection with all newline variants in each item replaced by the operating system's newline specification.</returns>
        public static IEnumerable<string> NormalizeNewLines(this IEnumerable<string> input)
        {
            foreach (string line in input)
            {
                yield return StringExtension.NormalizeNewLine(line);
            }
        }

        /// <summary>
        /// Run the <see cref="StringExtension.NormalizeNewLine(string)"/> on each item in a string array.
        /// </summary>
        /// <param name="input">A string array.</param>
        /// <returns>A string array with all newline variants in each item replaced by the operating system's newline specification.</returns>
        public static string[] NormalizeNewLines(this string[] input)
        {
            return NormalizeNewLines((IEnumerable<string>)input).ToArray();
        }

        /// <summary>
        /// Removes all `null` items in a string array.
        /// </summary>
        /// <param name="value">A string array which may contain `null` items.</param>
        /// <returns>A string array which is the same as <paramref name="value"/> but with all `null` items removed.</returns>
        public static string[] RemoveNull(this string[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return value.Where(s => !(s == null)).ToArray();
        }

        /// <summary>
        /// Removes all empty string items in a string array.
        /// </summary>
        /// <param name="value">A string array which may contain zero length string items.</param>
        /// <returns>A string array which is the same as <paramref name="value"/> but with all empty string items removed.</returns>
        public static string[] RemoveEmpty(this string[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return value.Where(s => !(s == string.Empty)).ToArray();
        }

        /// <summary>
        /// Removes all `null` or empty string items in a string array.
        /// </summary>
        /// <param name="value">A string array which may contain `null` or zero length string items.</param>
        /// <returns>A string array which is the same as <paramref name="value"/> but with all `null` and empty string items removed.</returns>
        public static string[] RemoveNullOrEmpty(this string[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return value.Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        /// <summary>
        /// Removes all duplicate items in a string array.
        /// </summary>
        /// <param name="value">A string array which may contain duplicate items.</param>
        /// <returns>A string array with duplicated items removed.</returns>
        public static string[] Unique(this string[] value)
        {
            return Unique(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes all duplicate items in a string array. Strings with different casing are considered duplicates.
        /// </summary>
        /// <param name="value">A string array which may contain duplicate items.</param>
        /// <returns>A string array with duplicated items removed.</returns>
        public static string[] UniqueIgnoreCase(this string[] value)
        {           
            return Unique(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes all duplicate items in a string array.
        /// </summary>
        /// <param name="value">A string array which may contain duplicate items.</param>
        /// <param name="comparisonType">Specifies how duplication is defined.</param>
        /// <returns>A string array with duplicated items removed.</returns>
        public static string[] Unique(this string[] value, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length < 2)
                return value;

            if (comparisonType == StringComparison.Ordinal)
            {
                return value.Distinct().ToArray();
            }
            else
            {
                StringComparer comparer;
                if (comparisonType == StringComparison.OrdinalIgnoreCase)
                    comparer = StringComparer.OrdinalIgnoreCase;
                else if (comparisonType == StringComparison.CurrentCulture)
                    comparer = StringComparer.CurrentCulture;
                else if (comparisonType == StringComparison.CurrentCultureIgnoreCase)
                    comparer = StringComparer.CurrentCultureIgnoreCase;
#if !NETSTANDARD
                else if (comparisonType == StringComparison.InvariantCulture)
                    comparer = StringComparer.InvariantCulture;
                else if (comparisonType == StringComparison.InvariantCultureIgnoreCase)
                    comparer = StringComparer.InvariantCultureIgnoreCase;
#endif
                else
                    throw new ArgumentException(nameof(comparisonType));

                // #todo any way to improve perf?
                return value.Distinct(comparer).ToArray();
            }
        }

        /// <summary>
        /// Determines whether a string array contains the item specified.
        /// </summary>
        /// <param name="value">A string array.</param>
        /// <param name="member1">The string that should be a member of <paramref name="value"/>.</param>
        /// <param name="memberOther">Each other string that should be a member of <paramref name="value"/>.</param>
        /// <returns>`true` if all of <paramref name="member1"/> and <paramref name="memberOther"/> exists are items in <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool Contains(this string[] value, string member1, params string[] memberOther)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (member1 == null)
                throw new ArgumentNullException(nameof(member1));

            if (memberOther == null)
            {
                return Contains(value, new string[] { member1 }, StringComparison.Ordinal);
            }
            else
            {
                Array.Resize(ref memberOther, memberOther.Length + 1);
                memberOther[memberOther.Length - 1] = member1;
                return Contains(value, memberOther, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Determines whether a string array contains the item specified. Items of different casing are considered the equal.
        /// </summary>
        /// <param name="value">A string array.</param>
        /// <param name="member1">The string that should be a member of <paramref name="value"/>.</param>
        /// <param name="memberOther">Each other string that should be a member of <paramref name="value"/>.</param>
        /// <returns>`true` if all of <paramref name="member1"/> and <paramref name="memberOther"/> exists are items in <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool ContainsIgnoreCase(this string[] value, string member1, params string[] memberOther)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (member1 == null)
                throw new ArgumentNullException(nameof(member1));

            if (memberOther == null)
            {
                return Contains(value, new string[] { member1 }, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                Array.Resize(ref memberOther, memberOther.Length + 1);
                memberOther[memberOther.Length - 1] = member1;
                return Contains(value, memberOther, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Determines whether a string array contains the item specified.
        /// </summary>
        /// <param name="value">A string array.</param>
        /// <param name="member">A string array should be a subset of <paramref name="value"/>.</param>
        /// <param name="comparisonType">Specifies how equality is defined.</param>
        /// <returns>`true` if all of <paramref name="member"/> exists are items in <paramref name="value"/>. Otherwise, `false`.</returns>
        public static bool Contains(this string[] value, string[] member, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (member == null || member.Length == 0)
                throw new ArgumentNullException(nameof(member));

            return Array.TrueForAll(member, m => 
                {
                    if (comparisonType == StringComparison.Ordinal)
                    {
                        if (Array.IndexOf(value, m) == -1)
                            return false;
                    }
                    else
                    {
                        if (!Array.Exists(value, element => element.ToString().Equals(m.ToString(), comparisonType)))
                            return false;
                    }

                    return true;
                });
       }
    }
}
