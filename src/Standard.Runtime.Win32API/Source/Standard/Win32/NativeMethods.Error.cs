using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Standard.Win32
{
	partial class NativeMethods
	{
		/// <summary>
		///   Controls whether the system will handle the specified types of serious errors or whether the process will handle them.
		/// </summary>
		/// <remarks>
		///   Because the error mode is set for the entire process, you must ensure that multi-threaded applications do not set different error-
		///   mode attributes. Doing so can lead to inconsistent error handling.
		/// </remarks>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only].</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only].</remarks>
		/// <param name="uMode">The mode.</param>
		/// <returns>The return value is the previous state of the error-mode bit attributes.</returns>
		[DllImport(Kernel32, SetLastError = false, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern ErrorMode SetErrorMode(ErrorMode uMode);

		/// <summary>
		///   Controls whether the system will handle the specified types of serious errors or whether the calling thread will handle them.
		/// </summary>
		/// <remarks>
		///   Because the error mode is set for the entire process, you must ensure that multi-threaded applications do not set different error-
		///   mode attributes. Doing so can lead to inconsistent error handling.
		/// </remarks>
		/// <remarks>Minimum supported client: Windows 7 [desktop apps only].</remarks>
		/// <remarks>Minimum supported server: Windows Server 2008 R2 [desktop apps only].</remarks>
		/// <param name="dwNewMode">The new mode.</param>
		/// <param name="lpOldMode">[out] The old mode.</param>
		/// <returns>The return value is the previous state of the error-mode bit attributes.</returns>
		[DllImport(Kernel32, SetLastError = false, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetThreadErrorMode(ErrorMode dwNewMode, [MarshalAs(UnmanagedType.U4)] out ErrorMode lpOldMode);
	}
}
