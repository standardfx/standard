using System;
using System.Collections;
using System.Collections.Generic;

namespace Standard.Collections.Generic
{
    /// <summary>
    /// Represents an add-only collection of elements that can be accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the add-only list.</typeparam>
    /// <remarks>
    /// The <see cref="AddOnlyList{T}"/> represents a list in which the list elements can be sequentially added. This implies that insertion is only 
    /// possible at the last index position, and elements cannot be removed once added. 
    /// 
    /// The content of list elements is not guaranteed to behave similarly.
    /// 
    /// The <see cref="AddOnlyList{T}"/> is most suitable when data copying and memory fragmentations are unacceptable.
    /// </remarks>
    public class AddOnlyList<T> : IList<T>
    {
        // Passed in allocator.  We use to this get "memory" where the actual data is stored. Can be any IList'T
        private readonly Func<int, IList<T>> _allocator;

        // Backing field for Count.  Contains the number of elements in the list from the user's perspective.
        private int _count;

        // This is where all the allocations are stored.  The 1st bucket is a special case 
        // and represents the first 3 elements of the list.
        // The 2nd bucket contains the next 4 element of the list.
        // The 3rd bucket contains the next 8 element of the list.
        // The 4th bucket contains the next 16 element of the list.
        // The 5th bucket contains the next 32 element of the list.
        // etc.
        //
        // This way, given the index into the list, we can calculate which bucket it belongs to using log(i)/log(2)
        // This array is initially allocated to contain only one bucket. But a hint can be passed in to the constructor
        // to pre-allocate more pubckets. This will reduce reallocating the array as it grows.
        private IList<T>[] _buckets;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOnlyList{T}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public AddOnlyList()
            : this(null, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOnlyList{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public AddOnlyList(int capacity)
            : this(null, capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOnlyList{T}"/> class that is empty and has the specified custom allocator.
        /// </summary>
        /// <param name="allocator">A function that returns an <see cref="IList{T}"/> of the indicated size.</param>
        public AddOnlyList(Func<int, IList<T>> allocator)
            : this(allocator, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOnlyList{T}"/> class.
        /// </summary>
        /// <param name="allocator">A function that returns an <see cref="IList{T}"/> of the indicated size.</param>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <remarks>
        /// The <paramref name="allocator"/> is used to specify a custom allocation function. It should return an 
        /// <see cref="IList{T}"/> of the indicated size.
        /// 
        /// A simple implementation will look something like this:
        /// ```C#
        /// size => new int[size]
        /// ```
        /// 
        /// The <paramref name="capacity"/> parameter allows us to preallocate the buckets based on what the
        /// the final capacity might be. This does not allocate the entire list.
        /// 
        /// For instance, if <paramref name="capacity"/> is 1 million, an array of 18 objects are allocated.
        /// 
        /// Guessing a smaller number simply means that the buckets are reallocated as the array grows, causing extra work  
        /// on the GC.
        /// </remarks>
        public AddOnlyList(Func<int, IList<T>> allocator, int capacity)
        {
            _allocator = allocator ?? (size => new T[size]);
            _buckets = new IList<T>[Math.Max(GetBucketIndex(capacity), 1)];
        }

        #region IList{T} Implements

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="AddOnlyList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="AddOnlyList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item" /> if found within the entire <see cref="AddOnlyList{T}"/>; 
        /// otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Inserts an element into the <see cref="AddOnlyList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="AddOnlyList{T}"/>.</exception>
        /// <remarks>
        /// This method is not implemented in <see cref="AddOnlyList{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="AddOnlyList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="AddOnlyList{T}"/>.</exception>
        /// <remarks>
        /// This method is not implemented in <see cref="AddOnlyList{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <value>The element at the specified index.</value>
        public T this[int index]
        {
            get
            {
                IList<T> bucket;
                int localIndex = GetLocalIndex(index, out bucket);
                return bucket[localIndex];
            }
            set
            {
                IList<T> bucket;
                int localIndex = GetLocalIndex(index, out bucket);
                bucket[localIndex] = value;
            }
        }

        #endregion // IList{T} Implements

        #region ICollection{T} Implements

        /// <summary>
        /// Gets the number of elements contained in the <see cref="AddOnlyList{T}"/>.
        /// </summary>
        /// <value>
        /// The number of elements contained in the <see cref="AddOnlyList{T}"/>.
        /// </value>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="AddOnlyList{T}" /> is read-only.
        /// </summary>
        /// <value>
        /// `true` if the <see cref="AddOnlyList{T}"/> is read-only; otherwise, `false`.
        /// </value>
        /// <remarks>
        /// This property always returns `false`.
        /// </remarks>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="AddOnlyList{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="AddOnlyList{T}"/>. The value can be `null` for reference types.</param>
        public void Add(T item)
        {
            int indexNewEntry = _count;

            IList<T> bucket;
            int localIndex = GetLocalIndex(indexNewEntry, out bucket);
            bucket[localIndex] = item;

            _count = indexNewEntry + 1;
        }

        /// <summary>
        /// Removes all elements from the <see cref="AddOnlyList{T}"/>.
        /// </summary>
        public void Clear()
        {
            _count = 0;
            _buckets = new IList<T>[1];
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="AddOnlyList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="AddOnlyList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// `true` if <paramref name="item"/> is found in the <see cref="AddOnlyList{T}"/>; otherwise, `false`.
        /// </returns>
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        /// <see cref="ICollection{T}.CopyTo(T[], int)"/>
        /// <summary>
        /// Copies the entire <see cref="AddOnlyList{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="AddOnlyList{T}"/>. 
        /// The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < _count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="AddOnlyList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="AddOnlyList{T}"/>. The value can be `null` for reference types.</param>
        /// <exception cref="NotSupportedException">This method is not implemented in <see cref="AddOnlyList{T}"/>.</exception>
        /// <returns>
        /// This method is not implemented in <see cref="AddOnlyList{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </returns>
        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion // ICollection{T} Implements

        #region IEnumerable{T} Implements

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the list.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return this[i];
            }
        }

        #endregion // IEnumerable{T} Implements

        #region IEnumerable Implements

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the list.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion // IEnumerable Implements

        /// <summary>
        /// Returns the index position of the internal bucket that is storing the element specified.
        /// </summary>
        /// <param name="index">The zero-based index of an element in the collection.</param>
        /// <returns></returns>
        public static int GetBucketIndex(int index)
        {
            return Math.Max((int)(Math.Log(index + 1) / Math.Log(2)) - 1, 0);
        }

        /// <summary>
        /// Given the index into the list, returns the index into the bucket where the actual element resides.
        /// along with the bucket itself.
        /// </summary>
        /// <param name="globalIndex"></param>
        /// <param name="bucket"></param>
        /// <returns></returns>
        private int GetLocalIndex(int globalIndex, out IList<T> bucket)
        {
            int bucketIndex = GetBucketIndex(globalIndex);
            bucket = GetBucket(bucketIndex) ?? (_buckets[bucketIndex] = _allocator(Math.Max(3, globalIndex + 1)));
            return globalIndex - (bucketIndex > 0 ? (int)Math.Pow(2, bucketIndex + 1) - 1 : 0);
        }

        /// <summary>
        /// Given the bucket number, returns the bucket entry, which may be `null` if it hasn't
        /// been allocated yet.
        /// </summary>
        /// <param name="bucketIndex"></param>
        /// <returns></returns>
        private IList<T> GetBucket(int bucketIndex)
        {
            if (bucketIndex >= _buckets.Length)
            {
                IList<T>[] newBuckets = new IList<T>[_buckets.Length + 1];
                _buckets.CopyTo(newBuckets, 0);
                _buckets = newBuckets;
            }
            return _buckets[bucketIndex];
        }
    }
}
