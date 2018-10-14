using System;
using System.Collections;
using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Common extensions to the <see cref="Queue" /> class.
    /// </summary>
    public static class QueueExtension
    {
        /// <summary>
        /// Predicate for searching inside a queue.
        /// </summary>
        /// <param name="item">Item of the queue.</param>
        /// <returns>Result of predicate.</returns>
        public delegate bool QueuePredicate(object item);

        /// <summary>
        /// Returns the first item in a <see cref="Queue"/> that satisfies a specified condition.
        /// </summary>
        /// <param name="queue">A <see cref="Queue"/> to return an element from.</param>
        /// <param name="predicate">A predicate to test each element for a condition.</param>
        /// <returns>
        /// The first element in <paramref name="queue"/> that passes the test in the <paramref name="predicate"/> specified.
        /// </returns>
        public static object First(this Queue queue, QueuePredicate predicate)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (queue.Count == 0)
                throw new InvalidOperationException(RS.SequenceIsEmpty);

            foreach (var item in queue)
            {
                if (predicate(item))
                    return item;
            }

            throw new InvalidOperationException(RS.NoItemSatisfyPredicate);
        }

        /// <summary>
        /// Returns the first item in a <see cref="Queue"/> that satisfies a specified condition, or `null` if no item 
        /// can be found that satisfies such condition.
        /// </summary>
        /// <param name="queue">A <see cref="Queue"/> to return an element from.</param>
        /// <param name="predicate">A predicate to test each element for a condition.</param>
        /// <returns>
        /// The first element in <paramref name="queue"/> that passes the test in the <paramref name="predicate"/> specified, 
        /// or `null` if no item can be found that satisfies such condition.
        /// </returns>
        public static object FirstOrDefault(this Queue queue, QueuePredicate predicate)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (predicate == null)
                return null;

            if (queue.Count == 0)
                return null;

            foreach (var item in queue)
            {
                if (predicate(item))
                    return item;
            }

            return null;
        }
    }
}
