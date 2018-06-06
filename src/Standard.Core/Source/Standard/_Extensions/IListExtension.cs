using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Standard
{
    public static class IListExtension
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> elements)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            foreach (T item in elements)
            {
                list.Add(item);
            }
        }

        public static T ElementAt<T>(this IList<T> list, int index, T defaultValue)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (defaultValue == null)
                throw new ArgumentNullException(nameof(defaultValue));

            return index >= 0 && index < list.Count 
                ? list[index] 
                : defaultValue;
        }

        public static IList<T> RemoveStart<T>(this IList<T> list)
        {
            return RemoveStart<T>(list, 1);
        }

        public static IList<T> RemoveStart<T>(this IList<T> list, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            IList<T> elements = list.Take(count).ToList();
            for (int i = 0; i < count; ++i)
            {
                list.RemoveAt(0);
            }
            return elements;
        }
    }
}
