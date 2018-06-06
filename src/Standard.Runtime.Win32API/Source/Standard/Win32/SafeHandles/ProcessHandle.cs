using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Standard.Win32.SafeHandles
{
    public sealed class ProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
		public ProcessHandle(IntPtr processHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            handle = processHandle;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            if (!NativeMethods.CloseHandle(handle))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return true;
        }
    }
}
