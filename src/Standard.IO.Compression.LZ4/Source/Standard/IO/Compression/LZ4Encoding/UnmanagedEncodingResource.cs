using System;
using System.Threading;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// A skeleton class for working with unmanaged resources.
    /// </summary>
    /// <remarks>
    /// This class implements <see cref="IDisposable"/> but also handles proper release in
    /// case <see cref="Dispose()"/> was not called.
    /// </remarks>
    internal abstract class UnmanagedEncodingResource : IDisposable
	{
		private int _disposed;

		/// <summary>
        /// Determines whether this instance of <see cref="UnmanagedEncodingResource"/> was already disposed.
        /// </summary>
		public bool IsDisposed
        {
            get
            {
                return Interlocked.CompareExchange(ref _disposed, 0, 0) != 0;
            }
        }

		/// <summary>
        /// Throws an exception if this instance of <see cref="UnmanagedEncodingResource"/> has been disposed already.
        /// </summary>
		/// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        /// <remarks>
        /// This is a convenience method for raising exceptions.
        /// </remarks>
		protected void ThrowIfDisposed()
		{
			if (IsDisposed)
                throw new ObjectDisposedException(string.Format("The object {0} is already disposed.", GetType().FullName));
		}

		/// <summary>
        /// Release all unmanaged resources held by this instance of <see cref="UnmanagedEncodingResource"/>.
        /// </summary>
		protected virtual void ReleaseUnmanaged()
        {
        }

        /// <summary>
        /// Release all managed resources held by this instance of <see cref="UnmanagedEncodingResource"/>.
        /// </summary>
        protected virtual void ReleaseManaged()
        {
        }

		/// <summary>
		/// Dispose both managed and unmanaged resources.
		/// </summary>
		/// <param name="disposing">`true` if dispose was explicitly called, or `false` if called from GC.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
				return;

			ReleaseUnmanaged();

			if (disposing)
				ReleaseManaged();
		}

		/// <see cref="IDisposable.Dispose()"/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
        /// Deconstructor for the <see cref="UnmanagedEncodingResource"/> instance.
        /// </summary>
		~UnmanagedEncodingResource()
        {
            Dispose(false);
        }
	}
}
