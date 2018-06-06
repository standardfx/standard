using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Standard.Core;

namespace Standard
{
    public static class CharArrayExtension
    {
        public static char[] Unique(this char[] value)
        {
            return Unique(value, StringComparison.Ordinal);
        }

        public static char[] UniqueIgnoreCase(this char[] value)
        {           
            return Unique(value, StringComparison.OrdinalIgnoreCase);
        }

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

        public static string Join(this char[] value)
        {
            return Join(value, null);
        }

        public static string Join(this char[] value, char separator)
        {
            return Join(value, separator.ToString());
        }

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

        public static bool Contains(this char[] value, params char[] member)
        {
            return Contains(value, member, StringComparison.Ordinal);
        }

        public static bool ContainsIgnoreCase(this char[] value, params char[] member)
        {
            return Contains(value, member, StringComparison.OrdinalIgnoreCase);
        }

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
