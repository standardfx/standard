using System;
using System.Runtime.InteropServices;

namespace Standard.IPC.SharedMemory
{
    /// <summary>
    /// A structure that is always located at the start of the shared memory in a <see cref="SharedBuffer"/> instance. 
    /// This allows the shared memory to be opened by other instances without knowing its size before hand.
    /// </summary>
    /// <remarks>
    /// This structure is the same size on 32-bit and 64-bit architectures.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct SharedHeader
    {
        /// <summary>
        /// Pad to 16-bytes.
        /// </summary>
        private int _padding0;

        /// <summary>
        /// The total size of the buffer including <see cref="SharedHeader"/>.
        /// </summary>
        /// <remarks>
        /// The size is calculated using the following formula:
        /// 
        /// ```C#
        /// BufferSize + Marshal.SizeOf(typeof(SharedMemory.SharedHeader))
        /// ```
        /// </remarks>
        public long SharedMemorySize;

        /// <summary>
        /// Flag indicating whether the owner of the buffer has closed its <see cref="System.IO.MemoryMappedFiles.MemoryMappedFile"/> 
        /// and <see cref="System.IO.MemoryMappedFiles.MemoryMappedViewAccessor"/>.
        /// </summary>
        public volatile int Shutdown;
    }
}