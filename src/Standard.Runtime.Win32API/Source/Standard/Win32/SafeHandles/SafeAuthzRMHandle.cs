using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;
using HANDLE = System.IntPtr;

namespace Standard.Win32.SafeHandles
{
    /// <summary>
    /// Safe wrapper for AUTHZ_RESOURCE_MANAGER_HANDLE.
    /// </summary>
    public class SafeAuthzRMHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// This safehandle instance "owns" the handle, hence base(true)
        /// is being called. When safehandle is no longer in use it will
        /// call this class's ReleaseHandle method which will release
        /// the resources
        /// </summary>
        SafeAuthzRMHandle() 
			: base(true)
		{ }

        SafeAuthzRMHandle(HANDLE handle)
            : base(true)
        {
            SetHandle(handle);
        }

        public static SafeAuthzRMHandle InvalidHandle
        {
            get { return new SafeAuthzRMHandle(HANDLE.Zero); }
        }

        /// <summary>
        /// Release the resource manager handle held by this instance
        /// </summary>
        /// <returns>true if the release was successful. false otherwise.</returns>        
        protected override bool ReleaseHandle()
        {
            return NativeMethods.AuthzFreeResourceManager(handle);
        }
    }
}
