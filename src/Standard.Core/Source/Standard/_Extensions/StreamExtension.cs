using System;
using System.IO;
using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Extension methods for working with <see cref="Stream"/> class.
    /// </summary>
    public static class StreamExtension
    {
        private const int MIN_BUFFER_SIZE= 128;

        /// <summary>
        /// Tests whether a <see cref="Stream"/> is at its last position.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> object.</param>
        /// <returns>
        /// `true` if the <paramref name="stream"/> object is at its last position; otherwise, `false`.
        /// </returns>
        public static bool IsEndOfStream(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
                
            return (stream.Position == stream.Length);
        }

        /// <summary>
        /// Read from a <see cref="Stream"/> ensuring all the required data is read.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="buffer">The buffer to fill.</param>
        /// <seealso cref="ReadAll(Stream,byte[],int,int)"/>
        /// <remarks>
        /// The buffer will be filled with data from the stream, up to the length of the buffer.
        ///
        /// You need to ensure that the buffer is of the same length as the stream data. If the 
        /// end of stream has been reached before the buffer is filled, an error occurs.
        /// </remarks>
        public static void ReadAll(this Stream stream, byte[] buffer)
        {
            ReadAll(stream, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Read from a <see cref="Stream"/>" ensuring all the required data is read.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="buffer">The buffer to store data in.</param>
        /// <param name="offset">The offset at which to begin storing data.</param>
        /// <param name="count">The number of bytes of data to store.</param>
        /// <exception cref="ArgumentNullException">Required parameter is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> and or <paramref name="count"/> are invalid.</exception>
        /// <exception cref="EndOfStreamException">End of stream is encountered before all the data has been read.</exception>
        public static void ReadAll(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // Offset can equal length when buffer and count are 0.
            if ((offset < 0) || (offset > buffer.Length))
                throw new ArgumentOutOfRangeException(nameof(offset));

            if ((count < 0) || (offset + count > buffer.Length))
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 0)
            {
                int readCount = stream.Read(buffer, offset, count);
                if (readCount <= 0)
                    throw new EndOfStreamException();

                offset += readCount;
                count -= readCount;
            }
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        public static void CopyTo(this Stream source, Stream destination, byte[] buffer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < MIN_BUFFER_SIZE)
                throw new ArgumentException(RS.BufferTooSmall, nameof(buffer));

            bool copying = true;
            while (copying)
            {
                int bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }
            }
        }
    }
}
