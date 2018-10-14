using System;
using System.IO;

namespace Standard
{
    /// <summary>
    /// Extension methods for working with the <see cref="Stream"/> class. This class is used for compatibility purposes.
    /// </summary>
    public static class StreamCompatExtension
    {
#if NETSTANDARD
        /// <summary>
        /// Close and disposes a <see cref="Stream"/> object.
        /// </summary>
        /// <param name="stream">The stream instance to close.</param>
        public static void Close(this Stream stream)
        {
            stream.Dispose();
            GC.SuppressFinalize(stream);
        }
#endif
    }
}
