using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standard.Collections.Graphs
{
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

		public static IEnumerable<TItem> TopoSort<TItem, TKey>(IEnumerable<TItem> source, Func<TItem, TKey> getKey, Func<TItem, IEnumerable<TKey>> getDependencies)
		{
			var enumerator = new TopoSortEnumerator<TItem, TKey>(source, getKey, getDependencies);
			return new DummyEnumerable<TItem>(() => enumerator);
		}

		public static IEnumerable<DependencyItem<T>> TopoSort<T>(IEnumerable<DependencyItem<T>> source)
		{
			return TopoSort(source, x => x.Name, y => y.Dependencies);
		}

		public static IEnumerable<List<T>> TopoSort<T>(IEnumerable<List<T>> source)
		{
			return TopoSort(source, x => x[0], y => y.GetRange(1, y.Count - 1));
		}
	}	
}