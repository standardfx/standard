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
    /// Safe wrapper for HANDLE to a token.
    /// </summary>
    /// <devdoc>http://msdn.microsoft.com/en-us/magazine/cc163823.aspx</devdoc>
    public class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// This safehandle instance "owns" the handle, hence base(true)
        /// is being called. When safehandle is no longer in use it will
        /// call this class's ReleaseHandle method which will release
        /// the resources
        /// </summary>
        private SafeTokenHandle() 
            : base(true) 
        { }

        // 0 is an Invalid Handle
        public SafeTokenHandle(HANDLE handle)
            : base(true)
        {
            SetHandle(handle);
        }

        public static SafeTokenHandle InvalidHandle
        {
            get { return new SafeTokenHandle(HANDLE.Zero); }
        }

        /// <summary>
        /// Release the HANDLE held by this instance
        /// </summary>
        /// <returns>true if the release was successful. false otherwise.</returns>        
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }
}
