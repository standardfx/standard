using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Standard.Win32
{
    [SuppressUnmanagedCodeSecurity]
    public static partial class UnsafeNativeMethods
    {
        /// <summary>
        /// Allow copying memory from one IntPtr to another. Required as the <see cref="System.Runtime.InteropServices.Marshal.Copy(System.IntPtr, System.IntPtr[], int, int)"/> implementation does not provide an appropriate override.
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <param name="count"></param>
        [DllImport(NativeMethods.Kernel32, EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport(NativeMethods.Kernel32, EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern unsafe void CopyMemoryPtr(void* dest, void* src, uint count);
    }
}
