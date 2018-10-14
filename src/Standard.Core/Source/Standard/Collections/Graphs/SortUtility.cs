using System;
using System.Collections.Generic;

namespace Standard.Collections.Graphs
{
    /// <summary>
    /// A utility class to perform topological sorting.
    /// </summary>
    public static class SortUtility
	{
		private class DummyEnumerable<T> : IEnumerable<T>
		{
			private readonly Func<IEnumerator<T>> getEnumerator;

 			public DummyEnumerable(Func<IEnumerator<T>> getEnumerator)
			{
				this.getEnumerator = getEnumerator;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return getEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

        /// <summary>
        /// Sorts a list of objects in order of their dependencies.
        /// </summary>
        /// <typeparam name="TItem">The type of an item in the <paramref name="source"/> array.</typeparam>
        /// <typeparam name="TKey">The type of the unique key for each item in <typeparamref name="TItem"/>.</typeparam>
        /// <param name="source">An enumerable list of <typeparamref name="TItem"/> objects.</param>
        /// <param name="getKey">A function to get the key of a <typeparamref name="TItem"/> object in <paramref name="source"/>.</param>
        /// <param name="getDependencies">A function to get the dependencies of a <typeparamref name="TItem"/> object.</param>
        /// <returns>
        /// An enumerable list of <typeparamref name="TItem"/> objects, sorted in order of their dependencies.
        /// </returns>
        public static IEnumerable<TItem> TopoSort<TItem, TKey>(IEnumerable<TItem> source, Func<TItem, TKey> getKey, Func<TItem, IEnumerable<TKey>> getDependencies)
		{
			var enumerator = new TopoSortEnumerator<TItem, TKey>(source, getKey, getDependencies);
			return new DummyEnumerable<TItem>(() => enumerator);
		}

        /// <summary>
        /// Sorts a list of <see cref="DependencyItem{T}"/> objects in order of their dependencies.
        /// </summary>
        /// <typeparam name="T">The type of an item in the <paramref name="source"/> array.</typeparam>
        /// <param name="source">An enumerable list of <typeparamref name="T"/> objects.</param>
        /// <returns>
        /// An enumerable list of <see cref="DependencyItem{T}"/> objects, sorted in order of their dependencies.
        /// </returns>
		public static IEnumerable<DependencyItem<T>> TopoSort<T>(IEnumerable<DependencyItem<T>> source)
		{
			return TopoSort(source, x => x.Name, y => y.Dependencies);
		}

        /// <summary>
        /// Sorts a list of <see cref="List{T}"/> objects in order of their dependencies.
        /// </summary>
        /// <typeparam name="T">The type of an item inside each item of the <paramref name="source"/>.</typeparam>
        /// <param name="source">An enumerable list of <see cref="List{T}"/> objects.</param>
        /// <returns>
        /// An enumerable list of <see cref="List{T}"/> objects, sorted in order of their dependencies.
        /// </returns>
		public static IEnumerable<List<T>> TopoSort<T>(IEnumerable<List<T>> source)
		{
			return TopoSort(source, x => x[0], y => y.GetRange(1, y.Count - 1));
		}
	}	
}