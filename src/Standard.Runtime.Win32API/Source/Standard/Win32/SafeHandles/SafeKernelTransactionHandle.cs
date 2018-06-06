using Microsoft.Win32.SafeHandles;
using System.Security;

namespace Standard.Win32.SafeHandles
{
    /// <summary>
    /// Provides a concrete implementation of SafeHandle supporting transactions.
    /// </summary>
    public class SafeKernelTransactionHandle : SafeHandleMinusOneIsInvalid
    {
        /// <summary>
	    /// Initializes a new instance of the <see cref="SafeKernelTransactionHandle"/> class.
	    /// </summary>      
        public SafeKernelTransactionHandle()
            : base(true)
        { }

        /// <summary>
	    /// When overridden in a derived class, executes the code required to free the handle.
	    /// </summary>
        /// <returns>
	    /// <see langword="true"/> if the handle is released successfully; otherwise, in the event of a catastrophic failure, <see langword="false"/>. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
	    /// </returns>
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }
}
