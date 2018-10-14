using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Core;

namespace Standard.Collections.Graphs
{
    // https://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp

    /// <summary>
    /// Enumerates the elements of an enumerable list of <typeparamref name="TItem"/> object.
    /// </summary>
    /// <typeparam name="TItem">The type of a member of the enumerable list to sort.</typeparam>
    /// <typeparam name="TKey">The type of the unique key for each <typeparamref name="TItem"/> item.</typeparam>
    public class TopoSortEnumerator<TItem, TKey> : IEnumerator<TItem>
    {
	    private readonly IEnumerator<TItem> source;
	    private readonly Func<TItem, TKey> getKey;
	    private readonly Func<TItem, IEnumerable<TKey>> getDependencies;
	    private readonly HashSet<TKey> sortedItems;
	    private readonly Queue<TItem> readyToOutput;
	    private readonly DependencyWaitList<TItem, TKey> waitList = new DependencyWaitList<TItem, TKey>();

	    private TItem current;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopoSortEnumerator{TItem, TKey}"/> class.
        /// </summary>
        /// <param name="source">A list of items to sort.</param>
        /// <param name="getKey">Function to obtain the key of an item in the <paramref name="source"/>.</param>
        /// <param name="getDependencies">Function to obtain the dependencies of an item in the <paramref name="source"/>.</param>
        public TopoSortEnumerator(IEnumerable<TItem> source, Func<TItem, TKey> getKey, Func<TItem, IEnumerable<TKey>> getDependencies)
        {
            this.source = source.GetEnumerator();
            this.getKey = getKey;
            this.getDependencies = getDependencies;

            readyToOutput = new Queue<TItem>();
            sortedItems = new HashSet<TKey>();
        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        public TItem Current
        {
            get { return current; }
        }

        /// <summary>
        /// Releases all resources used by a <see cref="TopoSortEnumerator{TItem, TKey}"/> object.
        /// </summary>
        public void Dispose()
        {
            source.Dispose();
        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the <see cref="TopoSortEnumerator{TItem, TKey}"/> collection.
        /// </summary>
        /// <returns>
        /// `true` if the enumerator was successfully advanced to the next element; `false` if the enumerator has passed the end of the collection.
        /// </returns>
		public bool MoveNext()
		{
		    while (true)
		    {
		        if (readyToOutput.Count > 0)
		        {
		            current = readyToOutput.Dequeue();
		            Release(current);
		            return true;
		        }

		        if (!source.MoveNext())
		            break;

		        Process(source.Current);
		    }

		    if (waitList.Count > 0)
		        throw new ArgumentException(RS.CyclicOrMissingDependecy);

		    return false;
		}

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            source.Reset();
            sortedItems.Clear();
            readyToOutput.Clear();
            current = default(TItem);
        }

        private void Process(TItem item)
        {
            var pendingDependencies = getDependencies(item)
                .Where(key => !sortedItems.Contains(key))
                .ToArray();

            if (pendingDependencies.Length > 0)
                waitList.Add(item, pendingDependencies);
            else
                readyToOutput.Enqueue(item);
        }

        private void Release(TItem item)
        {
            var key = getKey(item);
            sortedItems.Add(key);

            var releasedItems = waitList.Remove(key);
            if (releasedItems != null)
            {
                foreach (var releasedItem in releasedItems)
                {
                    readyToOutput.Enqueue(releasedItem);
                }
            }
        }
    }
}