using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Standard.IPC.SharedMemory
{
    [SuppressUnmanagedCodeSecurity]
    internal static partial class UnsafeNativeMethods
    {
        public const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport(Kernel32, EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern unsafe void CopyMemoryPtr(void* dest, void* src, uint count);
    }
}
