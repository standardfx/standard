using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;

namespace Standard.Win32
{
	partial class NativeMethods
    {
        public const short VER_NT_WORKSTATION = 1;
        public const short VER_NT_DOMAIN_CONTROLLER = 2;
        public const short VER_NT_SERVER = 3;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RTL_OSVERSIONINFOEXW
        {
            public int dwOSVersionInfoSize;
            public readonly int dwMajorVersion;
            public readonly int dwMinorVersion;
            public readonly int dwBuildNumber;
            public readonly int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public readonly string szCSDVersion;
            public readonly ushort wServicePackMajor;
            public readonly ushort wServicePackMinor;
            public readonly ushort wSuiteMask;
            public readonly byte wProductType;
            public readonly byte wReserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SYSTEM_INFO
        {
            public readonly ushort wProcessorArchitecture;
            private readonly ushort wReserved;
            public readonly uint dwPageSize;
            public readonly IntPtr lpMinimumApplicationAddress;
            public readonly IntPtr lpMaximumApplicationAddress;
            public readonly IntPtr dwActiveProcessorMask;
            public readonly uint dwNumberOfProcessors;
            public readonly uint dwProcessorType;
            public readonly uint dwAllocationGranularity;
            public readonly ushort wProcessorLevel;
            public readonly ushort wProcessorRevision;
        }

        /// <summary>The RtlGetVersion routine returns version information about the currently running operating system.</summary>
        /// <returns>RtlGetVersion returns STATUS_SUCCESS.</returns>
        /// <remarks>Available starting with Windows 2000.</remarks>
        [DllImport(Ntdll, SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RtlGetVersion([MarshalAs(UnmanagedType.Struct)] ref RTL_OSVERSIONINFOEXW lpVersionInformation);

        /// <summary>
		/// Retrieves information about the current system to an application running under WOW64.
        /// If the function is called from a 64-bit application, it is equivalent to the GetSystemInfo function.
        /// </summary>
        /// <returns>This function does not return a value.</returns>
        /// <remarks>To determine whether a Win32-based application is running under WOW64, call the <see cref="IsWow64Process"/> function.</remarks>
        /// <remarks>Minimum supported client: Windows XP [desktop apps | Windows Store apps]</remarks>
        /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps | Windows Store apps]</remarks>
        [DllImport(Kernel32, SetLastError = false, CharSet = CharSet.Unicode)]
        public static extern void GetNativeSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        /// <summary>Determines whether the specified process is running under WOW64.</summary>
        /// <returns>
        /// If the function succeeds, the return value is a nonzero value.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>Minimum supported client: Windows Vista, Windows XP with SP2 [desktop apps only]</remarks>
        /// <remarks>Minimum supported server: Windows Server 2008, Windows Server 2003 with SP1 [desktop apps only]</remarks>
        [DllImport(Kernel32, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out, MarshalAs(UnmanagedType.Bool)] out bool lpSystemInfo);
    }
}
