using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Standard.IPC.SharedMemory
{
    /// <summary>
    /// A generic fixed-length shared memory array of structures, with support for simple inter-process read/write synchronization.
    /// </summary>
    /// <typeparam name="T">The structure type that will be stored in the elements of this fixed array buffer.</typeparam>
    [PermissionSet(SecurityAction.LinkDemand)]
    [PermissionSet(SecurityAction.InheritanceDemand)]
    public class SharedArray<T> : LockableBuffer, IList<T>
        where T : struct
    {
        private int _elementSize;

        /// <summary>
        /// Creates the shared memory array with the specified name.
        /// </summary>
        /// <param name="name">The name of the shared memory array to be created.</param>
        /// <param name="length">The number of elements to make room for within the shared memory array.</param>
        public SharedArray(string name, int length)
            : base(name, Marshal.SizeOf(typeof(T)) * length, true)
        {
            Length = length;
            _elementSize = Marshal.SizeOf(typeof(T));

            Open();
        }

        /// <summary>
        /// Opens an existing shared memory array with the name specified.
        /// </summary>
        /// <param name="name">The name of the shared memory array to open.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The shared memory location specified by <paramref name="name"/> does not have a <see cref="SharedBuffer.BufferSize"/> that is evenly 
        /// divisible by the size of <typeparamref name="T"/>.
        /// </exception>
        public SharedArray(string name)
            : base(name, 0, false)
        {
            _elementSize = Marshal.SizeOf(typeof(T));

            Open();
        }

        /// <summary>
        /// Gets a 32-bit integer that represents the total number of elements in the <see cref="SharedArray{T}"/>.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="index"/> is equal to or greater 
        /// than <see cref="Length"/>.</exception>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                T item;
                Read(out item, index);
                return item;
            }
            set
            {
                Write(ref value, index);
            }
        }

        /// <summary>
        /// Perform any initialization required when opening the shared memory array.
        /// </summary>
        /// <returns>`true` if successful. Otherwise, `false`.</returns>
        protected override bool DoOpen()
        {
            if (!IsOwnerOfSharedMemory)
            {
                if (BufferSize % _elementSize != 0)
                    throw new ArgumentOutOfRangeException("name", string.Format(RS.BufferSizeNotEvenlyDivisible, typeof(T).Name));

                Length = (int)(BufferSize / _elementSize);
            }
            return true;
        }

        #region Read/Write

        /// <summary>
        /// Copy <paramref name="data"/> to the shared memory array element at the specified index position.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="index">The zero-based index of the element to set.</param>
        public void Write(ref T data, int index)
        {
            if (index > Length - 1 || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            base.Write(ref data, index * _elementSize);
        }

        /// <summary>
        /// Copy the elements of an array into the shared memory array, starting at the index position.
        /// </summary>
        /// <param name="buffer">The source array to copy elements from.</param>
        /// <param name="startIndex">The zero-based index of the shared memory array element to begin writing to.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0, or the total length of <paramref name="buffer"/> and <paramref name="startIndex"/> is greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is `null`.</exception>
        public void Write(T[] buffer, int startIndex = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Length + startIndex > Length || startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            base.Write(buffer, startIndex * _elementSize);
        }

        /// <summary>
        /// Reads a single element from the shared memory array into an element located at the specified index position.
        /// </summary>
        /// <param name="data">The element at the specified index.</param>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or <paramref name="index"/> is equal to or greater than <see cref="Length"/>.</exception>
        public void Read(out T data, int index)
        {
            if (index > Length - 1 || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            base.Read(out data, index * _elementSize);
        }

        /// <summary>
        /// Reads a number of elements from the shared memory array into a buffer array <paramref name="buffer"/>, starting from the specified index position.
        /// </summary>
        /// <param name="buffer">The destination array to copy the elements into.</param>
        /// <param name="startIndex">The zero-based index of the shared memory array element to begin reading from.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0, or the total length of <paramref name="buffer"/> and 
        /// <paramref name="startIndex"/> is greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is `null`.</exception>
        public void CopyTo(T[] buffer, int startIndex = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Length + startIndex > Length || startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            base.Read(buffer, startIndex * _elementSize);
        }

        #endregion // Read/Write

        #region IEnumerable<T>

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerator{T}"/> instance that can be used to iterate through the collection</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList<T>

        /// <summary>
        /// Operation not supported. Throws <see cref="System.NotImplementedException"/>
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Operation not supported. Throws <see cref="System.NotImplementedException"/>
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the list contains the specified item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>`true` if <paramref name="item"/> exists in the list. Otherwise, `false`.</returns>
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        /// <summary>
        /// Operation not supported. Throws <see cref="System.NotImplementedException"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the number of elements in the array.
        /// </summary>
        public int Count
        {
            get { return Length; }
        }

        /// <summary>
        /// The elements are not read-only. This property will always return `true`.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Return the index of the specified item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>The index position of the item if found, otherwise -1.</returns>
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
        /// Operation not supported. Throws <see cref="System.NotImplementedException"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Operation not supported. Throws <see cref="System.NotImplementedException"/>
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}