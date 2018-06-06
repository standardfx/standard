using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Standard.Security.AccessControl;
using Standard.Security.Privileges;

namespace Standard.Win32.SafeHandles
{
    /// <summary>Handle to an access token.</summary>
    public sealed class AccessTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
		public AccessTokenHandle(ProcessHandle processHandle, TokenAccessRights tokenAccessRights)
            : base(true)
        {
            if (!NativeMethods.OpenProcessToken(processHandle, tokenAccessRights, ref handle))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>Releases the handle.</summary>
        /// <returns>Value indicating if the handle released successfully.</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            if (!NativeMethods.CloseHandle(handle))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return true;
        }
    }
}
