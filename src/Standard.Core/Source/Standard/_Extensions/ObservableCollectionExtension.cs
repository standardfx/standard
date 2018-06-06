using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Standard
{
    public static class ObservableCollectionExtension
    {
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