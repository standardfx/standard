using System;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// Interface for LZ4 encoders. Encoders are used by LZ4 streams.
    /// </summary>
    internal interface ILZ4Encoder : IDisposable
	{
        /// <summary>
        /// Block size.
        /// </summary>
        int BlockSize { get; }

        /// <summary>
        /// Bytes already decoded and available to be read.
        /// Always smaller than <see cref="BlockSize"/>
        /// </summary>
        int BytesReady { get; }

		/// <summary>
        /// Adds bytes to internal buffer. Calling this method will increment <see cref="BytesReady"/>.
        /// </summary>
		/// <param name="source">Source buffer.</param>
		/// <param name="length">Source buffer length.</param>
		/// <returns>
        /// Number of bytes topped up. If this function returns 0, it means that the buffer
		/// is full (<see cref="BytesReady"/> equals <see cref="BlockSize"/>). In such case, 
		/// call <see cref="Encode(byte*, int, bool)"/> to flush it.
        /// </returns>
		unsafe int Topup(byte* source, int length);

		/// <summary>
		/// Encodes bytes in internal buffer (see: <see cref="BytesReady"/>, <see cref="Topup(byte*, int)"/>).
		/// If <paramref name="allowCopy"/> is `true`, if encoded buffer is bigger than
		/// source buffer source bytes are copied instead. In such case, the returned length is negative.
		/// </summary>
		/// <param name="target">Target buffer.</param>
		/// <param name="length">Target buffer length.</param>
		/// <param name="allowCopy">Indicates if copying is allowed.</param>
		/// <returns>
        /// Length of encoded buffer. A negative value is returned if bytes are just copied.
        /// </returns>
		unsafe int Encode(byte* target, int length, bool allowCopy);
	}
}
