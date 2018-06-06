using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;
using Standard.Runtime.Win32API;

namespace Standard.Win32.SafeHandles
{
    /// <summary>An IntPtr wrapper which can be used as the result of a Marshal.AllocHGlobal operation.
    /// <para>Calls Marshal.FreeHGlobal when disposed or finalized.</para>
    /// </summary>
    public sealed class SafeLocalMemoryBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>Creates new instance with zero IntPtr.</summary>      
        public SafeLocalMemoryBufferHandle() 
			: base(true)
        {
        }

        /// <summary>Copies data from a one-dimensional, managed 8-bit unsigned integer array to the unmanaged memory pointer referenced by this instance.</summary>
        /// <param name="source">The one-dimensional array to copy from.</param>
        /// <param name="startIndex">The zero-based index into the array where Copy should start.</param>
        /// <param name="length">The number of array elements to copy.</param>      
        public void CopyFrom(byte[] source, int startIndex, int length)
        {
            Marshal.Copy(source, startIndex, handle, length);
        }

        public void CopyTo(byte[] destination, int destinationOffset, int length)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");

            if (destinationOffset < 0)
                throw new ArgumentOutOfRangeException("destinationOffset", RS.NegativeDestinationOffset);

            if (length < 0)
                throw new ArgumentOutOfRangeException("length", RS.NegativeLength);

            if (destinationOffset + length > destination.Length)
                throw new ArgumentException(RS.DestinationBufferNotLargeEnough);

             Marshal.Copy(handle, destination, destinationOffset, length);
        }

        public byte[] ToByteArray(int startIndex, int length)
        {
            if (IsInvalid)
                return null;

            byte[] arr = new byte[length];
            Marshal.Copy(handle, arr, startIndex, length);
            return arr;
        }

        /// <summary>Called when object is disposed or finalized.</summary>
        protected override bool ReleaseHandle()
        {
            return handle == IntPtr.Zero || NativeMethods.LocalFree(handle) == IntPtr.Zero;
        }
    }
}
