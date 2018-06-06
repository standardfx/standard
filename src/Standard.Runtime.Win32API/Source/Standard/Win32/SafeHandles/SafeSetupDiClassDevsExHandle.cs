using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Standard.Win32.SafeHandles
{
    /// <summary>
	/// Represents a wrapper class for a handle used by the SetupDiGetClassDevs/SetupDiDestroyDeviceInfoList Win32 API functions.
	/// </summary>
    public sealed class SafeSetupDiClassDevsExHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>Initializes a new instance of the <see cref="SafeSetupDiClassDevsExHandle"/> class.</summary>
        public SafeSetupDiClassDevsExHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.SetupDiDestroyDeviceInfoList(handle);
        }
    }      
}
