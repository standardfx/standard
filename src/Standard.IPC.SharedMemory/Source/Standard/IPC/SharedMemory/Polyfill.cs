using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Standard.IPC.SharedMemory
{
    internal static class Polyfill
    {
        public static int GetMarshalSizeOf<T>()
        {
#if NETSTANDARD
            return Marshal.SizeOf<T>();
#else
            return Marshal.SizeOf(typeof(T));
#endif
        }

        public static T GetMarshalPtrToStructure<T>(IntPtr pointer)
        {
#if NETSTANDARD
            return Marshal.PtrToStructure<T>(pointer);
#else
            return (T)(Marshal.PtrToStructure(pointer, typeof(T)));
#endif
        }

    }
}
