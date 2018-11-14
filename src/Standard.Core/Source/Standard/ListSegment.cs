using System;
using System.Collections;
using System.Collections.Generic;

#if NETFX
using System.Security.Permissions;
#endif

using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Delimits a section of a one-dimensional list. This structure is similar to <see cref="ArraySegment{T}"/>, but 
    /// is designed to work with <see cref="IList{T}"/> instead of just arrays.
    /// </summary>
    /// <typeparam name="T">The type that is stored in the elements of the <see cref="IList{T}"/>.</typeparam>
#if NETFX
    [PermissionSet(SecurityAction.LinkDemand)]
    [PermissionSet(SecurityAction.InheritanceDemand)]
#endif
    public struct ListSegment<T> : IList<T>
    {
        private readonly IList<T> _list;
        private readonly int _offset;
        private readonly int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSegment{T}"/> structure that delimits all the elements in the specified list.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is `null`.</exception>
        public ListSegment(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _list = list;
            _offset = 0;
            _count = list.Count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSegment{T}"/> class. virtually.
        /// </summary>
        /// <param name="list">The list to be sliced.</param>
        /// <param name="offset">The offset into <paramref name="list"/> to start the slice.</param>
        /// <param name="count">The number of elements to be included in this slice.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is `null`.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> are less than zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="offset"/> and <paramref name="count"/> do not specify a valid range in in <paramref name="list"/>.</exception>
        public ListSegment(IList<T> list, int offset, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), string.Format(RS.Err_NumberNotGe, 0, offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), string.Format(RS.Err_NumberNotGe, 0, count));
            if (list.Count - offset < count)
                throw new ArgumentException(RS.Err_InvalidOffsetLength);

            _list = list;
            _offset = offset;
            _count = count;
        }

        /// <summary>
        /// Gets the original list containing the range of elements that the list segment delimits.
        /// </summary>
        public IList<T> List
        {
            get { return _list; }
        }

        /// <summary>
        /// Gets the position of the first element in the range delimited by the list segment, relative to the start of the original lsit.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// Returns the hash code for the current instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return null == _list
                ? 0
                : _list.GetHashCode() ^ _offset ^ _count;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to be compared with the current instance.</param>
        /// <returns>
        /// `true` if the specified object is a <see cref="ListSegment{T}"/> structure and is equal to the current instance; otherwise, `false`.
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (obj is ListSegment<T>)
                return Equals((ListSegment<T>)obj);
            else
                return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ListSegment{T}"/> structure is equal to the current instance.
        /// </summary>
        /// <param name="obj">The structure to compare with the current instance.</param>
        /// <returns>
        /// `true` if the specified <see cref="ListSegment{T}"/> structure is equal to the current instance; otherwise, `false`.
        /// </returns>
        public bool Equals(ListSegment<T> obj)
        {
            return obj._list == _list && obj._offset == _offset && obj._count == _count;
        }

        /// <summary>
        /// Indicates whether two <see cref="ListSegment{T}"/> structures are equal.
        /// </summary>
        /// <param name="a">The structure on the left side of the equality operator.</param>
        /// <param name="b">The structure on the right side of the equality operator.</param>
        /// <returns>
        /// `true` if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, `false`.
        /// </returns>
        public static bool operator ==(ListSegment<T> a, ListSegment<T> b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Indicates whether two <see cref="ListSegment{T}"/> structures are unequal.
        /// </summary>
        /// <param name="a">The structure on the left side of the inequality operator.</param>
        /// <param name="b">The structure on the right side of the inequality operator.</param>
        /// <returns>
        /// `true` if <paramref name="a"/> is not equal to <paramref name="b"/>; otherwise, `false`.
        /// </returns>
        public static bool operator !=(ListSegment<T> a, ListSegment<T> b)
        {
            return !(a == b);
        }

        #region IList<T>

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="ListSegment{T}"/>.</exception>
        /// <exception cref="InvalidOperationException">This object instance is `null`.</exception>
        /// <value>
        /// The element at the specified index.
        /// </value>
        public T this[int index]
        {
            get
            {
                if (_list == null)
                    throw new InvalidOperationException(RS.Err_ArrayIsNull);

                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _list[_offset + index];
            }
            set
            {
                if (_list == null)
                    throw new InvalidOperationException(RS.Err_ArrayIsNull);

                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                _list[_offset + index] = value;
            }
        }

        /// <summary>
        /// Determines the index of a specific item in the list segment.
        /// </summary>
        /// <param name="item">The object to locate in the list segment.</param>
        /// <exception cref="InvalidOperationException">This object instance is `null`.</exception>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            if (_list == null)
                throw new InvalidOperationException(RS.Err_ArrayIsNull);

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(item)) 
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Inserts an item into the list segment at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the list segment.</param>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="ListSegment{T}"/>.</exception>
        /// <remarks>
        /// This method is not implemented in <see cref="ListSegment{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the list segment item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="ListSegment{T}"/>.</exception>
        /// <remarks>
        /// This method is not implemented in <see cref="ListSegment{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<T>

        /// <summary>
        /// Gets the number of elements in the range delimited by the list segment.
        /// </summary>
        /// <value>
        /// The number of elements in the range delimited by the list segment.
        /// </value>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets a value that indicates whether the list segment is read-only.
        /// </summary>
        /// <value>
        /// `true` if the <see cref="ListSegment{T}"/> is read-only; otherwise, `false`.
        /// </value>
        /// <remarks>
        /// This property will always return `true`.
        /// </remarks>
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                // the indexer setter does not throw an exception although IsReadOnly is true.
                // This is to match the behavior of arrays.
                return true;
            }
        }

        /// <summary>
        /// Adds an item to the list segment.
        /// </summary>
        /// <param name="item">The object to add to the list segment.</param>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="ListSegment{T}"/>.</exception>
        /// <remarks>
        /// This method is not implemented in <see cref="ListSegment{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the list segment.
        /// </summary>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="ListSegment{T}"/>.</exception>
        /// <remarks>
        /// This method is not implemented in <see cref="ListSegment{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the list segment contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the list segment.</param>
        /// <exception cref="InvalidOperationException">This object instance is `null`.</exception>
        /// <returns>
        /// `true` if <paramref name="item"/> is found in the list segment; otherwise, `false`.
        /// </returns>
        bool ICollection<T>.Contains(T item)
        {
            if (_list == null)
                throw new InvalidOperationException(RS.Err_ArrayIsNull);

            return IndexOf(item) >= 0;
        }

        /// <summary>
        /// Copies the elements of the list segment to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the list segment. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="ListSegment{T}"/>.</exception>
        /// <remarks>
        /// This method is not implemented in <see cref="ListSegment{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list segment.
        /// </summary>
        /// <param name="item">The object to remove from the list segment.</param>
        /// <exception cref="NotImplementedException">This method is not implemented in <see cref="ListSegment{T}"/>.</exception>
        /// <returns>
        /// This method is not implemented in <see cref="ListSegment{T}"/>. Calling this method will always result in a <see cref="NotImplementedException"/>.
        /// </returns>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<T>

        /// <summary>
        /// Returns an enumerator that iterates through the list segment.
        /// </summary>
        /// <exception cref="InvalidOperationException">This object instance is `null`, the operation has not started, or the operation has already ended.</exception>
        /// <returns>
        /// An enumerator that can be used to iterate through the list segment.
        /// </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_list == null)
                throw new InvalidOperationException(RS.Err_ArrayIsNull);

            return new ListSegmentEnumerator(this);
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the list segment.
        /// </summary>
        /// <exception cref="InvalidOperationException">This object instance is `null`, the operation has not started, or the operation has already ended.</exception>
        /// <returns>
        /// An enumerator that can be used to iterate through the list segment.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_list == null)
                throw new InvalidOperationException(RS.Err_ArrayIsNull);

            return new ListSegmentEnumerator(this);
        }

        #endregion

#if !NETSTANDARD
        [Serializable]
#endif
        private sealed class ListSegmentEnumerator : IEnumerator<T>
        {
            private IList<T> _array;
            private int _start;
            private int _end;
            private int _current;

            internal ListSegmentEnumerator(ListSegment<T> arraySlice)
            {
                _array = arraySlice._list;
                _start = arraySlice._offset;
                _end = _start + arraySlice._count;
                _current = _start - 1;
            }

            public bool MoveNext()
            {
                if (_current < _end)
                {
                    _current++;
                    return _current < _end;
                }
                return false;
            }

            public T Current
            {
                get
                {
                    if (_current < _start) 
                        throw new InvalidOperationException(RS.EnumeratorNotStarted);
                    if (_current >= _end) 
                        throw new InvalidOperationException(RS.EnumeratorEnded);

                    return _array[_current];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                _current = _start - 1;
            }

            public void Dispose()
            {
            }
        }
    }
}