using System;
using System.Runtime.InteropServices;

namespace Standard.Win32.SafeHandles
{
    /// <summary>
    /// Base class for Safe handles with <c>Null IntPtr</c> as invalid
    /// </summary>
    public abstract class ZeroInvalidHandle : SafeHandle
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected ZeroInvalidHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// Determines if this is a valid handle
        /// </summary>
        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }
}

