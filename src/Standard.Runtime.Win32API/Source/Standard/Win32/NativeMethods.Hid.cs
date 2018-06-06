using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Standard.Win32
{
    partial class NativeMethods
    {
		public const short HIDP_INPUT = 0;
		public const short HIDP_OUTPUT = 1;


		// structs

		[StructLayout(LayoutKind.Sequential)]
        public struct HIDD_ATTRIBUTES
        {
			public int Size;
			public ushort VendorID;
			public ushort ProductID;
			public short VersionNumber;
        }

        [StructLayout(LayoutKind.Sequential)]
		public struct HIDP_CAPS
        {
			public short Usage;
			public short UsagePage;
			public short InputReportByteLength;
			public short OutputReportByteLength;
			public short FeatureReportByteLength;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
			public short[] Reserved;

			public short NumberLinkCollectionNodes;
			public short NumberInputButtonCaps;
			public short NumberInputValueCaps;
			public short NumberInputDataIndices;
			public short NumberOutputButtonCaps;
			public short NumberOutputValueCaps;
			public short NumberOutputDataIndices;
			public short NumberFeatureButtonCaps;
			public short NumberFeatureValueCaps;
			public short NumberFeatureDataIndices;
        }

        [StructLayout(LayoutKind.Sequential)]
		public struct HIDP_VALUE_CAPS
        {
			public short UsagePage;
			public byte ReportID;
			public int IsAlias;
			public short BitField;
			public short LinkCollection;
			public short LinkUsage;
			public short LinkUsagePage;
			public int IsRange;
			public int IsStringRange;
			public int IsDesignatorRange;
			public int IsAbsolute;
			public int HasNull;
			public byte Reserved;
			public short BitSize;
			public short ReportCount;
			public short Reserved2;
			public short Reserved3;
			public short Reserved4;
			public short Reserved5;
			public short Reserved6;
			public int LogicalMin;
			public int LogicalMax;
			public int PhysicalMin;
			public int PhysicalMax;
			public short UsageMin;
			public short UsageMax;
			public short StringMin;
			public short StringMax;
			public short DesignatorMin;
			public short DesignatorMax;
			public short DataIndexMin;
			public short DataIndexMax;
        }


		// PI
		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_FlushQueue(SafeFileHandle handle);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_GetAttributes(SafeFileHandle handle, ref HIDD_ATTRIBUTES attributes);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_GetFeature(SafeFileHandle handle, byte[] lpReportBuffer, int reportBufferLength);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_GetInputReport(SafeFileHandle handle, ref byte lpReportBuffer, int reportBufferLength);

		[DllImport(Hid, SetLastError = true)]
		public static extern void HidD_GetHidGuid(ref Guid hidGuid);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_GetNumInputBuffers(SafeFileHandle handle, ref int numberBuffers);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_GetPreparsedData(SafeFileHandle handle, ref IntPtr preparsedData);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_SetFeature(SafeFileHandle handle, byte[] lpReportBuffer, int reportBufferLength);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_SetNumInputBuffers(SafeFileHandle handle, int numberBuffers);

		[DllImport(Hid, SetLastError = true)]
		public static extern bool HidD_SetOutputReport(SafeFileHandle handle, byte[] lpReportBuffer, int reportBufferLength);

		[DllImport(Hid, SetLastError = true)]
		public static extern int HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

		[DllImport(Hid, SetLastError = true)]
		public static extern int HidP_GetValueCaps(short reportType, ref byte valueCaps, ref short valueCapsLength, IntPtr preparsedData);

		[DllImport(Hid, CharSet = CharSet.Unicode)]
		public static extern bool HidD_GetProductString(SafeFileHandle handle, ref byte lpReportBuffer, int ReportBufferLength);

		[DllImport(Hid, CharSet = CharSet.Unicode)]
		public static extern bool HidD_GetManufacturerString(SafeFileHandle handle, ref byte lpReportBuffer, int ReportBufferLength);

		[DllImport(Hid, CharSet = CharSet.Unicode)]
		public static extern bool HidD_GetSerialNumberString(SafeFileHandle handle, ref byte lpReportBuffer, int reportBufferLength);
	}
}
