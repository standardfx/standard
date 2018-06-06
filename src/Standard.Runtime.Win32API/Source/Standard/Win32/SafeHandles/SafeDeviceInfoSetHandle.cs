using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Standard.Win32.SafeHandles
{
    public class SafeDeviceInfoSetHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeDeviceInfoSetHandle() 
        	: base(true)
        { }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.SetupDiDestroyDeviceInfoList(handle);
        }
    }
}
