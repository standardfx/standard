using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Standard.Win32
{
    partial class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PINHERITED_FROM
        {
            public Int32 GenerationGap;

			[MarshalAs(UnmanagedType.LPTStr)]
            public string AncestorName;
        }

		[DllImport(Advapi32, EntryPoint = "GetInheritanceSourceW", CharSet = CharSet.Unicode)]
		public static extern UInt32 GetInheritanceSource(
			[MarshalAs(UnmanagedType.LPTStr)] string pObjectName,
			ResourceType ObjectType,
			SECURITY_INFORMATION SecurityInfo,
			[MarshalAs(UnmanagedType.Bool)]bool Container,
			IntPtr pObjectClassGuids,
			UInt32 GuidCount,
			byte[] pAcl,
			IntPtr pfnArray,
			ref GENERIC_MAPPING pGenericMapping,
			IntPtr pInheritArray);

		[DllImport(Advapi32, EntryPoint = "FreeInheritedFromArray", CharSet = CharSet.Unicode)]
		public static extern UInt32 FreeInheritedFromArray(
		   IntPtr pInheritArray,
		   UInt16 AceCnt,
		   IntPtr pfnArray);
	}
}
