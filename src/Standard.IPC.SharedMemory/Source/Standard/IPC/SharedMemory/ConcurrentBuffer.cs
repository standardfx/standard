using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace Standard.IPC.SharedMemory
{
    /// <summary>
    /// Read/Write buffer with support for simple inter-process read/write synchronization.
    /// </summary>
    [PermissionSet(SecurityAction.LinkDemand)]
    public unsafe class ConcurrentBuffer : LockableBuffer
    {
        /// <summary>
        /// Creates a new shared memory buffer with the specified name and size
        /// </summary>
        /// <param name="name">The name of the shared memory to create</param>
        /// <param name="bufferSize">The size of the buffer</param>
        public ConcurrentBuffer(string name, int bufferSize)
            : base(name, bufferSize, true)
        {
            Open();
        }

        /// <summary>
        /// Opens an existing shared memory buffer with the specified name
        /// </summary>
        /// <param name="name">The name of the shared memory to open</param>
        public ConcurrentBuffer(string name)
            : base(name, 0, false)
        {
            Open();
        }

        /// <summary>
        /// Writes an instance of <typeparamref name="T"/> into the buffer
        /// </summary>
        /// <typeparam name="T">A structure type</typeparam>
        /// <param name="data">A reference to an instance of <typeparamref name="T"/> to be written</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to write to.</param>
        new public void Write<T>(ref T data, long bufferPosition = 0)
            where T : struct
        {
            base.Write(ref data, bufferPosition);
        }

        /// <summary>
        /// Writes an array of <typeparamref name="T"/> into the buffer
        /// </summary>
        /// <typeparam name="T">A structure type</typeparam>
        /// <param name="buffer">An array of <typeparamref name="T"/> to be written. The length of this array controls the number of elements to be written.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to write to.</param>
        new public void Write<T>(T[] buffer, long bufferPosition = 0)
            where T : struct
        {
            base.Write(buffer, bufferPosition);
        }

        /// <summary>
        /// Writes <paramref name="length"/> bytes from the <paramref name="ptr"/> into the shared memory buffer.
        /// </summary>
        /// <param name="ptr">A managed pointer to the memory location to be copied into the buffer</param>
        /// <param name="length">The number of bytes to be copied</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to write to.</param>
        new public void Write(IntPtr ptr, int length, long bufferPosition = 0)
        {
            base.Write(ptr, length, bufferPosition);
        }

        /// <summary>
        /// Prepares an IntPtr to the buffer position and calls <paramref name="writeFunc"/> to perform the writing.
        /// </summary>
        /// <param name="writeFunc">A function used to write to the buffer. The IntPtr parameter is a pointer to the buffer offset by <paramref name="bufferPosition"/>.</param>
        /// <param name="bufferPosition">The offset within the buffer region to start writing from.</param>
        new public void Write(Action<IntPtr> writeFunc, long bufferPosition = 0)
        {
            base.Write(writeFunc, bufferPosition);
        }

        /// <summary>
        /// Reads an instance of <typeparamref name="T"/> from the buffer
        /// </summary>
        /// <typeparam name="T">A structure type</typeparam>
        /// <param name="data">Output parameter that will contain the value read from the buffer</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        new public void Read<T>(out T data, long bufferPosition = 0)
            where T : struct
        {
            base.Read(out data, bufferPosition);
        }

        /// <summary>
        /// Reads an array of <typeparamref name="T"/> from the buffer
        /// </summary>
        /// <typeparam name="T">A structure type</typeparam>
        /// <param name="buffer">Array that will contain the values read from the buffer. The length of this array controls the number of elements to read.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        new public void Read<T>(T[] buffer, long bufferPosition = 0)
            where T : struct
        {
            base.Read(buffer, bufferPosition);
        }

        /// <summary>
        /// Reads <paramref name="length"/> bytes into the memory location <paramref name="destination"/> from the shared memory buffer.
        /// </summary>
        /// <param name="destination">A managed pointer to the memory location to copy data into from the buffer</param>
        /// <param name="length">The number of bytes to be copied</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        new public void Read(IntPtr destination, int length, long bufferPosition = 0)
        {
            base.Read(destination, length, bufferPosition);
        }

        /// <summary>
        /// Prepares an IntPtr to the buffer position and calls <paramref name="readFunc"/> to perform the reading.
        /// </summary>
        /// <param name="readFunc">A function used to read from the buffer. The IntPtr parameter is a pointer to the buffer offset by <paramref name="bufferPosition"/>.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        new public void Read(Action<IntPtr> readFunc, long bufferPosition = 0)
        {
            base.Read(readFunc, bufferPosition);
        }
    }
}