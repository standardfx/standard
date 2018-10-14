using System;
using System.Linq;

namespace Standard
{
    /// <summary>
    /// Extension methods for arrays of the <see cref="Char"/> class.
    /// </summary>
    public static class CharArrayExtension
    {
        /// <summary>
        /// @ref <see cref="Unique(char[], StringComparison)"/>
        /// </summary>
        public static char[] Unique(this char[] value)
        {
            return Unique(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Ensures that each item in the character array is unique. Upper and lower cases are considered as the same character. @ref <see cref="Unique(char[], StringComparison)"/>
        /// </summary>
        public static char[] UniqueIgnoreCase(this char[] value)
        {           
            return Unique(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Ensures that each item in the character array is unique.
        /// </summary>
        /// <param name="value">An array of characters.</param>
        /// <param name="comparisonType">Specifies how characters are compared.</param>
        /// <returns>
        /// The array specified in <paramref name="value"/>, except that each repeated character is removed.
        /// </returns>
        public static char[] Unique(this char[] value, StringComparison comparisonType)
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
                return value.Select(c => c.ToString()).Distinct(comparer).Select(c => c[0]).ToArray();
            }
        }

        /// <summary>
        /// @ref <see cref="Join(char[], string)"/>
        /// </summary>
        public static string Join(this char[] value)
        {
            return Join(value, null);
        }

        /// <summary>
        /// @ref <see cref="Join(char[], string)"/>
        /// </summary>
        public static string Join(this char[] value, char separator)
        {
            return Join(value, separator.ToString());
        }

        /// <summary>
        /// Joins an character array into a string.
        /// </summary>
        /// <param name="value">A character array.</param>
        /// <param name="separator">Insert a separater between each item of the character array.</param>
        /// <returns>
        /// A string obtained by concating all characters in <paramref name="value"/> and separator.
        /// </returns>
        public static string Join(this char[] value, string separator)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return string.Empty;

            if (string.IsNullOrEmpty(separator))
                return new string(value);
                        
            return string.Join(separator, value.Select(x => x.ToString()).ToArray());
        }

        /// <summary>
        /// @ref <see cref="Contains(char[], char[], StringComparison)"/>
        /// </summary>
        public static bool Contains(this char[] value, params char[] member)
        {
            return Contains(value, member, StringComparison.Ordinal);
        }

        /// <summary>
        /// Tests whether the characters exist in a character array. Upper and lower cases are considered the same character. @ref <see cref="Contains(char[], char[], StringComparison)"/>
        /// </summary>
        public static bool ContainsIgnoreCase(this char[] value, params char[] member)
        {
            return Contains(value, member, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests whether the characters exist in a character array.
        /// </summary>
        /// <param name="value">The character array to be tested.</param>
        /// <param name="member">The items that should exist in the character array specified by <paramref name="value"/>.</param>
        /// <param name="comparisonType">Specifies how characters are compared.</param>
        /// <returns>
        /// `true` if each item in <paramref name="member"/> exists within <paramref name="value"/>; otherwise, `false`.
        /// </returns>
        public static bool Contains(this char[] value, char[] member, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (member == null || member.Length == 0)
                throw new ArgumentNullException(nameof(member));

            if (value.Length < member.Length)
                return false;

            return Array.TrueForAll(member, m => {
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
