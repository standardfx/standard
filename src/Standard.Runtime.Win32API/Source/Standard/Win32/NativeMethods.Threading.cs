using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Standard.Security.AccessControl;

namespace Standard.Win32
{
	partial class NativeMethods
	{
		public const uint WAIT_OBJECT_0 = 0;
		public const uint WAIT_FAILED = 0xffffffff;
		public const int WAIT_INFINITE = 0xffff;

		[DllImport(Kernel32, CharSet = CharSet.Auto)]
		public static extern IntPtr CreateEvent(
          [MarshalAs(UnmanagedType.LPStruct)] ref SecurityAttributes lpSecurityAttributes,
          int bManualReset, 
          int bInitialState, 
          string lpName);

		[DllImport(Kernel32, SetLastError = true)]
		public static extern uint WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);
	}
}
