using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using Standard.Win32.SafeHandles;
using Standard.Security.Privileges;

namespace Standard.Win32
{
    partial class NativeMethods
    {
        /// <summary>The AdjustTokenPrivileges function enables or disables privileges in the specified access token. Enabling or disabling privileges in an access token requires TOKEN_ADJUST_PRIVILEGES access.</summary>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// To determine whether the function adjusted all of the specified privileges, call GetLastError.
        /// </returns>
        /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
        /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
        [DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(
            [In] AccessTokenHandle tokenHandle, 
            [In, MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, 
            [In] ref TokenPrivileges newState, 
            [In] int bufferLength, 
            [In, Out] ref TokenPrivileges previousState, 
            [In, Out] ref int returnLength);

        [DllImport(Advapi32, SetLastError = true), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetTokenInformation(
            [In] AccessTokenHandle accessTokenHandle,
            [In] TokenInformationClass tokenInformationClass,
            [Out] IntPtr tokenInformation,
            [In] int tokenInformationLength,
            [In, Out] ref int returnLength);

        [DllImport(Advapi32, SetLastError = true), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool OpenProcessToken(
            [In] ProcessHandle processHandle,
            [In] TokenAccessRights desiredAccess,
            [In, Out] ref IntPtr tokenHandle);

        [DllImport(Advapi32, CharSet = CharSet.Unicode, SetLastError = true), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LookupPrivilegeName(
           [In] string systemName,
           [In] ref Luid luid,
           [In, Out] StringBuilder name,
           [In, Out] ref int nameLength);

        /// <summary>The LookupPrivilegeDisplayName function retrieves the display name that represents a specified privilege.</summary>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, it returns zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
        /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
        [DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeDisplayNameW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LookupPrivilegeDisplayName(
            [MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, 
            [MarshalAs(UnmanagedType.LPWStr)] string lpName, 
            [In, Out] StringBuilder lpDisplayName, 
            ref uint cchDisplayName, 
            out uint lpLanguageId);

        /// <summary>The LookupPrivilegeValue function retrieves the locally unique identifier (LUID) used on a specified system to locally represent the specified privilege name.</summary>
        /// <returns>
        /// If the function succeeds, the function returns nonzero.
        /// If the function fails, it returns zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
        /// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
        [DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW")]
        [return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LookupPrivilegeValue(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, 
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpName, 
            [In, Out] ref Luid lpLuid);
    }
}
