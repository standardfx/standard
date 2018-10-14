using System;
using System.Collections.Generic;

namespace Standard.Collections.Concurrent
{
    /// <summary>
    /// Thread-safe implementation of the <see cref="SortedList{TKey, TValue}" /> class.
    /// </summary>
    /// <remarks>
    /// Unless stated otherwise, refer to the documentation for <see cref="SortedList{TKey, TValue}" />.
    /// </remarks>
    public class ConcurrentSortedList<TKey, TValue>
    {
        private object _lock = new object();
        private SortedList<TKey, TValue> _list = new SortedList<TKey, TValue>();

        /// <summary>
        /// Gets the number of elements contained in a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        public int Count
        {
            get 
            { 
                lock (_lock) 
                    return _list.Count; 
            }
        }


        /// <summary>
        /// Gets and sets the value associated with a specific key in a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="key">The key associated with the value to get or set.</param>
        public TValue this[TKey key]
        {
            get
            {
                lock (_lock)
                    return _list[key];
            }
            set
            {
                lock (_lock)
                    _list[key] = value;
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value to a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be `null`.</param>
        public void Add(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (_list.ContainsKey(key) == false)
                    _list.Add(key, value);
                else
                    _list[key] = value;
            }
        }

        /// <summary>
        /// Removes the element with the specified key from a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(TKey key)
        {
            if (key == null)
                return;

            lock (_lock)
                _list.Remove(key);
        }

        /// <summary>
        /// Gets the key at the specified index of a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="index">The zero-based index of the key to get.</param>
        /// <returns>
        /// The key at the specified index of the <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </returns>
        public TKey GetKey(int index)
        {
            lock (_lock) 
                return _list.Keys[index];
        }

        /// <summary>
        /// Gets the value at the specified index of a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>
        /// The value at the specified index of the <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </returns>
        public TValue GetValue(int index)
        {
            lock (_lock)
                return _list.Values[index];
        }

        /// <summary>
        /// Gets the keys in a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        /// <returns>
        /// An array of keys in the <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </returns>
        public TKey[] Keys()
        {
            lock (_lock)
            {
                TKey[] keys = new TKey[_list.Keys.Count];
                _list.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        /// <summary>
        /// Returns an `IEnumerator` object that iterates through a <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </summary>
        /// <returns>
        /// An `IEnumerator` object for the <see cref="ConcurrentSortedList{TKey, TValue}" /> object.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_list).GetEnumerator();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the `value` parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// `true` if the <see cref="ConcurrentSortedList{TKey, TValue}" /> object contains an element with the specified key; otherwise, `false`.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lock)
                return _list.TryGetValue(key, out value);
        }
    }
}
