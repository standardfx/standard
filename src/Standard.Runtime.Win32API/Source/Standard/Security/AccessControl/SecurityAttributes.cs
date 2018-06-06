using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Standard.Win32.SafeHandles;

namespace Standard.Security.AccessControl
{
	/// <summary>Class used to represent the SECURITY_ATTRIBUTES native Win32 structure. It provides initialization function from an <see cref="ObjectSecurity"/> object.</summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public sealed class SecurityAttributes : IDisposable
    {
        // Removing this member results in: "Invalid access to memory location: ..."
        [MarshalAs(UnmanagedType.U4)]
        private int length;

        private readonly SafeGlobalMemoryBufferHandle securityDescriptor;

        public SecurityAttributes(ObjectSecurity securityDescriptor)
        {
            SafeGlobalMemoryBufferHandle safeBuffer = ToUnmanagedSecurityAttributes(securityDescriptor);
            this.length = safeBuffer.Capacity;
            this.securityDescriptor = safeBuffer;
        }

        /// <summary>Marshals an ObjectSecurity instance to unmanaged memory.</summary>
        /// <returns>A safe handle containing the marshalled security descriptor.</returns>
        /// <param name="securityDescriptor">The security descriptor.</param>
		[SecuritySafeCritical]
        private static SafeGlobalMemoryBufferHandle ToUnmanagedSecurityAttributes(ObjectSecurity securityDescriptor)
        {
            if (securityDescriptor == null)
                return new SafeGlobalMemoryBufferHandle();
            
            byte[] src = securityDescriptor.GetSecurityDescriptorBinaryForm();
            var safeBuffer = new SafeGlobalMemoryBufferHandle(src.Length);

            try
            {
                safeBuffer.CopyFrom(src, 0, src.Length);
                return safeBuffer;
            }
            catch
            {
                safeBuffer.Close();
                throw;
            }
        }

        public void Dispose()
        {
            if (this.securityDescriptor != null)
                this.securityDescriptor.Close();
        }
    }
}
