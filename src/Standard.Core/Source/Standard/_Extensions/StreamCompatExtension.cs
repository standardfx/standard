using System;
using System.IO;

namespace Standard
{
    public static class StreamCompatExtension
    {
#if NETSTANDARD
        public static void Close(this Stream stream)
        {
            stream.Dispose();
            GC.SuppressFinalize(stream);
        }
#endif
    }
}
