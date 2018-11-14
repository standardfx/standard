using System;
using System.Threading;

#if NETFX
using System.Security.Permissions;
#endif

namespace Standard.IPC.SharedMemory
{
    /// <summary>
    /// Extends <see cref="SharedBuffer"/> to support simple thread-synchronisation for read/write 
    /// to the buffer by allowing callers to acquire and release read/write locks.
    /// </summary>
    /// <remarks>
    /// All buffer read/write operations have been overloaded to first perform a <see cref="System.Threading.WaitHandle.WaitOne()"/> 
    /// using the <see cref="ReadWaitEvent"/> and <see cref="WriteWaitEvent"/> respectively.
    /// 
    /// By default all read/write operations will not block, it is necessary to first acquire locks 
    /// through calls to <see cref="AcquireReadLock"/> and <see cref="AcquireWriteLock"/> as appropriate, with corresponding 
    /// calls to <see cref="ReleaseReadLock"/> and <see cref="ReleaseWriteLock"/> to release the locks.
    /// </remarks>
#if NETFX
    [PermissionSet(SecurityAction.LinkDemand)]
    [PermissionSet(SecurityAction.InheritanceDemand)]
#endif
    public abstract class LockableBuffer : SharedBuffer
    {
        private int _readWriteTimeout = 100;

        /// <summary>
        /// An event handle used for blocking write operations.
        /// </summary>
        protected EventWaitHandle WriteWaitEvent { get; private set; }
        
        /// <summary>
        /// An event handle used for blocking read operations.
        /// </summary>
        protected EventWaitHandle ReadWaitEvent { get; private set; }

        /// <summary>
        /// Create a new <see cref="LockableBuffer"/> instance with the specified name and buffer size.
        /// </summary>
        /// <param name="name">The name of the shared memory.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <param name="ownsSharedMemory">Whether or not the current instance owns the shared memory. If `true`, a new shared memory will be created and 
        /// initialized. Otherwise, an existing one is opened.</param>
        protected LockableBuffer(string name, long bufferSize, bool ownsSharedMemory)
            : base(name, bufferSize, ownsSharedMemory)
        {
            WriteWaitEvent = new EventWaitHandle(true, EventResetMode.ManualReset, Name + "_evt_write");
            ReadWaitEvent = new EventWaitHandle(true, EventResetMode.ManualReset, Name + "_evt_read");
        }

        /// <summary>
        /// The read/write operation timeout in milliseconds (to prevent deadlocks). Defaults to 100ms and must be larger than -1.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">A read or write operation's <see cref="ReadWaitEvent"/> or <see cref="WriteWaitEvent"/> did not complete with the timeframe specified.</exception>
        /// <remarks>
        /// To avoid timeout errors, use <see cref="AcquireReadLock(int)"/> and <see cref="ReleaseReadLock"/>, or 
        /// <see cref="AcquireWriteLock(int)"/> and <see cref="ReleaseWriteLock"/>.
        /// </remarks>
        public virtual int ReadWriteTimeout
        {
            get 
            { 
                return _readWriteTimeout; 
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("ReadWriteTimeout", RS.MustBeGtNegativeOne);

                _readWriteTimeout = value;
            }
        }

        /// <summary>
        /// Blocks the current thread until it is able to acquire a read lock. If successful, all subsequent writes will be blocked until after a 
        /// call to <see cref="ReleaseReadLock"/>.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite" /> to wait indefinitely.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an infinite time-out.</exception>
        /// <returns>`true` if the read lock was acquired successfully. Otherwise, `false`.</returns>
        /// <remarks>
        /// If <paramref name="millisecondsTimeout"/> is <see cref="Timeout.Infinite" />, then attempting to acquire a read lock after acquiring a write lock 
        /// on the same thread will result in a deadlock.
        /// </remarks>
        public bool AcquireReadLock(int millisecondsTimeout = Timeout.Infinite)
        {
            if (!ReadWaitEvent.WaitOne(millisecondsTimeout))
                return false;

            WriteWaitEvent.Reset();
            return true;
        }

        /// <summary>
        /// Releases the current read lock, allowing all blocked writes to continue.
        /// </summary>
        public void ReleaseReadLock()
        {
            WriteWaitEvent.Set();
        }

        /// <summary>
        /// Blocks the current thread until it is able to acquire a write lock. If successful, all subsequent reads will be blocked until after a 
        /// call to <see cref="ReleaseWriteLock"/>.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> to wait indefinitely.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an infinite time-out.</exception>
        /// <returns>
        /// `true` if the write lock was able to be acquired; otherwise `false`.</returns>
        /// <remarks>
        /// If <paramref name="millisecondsTimeout"/> is <see cref="Timeout.Infinite" />, then attempting to acquire a write lock after acquiring a read 
        /// lock on the same thread will result in a deadlock.
        /// </remarks>
        public bool AcquireWriteLock(int millisecondsTimeout = Timeout.Infinite)
        {
            if (!WriteWaitEvent.WaitOne(millisecondsTimeout))
                return false;

            ReadWaitEvent.Reset();
            return true;
        }

        /// <summary>
        /// Releases the current write lock, allowing all blocked reads to continue.
        /// </summary>
        public void ReleaseWriteLock()
        {
            ReadWaitEvent.Set();
        }

        /// <summary>
        /// Prevents write operations from deadlocking by throwing a <see cref="TimeoutException"/> if <see cref="WriteWaitEvent"/> is not available 
        /// within <see cref="ReadWriteTimeout"/>.
        /// </summary>
        private void WriteWait()
        {
            if (!WriteWaitEvent.WaitOne(ReadWriteTimeout))
                throw new TimeoutException(RS.WriteOperationTimedOut);
        }

        /// <summary>
        /// Writes an instance of <typeparamref name="T"/> into the buffer.
        /// </summary>
        /// <typeparam name="T">A structure type.</typeparam>
        /// <param name="data">A reference to an instance of <typeparamref name="T"/> to be written.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to write to.</param>
        protected override void Write<T>(ref T data, long bufferPosition = 0)
        {
            WriteWait();
            base.Write<T>(ref data, bufferPosition);
        }

        /// <summary>
        /// Writes an array of <typeparamref name="T"/> into the buffer.
        /// </summary>
        /// <typeparam name="T">A structure type.</typeparam>
        /// <param name="buffer">An array of <typeparamref name="T"/> to be written. The length of this array controls the number of elements to be written.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to write to.</param>
        protected override void Write<T>(T[] buffer, long bufferPosition = 0)
        {
            WriteWait();
            base.Write<T>(buffer, bufferPosition);
        }

        /// <summary>
        /// Writes the specified number of bytes from the pointer position into the shared memory buffer.
        /// </summary>
        /// <param name="ptr">A managed pointer to the memory location to be copied into the buffer.</param>
        /// <param name="length">The number of bytes to be copied.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to write to.</param>
        protected override void Write(IntPtr ptr, int length, long bufferPosition = 0)
        {
            WriteWait();
            base.Write(ptr, length, bufferPosition);
        }

        /// <summary>
        /// Prepares an <see cref="IntPtr"/> to the buffer position and calls an <see cref="Action"/> to perform the writing.
        /// </summary>
        /// <param name="writeFunc">A function used to write to the buffer. The <see cref="IntPtr"/> parameter is a pointer to the buffer 
        /// offset by <paramref name="bufferPosition"/>.</param>
        /// <param name="bufferPosition">The offset within the buffer region to start writing from.</param>
        protected override void Write(Action<IntPtr> writeFunc, long bufferPosition = 0)
        {
            WriteWait();
            base.Write(writeFunc, bufferPosition);
        }

        /// <summary>
        /// Prevents read operations from deadlocking by throwing a <see cref="TimeoutException"/> if <see cref="ReadWaitEvent"/> is not available
        /// within <see cref="ReadWriteTimeout"/>.
        /// </summary>
        private void ReadWait()
        {
            if (!ReadWaitEvent.WaitOne(ReadWriteTimeout))
                throw new TimeoutException(RS.ReadOperationTimedOut);
        }

        /// <summary>
        /// Reads an instance of <typeparamref name="T"/> from the buffer.
        /// </summary>
        /// <typeparam name="T">A structure type.</typeparam>
        /// <param name="data">Output parameter that will contain the value read from the buffer.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        protected override void Read<T>(out T data, long bufferPosition = 0)
        {
            ReadWait();
            base.Read<T>(out data, bufferPosition);
        }

        /// <summary>
        /// Reads an array of <typeparamref name="T"/> from the buffer.
        /// </summary>
        /// <typeparam name="T">A structure type.</typeparam>
        /// <param name="buffer">Array that will contain the values read from the buffer. The length of this array controls the number of elements to read.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        protected override void Read<T>(T[] buffer, long bufferPosition = 0)
        {
            ReadWait();
            base.Read<T>(buffer, bufferPosition);
        }

        /// <summary>
        /// Reads the specified number of bytes from the shared memory buffer into a memory location.
        /// </summary>
        /// <param name="destination">A managed pointer to the memory location to copy data into from the buffer.</param>
        /// <param name="length">The number of bytes to be copied.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        protected override void Read(IntPtr destination, int length, long bufferPosition = 0)
        {
            ReadWait();
            base.Read(destination, length, bufferPosition);
        }

        /// <summary>
        /// Prepares an <see cref="IntPtr"/> to the buffer position and calls an <see cref="Action"/> to perform the reading.
        /// </summary>
        /// <param name="readFunc">A function used to read from the buffer. The <see cref="IntPtr"/> parameter is a pointer to the buffer 
        /// offset by <paramref name="bufferPosition"/>.</param>
        /// <param name="bufferPosition">The offset within the buffer region of the shared memory to read from.</param>
        protected override void Read(Action<IntPtr> readFunc, long bufferPosition = 0)
        {
            ReadWait();
            base.Read(readFunc, bufferPosition);
        }

        #region IDisposable

        /// <summary>
        /// Implements the <see cref="IDisposable"/> pattern to dispose of managed/unmanaged resources.
        /// </summary>
        /// <param name="disposeManagedResources">`true` to dispose both managed and unmanaged resources. Otherwise, `false` to dispose unmanaged resources only.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                (WriteWaitEvent as IDisposable).Dispose();
                (ReadWaitEvent as IDisposable).Dispose();
            }

            base.Dispose(disposeManagedResources);
        }

        #endregion
    }
}