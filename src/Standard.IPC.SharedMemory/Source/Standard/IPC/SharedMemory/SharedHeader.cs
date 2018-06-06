using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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
        /// The total size of the buffer including <see cref="SharedHeader"/>, i.e. <code>BufferSize + Marshal.SizeOf(typeof(SharedMemory.SharedHeader))</code>.
        /// </summary>
        public long SharedMemorySize;

        /// <summary>
        /// Flag indicating whether the owner of the buffer has closed its <see cref="System.IO.MemoryMappedFiles.MemoryMappedFile"/> and <see cref="System.IO.MemoryMappedFiles.MemoryMappedViewAccessor"/>.
        /// </summary>
        public volatile int Shutdown;

        /// <summary>
        /// Pad to 16-bytes.
        /// </summary>
        private int _padding0;
    }
}