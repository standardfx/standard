using System;
using Standard;
using System.Security;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Standard.Win32
{
	/// <summary>Enum for struct ChangeErrorMode.</summary>
	[Flags]
	public enum ErrorMode
	{
		/// <summary>Use the system default, which is to display all error dialog boxes.</summary>
		SystemDefault = 0,

		/// <summary>The system does not display the critical-error-handler message box. Instead, the system sends the error to the calling process/thread.</summary>
		FailCriticalErrors = 1,

		/// <summary>The system does not display the Windows Error Reporting dialog.</summary>
		NoGpfaultErrorbox = 2,

		/// <summary>The system automatically fixes memory alignment faults and makes them invisible to the application. It does this for the calling process and any descendant processes. This feature is only supported by certain processor architectures.</summary>
		NoAlignmentFaultExcept = 4,

		/// <summary>The system does not display a message box when it fails to find a file. Instead, the error is returned to the calling process/thread.</summary>
		NoOpenFileErrorbox = 32768
	}

	/// <summary>Controls whether the system will handle the specified types of serious errors or whether the process will handle them.</summary>
	/// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
	/// <remarks>Minimum supported server: Windows 2000 Server</remarks>      
	public sealed class ChangeErrorMode : IDisposable
	{
		// using the _xxx notation is easier with dispose
		private readonly ErrorMode _oldMode;
		private bool winverIsAtLeast7 = false;
		private bool versionChecked = false;

		[SecuritySafeCritical]
		private bool IsAtLeastWindows7()
		{
			if (versionChecked == true)
				return winverIsAtLeast7;

			var verInfo = new NativeMethods.RTL_OSVERSIONINFOEXW();

            // Needed to prevent: System.Runtime.InteropServices.COMException:
            // The data area passed to a system call is too small. (Exception from HRESULT: 0x8007007A)
            verInfo.dwOSVersionInfoSize = Marshal.SizeOf(verInfo);

            var sysInfo = new NativeMethods.SYSTEM_INFO();
			NativeMethods.GetNativeSystemInfo(ref sysInfo);

            if (NativeMethods.RtlGetVersion(ref verInfo))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (verInfo.dwMajorVersion < 6)
            	winverIsAtLeast7 = false;
            else if (verInfo.dwMajorVersion == 6 && verInfo.dwMinorVersion == 0)
            	winverIsAtLeast7 = false;
            else
            	winverIsAtLeast7 = true;

            return winverIsAtLeast7;
		}

		[SecuritySafeCritical]
		public ChangeErrorMode(ErrorMode mode)
		{
            if (IsAtLeastWindows7())
               NativeMethods.SetThreadErrorMode(mode, out _oldMode);
            else
               _oldMode = NativeMethods.SetErrorMode(mode);
        }

		[SecuritySafeCritical]
		void IDisposable.Dispose()
        {
			ErrorMode oldMode;

            if (IsAtLeastWindows7())
               NativeMethods.SetThreadErrorMode(_oldMode, out oldMode);
            else
               NativeMethods.SetErrorMode(_oldMode);
		}
	}
}
