using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Standard
{
    /// <summary>
    /// Common extensions for the <see cref="IList{T}"/> class.
    /// </summary>
    public static class IListExtension
    {
        /// <summary>
        /// Adds a list of elements to a <see cref="IList{T}"/> object.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="list">An instance of <see cref="IList{T}"/>.</param>
        /// <param name="elements">The elements to add to <paramref name="list"/>.</param>
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

        /// <summary>
        /// Returns an element at the specified index.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="list">An instance of <see cref="IList{T}"/>.</param>
        /// <param name="index">The index position of the item in the collection.</param>
        /// <param name="defaultValue">The value to return if the index position does not exist in the collection.</param>
        /// <returns>
        /// The element at the specified index, or <paramref name="defaultValue"/> if the index position does not exist.
        /// </returns>
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

        /// <summary>
        /// Remove the first items from an <see cref="IList{T}"/> object.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="list">An instance of <see cref="IList{T}"/>.</param>
        /// <returns>
        /// A new list which is the same as <paramref name="list"/> but with the first item removed.
        /// </returns>
        public static IList<T> RemoveStart<T>(this IList<T> list)
        {
            return RemoveStart<T>(list, 1);
        }

        /// <summary>
        /// Remove a number of items from an <see cref="IList{T}"/> object, starting from the first index position.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="list">An instance of <see cref="IList{T}"/>.</param>
        /// <param name="count">The number of items to remove.</param>
        /// <returns>
        /// A new list which is the same as <paramref name="list"/> but with the specified number of items removed from the starting index position.
        /// </returns>
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
