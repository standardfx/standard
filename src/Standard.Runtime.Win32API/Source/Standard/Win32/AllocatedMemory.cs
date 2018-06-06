using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Standard.Win32
{
    public sealed class AllocatedMemory : IDisposable
    {
        private IntPtr pointer;

        [SecuritySafeCritical]        
        public AllocatedMemory(int bytesRequired)
        {
            this.pointer = Marshal.AllocHGlobal(bytesRequired);
        }

        ~AllocatedMemory()
        {
            this.InternalDispose();
        }

        public IntPtr Pointer
        {
            get
            {
                return this.pointer;
            }
        }

        public void Dispose()
        {
            this.InternalDispose();
            GC.SuppressFinalize(this);
        }

        [SecuritySafeCritical]        
        private void InternalDispose()
        {
            if (this.pointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.pointer);
                this.pointer = IntPtr.Zero;
            }
        }
    }
}
