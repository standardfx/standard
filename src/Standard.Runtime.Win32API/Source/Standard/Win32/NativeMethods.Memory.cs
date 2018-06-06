using System;
using System.Runtime.InteropServices;

namespace Standard.Win32
{
    partial class NativeMethods
    {
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class MEMORYSTATUSEX
		{
			public uint dwLength;
			public uint dwMemoryLoad;
			public ulong ullTotalPhys;
			public ulong ullAvailPhys;
			public ulong ullTotalPageFile;
			public ulong ullAvailPageFile;
			public ulong ullTotalVirtual;
			public ulong ullAvailVirtual;
			public ulong ullAvailExtendedVirtual;

			public MEMORYSTATUSEX()
			{
				this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
			}
		}

		[DllImport(Kernel32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryKb);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

		/// <summary>Frees the specified local memory object and invalidates its handle.</summary>
		/// <returns>
		/// If the function succeeds, the return value is <see langword="null"/>.
		/// If the function fails, the return value is equal to a handle to the local memory object. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>SetLastError is set to <see langword="false"/>.</remarks>
		/// <remarks>
		/// Note  The local functions have greater overhead and provide fewer features than other memory management functions.
		/// New applications should use the heap functions unless documentation states that a local function should be used.
		/// For more information, see Global and Local Functions.
		/// </remarks>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Kernel32, SetLastError = false, CharSet = CharSet.Unicode)]
        public static extern IntPtr LocalFree(IntPtr hMem);
    }
}
