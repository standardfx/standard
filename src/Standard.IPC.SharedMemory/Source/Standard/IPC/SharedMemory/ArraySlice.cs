using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Standard.IPC.SharedMemory
{
    /// <summary>
    /// Like <see cref="T:System.ArraySegment`1"/>, but works with <see cref="T:System.Collections.Generic.IList`1"/> not just an array.
    /// </summary>
    /// <typeparam name="T">The type that is stored in the elements of the <see cref="T:System.Collections.Generic.IList`1"/>.</typeparam>
    [PermissionSet(SecurityAction.LinkDemand)]
    [PermissionSet(SecurityAction.InheritanceDemand)]
    public struct ArraySlice<T> : IList<T>
    {
        private readonly IList<T> _list;
        private readonly int _offset;
        private readonly int _count;

        /// <summary>
        /// No slicing. Just mirror the list.
        /// </summary>
        /// <param name="list">The list to be sliced.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        public ArraySlice(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _list = list;
            _offset = 0;
            _count = list.Count;
        }

        /// <summary>
        /// Create a slice of a list (virtually).
        /// </summary>
        /// <param name="list">The list to be sliced.</param>
        /// <param name="offset">The offset into <paramref name="list"/> to start the slice.</param>
        /// <param name="count">The number of elements to be included in this slice.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> are less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in <paramref name="list"/> - <paramref name="offset"/> is not less than <paramref name="count"/>.</exception>
        public ArraySlice(IList<T> list, int offset, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), RS.MustBeGtNegativeOne);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), RS.MustBeGtNegativeOne);
            if (list.Count - offset < count)
                throw new ArgumentException(RS.InvalidOffsetLength);

            _list = list;
            _offset = offset;
            _count = count;
        }

        /// <summary>
        /// The list that is being sliced.
        /// </summary>
        public IList<T> List
        {
            get { return _list; }
        }

        /// <summary>
        /// The offset into the <see cref="T:ArraySlice`1.List"/>.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// The number of elements to be included in this slice.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Used to determine uniqueness.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return null == _list
                ? 0
                : _list.GetHashCode() ^ _offset ^ _count;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj is ArraySlice<T>)
                return Equals((ArraySlice<T>)obj);
            else
                return false;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public bool Equals(ArraySlice<T> obj)
        {
            return obj._list == _list && obj._offset == _offset && obj._count == _count;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="a">The first object being compared.</param>
        /// <param name="b">The second object being compared.</param>
        /// <returns>true if both objectscare equal, otherwise, false.</returns>
        public static bool operator ==(ArraySlice<T> a, ArraySlice<T> b)
        {
            return a.Equals(b);
        }

        /// <summary>Indicates whether this instance and a specified object are not equal.</summary>
        /// <param name="a">The first object being compared.</param>
        /// <param name="b">The second object being compared.</param>
        /// <returns>true if both objects are not equal, otherwise, false.</returns>
        public static bool operator !=(ArraySlice<T> a, ArraySlice<T> b)
        {
            return !(a == b);
        }

        #region IList<T>

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than 0 or has exceededthe number of items.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (_list == null)
                    throw new InvalidOperationException(RS.ArrayIsNull);

                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index");

                return _list[_offset + index];
            }
            set
            {
                if (_list == null)
                    throw new InvalidOperationException(RS.ArrayIsNull);

                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index");

                _list[_offset + index] = value;
            }
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.</summary>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public int IndexOf(T item)
        {
            if (_list == null)
                throw new InvalidOperationException(RS.ArrayIsNull);

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(item)) 
                    return i;
            }
            return -1;
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T>

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                // the indexer setter does not throw an exception although IsReadOnly is true.
                // This is to match the behavior of arrays.
                return true;
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            if (_list == null)
                throw new InvalidOperationException(RS.ArrayIsNull);

            return IndexOf(item) >= 0;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T>

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_list == null)
                throw new InvalidOperationException(RS.ArrayIsNull);

            return new ArraySliceEnumerator(this);
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_list == null)
                throw new InvalidOperationException(RS.ArrayIsNull);

            return new ArraySliceEnumerator(this);
        }

        #endregion

        [Serializable]
        private sealed class ArraySliceEnumerator : IEnumerator<T>
        {
            private IList<T> _array;
            private int _start;
            private int _end;
            private int _current;

            internal ArraySliceEnumerator(ArraySlice<T> arraySlice)
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