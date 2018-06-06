using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using Standard.Win32.SafeHandles;

namespace Standard.Win32
{
   partial class NativeMethods
   {
      [DllImport(Kernel32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CancelIo(SafeFileHandle handle);

      [DllImport(Kernel32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
      public static extern bool CancelIoEx(SafeFileHandle handle, IntPtr lpOverlapped);

      [DllImport(Kernel32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
      public static extern bool CancelSynchronousIo(IntPtr threadHandle);

      [StructLayout(LayoutKind.Sequential)]
      public struct OVERLAPPED
      {
         public int Internal;
         public int InternalHigh;
         public int Offset;
         public int OffsetHigh;
         public int hEvent;
      }

      /// <summary>Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.</summary>
      /// <returns>
      ///   <para>If the operation completes successfully, the return value is nonzero.</para>
      ///   <para>If the operation fails or is pending, the return value is zero. To get extended error information, call GetLastError.</para>
      /// </returns>
      /// <remarks>
      ///   <para>To retrieve a handle to the device, you must call the <c>CreateFile</c> function with either the name of a device or
      ///   the name of the driver associated with a device.</para>
      ///   <para>To specify a device name, use the following format: <c>\\.\DeviceName</c></para>
      ///   <para>Minimum supported client: Windows XP</para>
      ///   <para>Minimum supported server: Windows Server 2003</para>
      /// </remarks>
      /// <param name="hDevice">The device.</param>
      /// <param name="dwIoControlCode">The i/o control code.</param>
      /// <param name="lpInBuffer">Buffer for in data.</param>
      /// <param name="nInBufferSize">Size of the in buffer.</param>
      /// <param name="lpOutBuffer">Buffer for out data.</param>
      /// <param name="nOutBufferSize">Size of the out buffer.</param>
      /// <param name="lpBytesReturned">[out] The bytes returned.</param>
      /// <param name="lpOverlapped">The overlapped.</param>
      [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool DeviceIoControl(
            SafeFileHandle hDevice, 
            [MarshalAs(UnmanagedType.U4)] uint dwIoControlCode, 
            IntPtr lpInBuffer, 
            [MarshalAs(UnmanagedType.U4)] uint nInBufferSize, 
            SafeGlobalMemoryBufferHandle lpOutBuffer, 
            [MarshalAs(UnmanagedType.U4)] uint nOutBufferSize, 
            [MarshalAs(UnmanagedType.U4)] out uint lpBytesReturned, 
            IntPtr lpOverlapped);

		/// <summary>Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.</summary>
		/// <returns>
		///   <para>If the operation completes successfully, the return value is nonzero.</para>
		///   <para>If the operation fails or is pending, the return value is zero. To get extended error information, call GetLastError.</para>
		/// </returns>
		/// <remarks>
		///   <para>To retrieve a handle to the device, you must call the <c>CreateFile</c> function with either the name of a device or
		///   the name of the driver associated with a device.</para>
		///   <para>To specify a device name, use the following format: <c>\\.\DeviceName</c></para>
		///   <para>Minimum supported client: Windows XP</para>
		///   <para>Minimum supported server: Windows Server 2003</para>
		/// </remarks>
		/// <param name="hDevice">The device.</param>
		/// <param name="dwIoControlCode">The i/o control code.</param>
		/// <param name="lpInBuffer">Buffer for in data.</param>
		/// <param name="nInBufferSize">Size of the in buffer.</param>
		/// <param name="lpOutBuffer">Buffer for out data.</param>
		/// <param name="nOutBufferSize">Size of the out buffer.</param>
		/// <param name="lpBytesReturned">[out] The bytes returned.</param>
		/// <param name="lpOverlapped">The overlapped.</param>
		[DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool DeviceIoControl(
            SafeFileHandle hDevice, 
            [MarshalAs(UnmanagedType.U4)] uint dwIoControlCode, 
            [MarshalAs(UnmanagedType.AsAny)] object lpInBuffer, 
            [MarshalAs(UnmanagedType.U4)] uint nInBufferSize, 
            [MarshalAs(UnmanagedType.AsAny)] [Out] object lpOutBuffer, 
            [MarshalAs(UnmanagedType.U4)] uint nOutBufferSize, 
            [MarshalAs(UnmanagedType.U4)] out uint lpBytesReturned, 
            IntPtr lpOverlapped);
   }
}
