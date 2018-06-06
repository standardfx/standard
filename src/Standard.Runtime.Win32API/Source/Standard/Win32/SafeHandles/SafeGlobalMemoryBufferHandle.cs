using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Standard.Win32.SafeHandles
{
   /// <summary>Represents a block of native memory of a specified size allocated using the LocalAlloc function from Kernel32.dll.</summary>
   public sealed class SafeGlobalMemoryBufferHandle : SafeNativeMemoryBufferHandle
   {
      /// <summary>Creates new instance with zero IntPtr.</summary>
      public SafeGlobalMemoryBufferHandle()
         : base(true)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="SafeGlobalMemoryBufferHandle"/> class allocating the specified number of bytes of unmanaged memory.</summary>
      /// <param name="capacity">The capacity.</param>
      public SafeGlobalMemoryBufferHandle(int capacity) :
         base(capacity)
      {
         SetHandle(Marshal.AllocHGlobal(capacity));
      }

      private SafeGlobalMemoryBufferHandle(IntPtr buffer, int capacity)
         : base(buffer, capacity)
      {
      }

      public static SafeGlobalMemoryBufferHandle FromLong(long? value)
      {
         if (value.HasValue)
         {
            SafeGlobalMemoryBufferHandle safeBuffer = new SafeGlobalMemoryBufferHandle(Marshal.SizeOf(typeof (long)));
            Marshal.WriteInt64(safeBuffer.handle, value.Value);
            return safeBuffer;
         }

         return new SafeGlobalMemoryBufferHandle();
      }

      public static SafeGlobalMemoryBufferHandle FromStringUni(string str)
      {
         if (str == null)
            throw new ArgumentNullException(nameof(str));

         return new SafeGlobalMemoryBufferHandle(Marshal.StringToHGlobalUni(str), str.Length * UnicodeEncoding.CharSize + UnicodeEncoding.CharSize);
      }

      /// <summary>Called when object is disposed or finalized.</summary>
      protected override bool ReleaseHandle()
      {
         Marshal.FreeHGlobal(handle);
         return true;
      }
   }
}
