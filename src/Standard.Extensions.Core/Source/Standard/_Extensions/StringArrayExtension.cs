using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Standard
{
    public static class StringArrayExtension
    {
        public static IEnumerable<string> NormalizeNewLines(this IEnumerable<string> input)
        {
            foreach (string line in input)
            {
                yield return StringExtension.NormalizeNewLine(line);
            }
        }

        public static string[] NormalizeNewLines(this string[] input)
        {
            return NormalizeNewLines((IEnumerable<string>)input).ToArray();
        }

        public static string[] RemoveNull(this string[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return value.Where(s => !(s == null)).ToArray();
        }

        public static string[] RemoveEmpty(this string[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return value.Where(s => !(s == string.Empty)).ToArray();
        }

        public static string[] RemoveNullOrEmpty(this string[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return value;

            return value.Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        public static string[] Unique(this string[] value)
        {
            return Unique(value, StringComparison.Ordinal);
        }

        public static string[] UniqueIgnoreCase(this string[] value)
        {           
            return Unique(value, StringComparison.OrdinalIgnoreCase);
        }

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
