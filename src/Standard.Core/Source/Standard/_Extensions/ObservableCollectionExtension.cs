using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Standard
{
    /// <summary>
    /// Common extensions for the <see cref="ObservableCollection{T}"/> class.
    /// </summary>
    public static class ObservableCollectionExtension
    {
        /// <summary>
        /// Adds a list of elements into an <see cref="ObservableCollection{T}"/> object.
        /// </summary>
        /// <typeparam name="T">The data type of each element in the collection.</typeparam>
        /// <param name="collection">The collection to modify.</param>
        /// <param name="items">The elements to be added to <paramref name="collection"/>.</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
                
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }
    }
}