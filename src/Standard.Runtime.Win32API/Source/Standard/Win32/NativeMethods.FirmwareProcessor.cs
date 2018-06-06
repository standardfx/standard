#pragma warning disable 0649
using System;
using System.Security;
using System.Text;
using System.Runtime.InteropServices;

namespace Standard.Win32
{
	partial class NativeMethods
	{
		public enum FIRMWARE_TYPE
		{
			FirmwareTypeUnknown,
			FirmwareTypeBios,
			FirmwareTypeUefi,
			FirmwareTypeMax
		}

		public struct GROUP_AFFINITY
		{
			public uint Mask;
			public ushort Group;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U2)]
			internal ushort[] Reserved;
		}

		public enum ProcessorFeature
		{
			PF_FLOATING_POINT_PRECISION_ERRATA,
			PF_FLOATING_POINT_EMULATED,
			PF_COMPARE_EXCHANGE_DOUBLE,
			PF_MMX_INSTRUCTIONS_AVAILABLE,
			PF_PPC_MOVEMEM_64BIT_OK,
			PF_ALPHA_BYTE_INSTRUCTIONS,
			PF_XMMI_INSTRUCTIONS_AVAILABLE,
			PF_3DNOW_INSTRUCTIONS_AVAILABLE,
			PF_RDTSC_INSTRUCTION_AVAILABLE,
			PF_PAE_ENABLED,
			PF_XMMI64_INSTRUCTIONS_AVAILABLE,
			PF_SSE_DAZ_MODE_AVAILABLE,
			PF_NX_ENABLED,
			PF_SSE3_INSTRUCTIONS_AVAILABLE,
			PF_COMPARE_EXCHANGE128,
			PF_COMPARE64_EXCHANGE128,
			PF_CHANNELS_ENABLED,
			PF_XSAVE_ENABLED,
			PF_ARM_VFP_32_REGISTERS_AVAILABLE,
			PF_ARM_NEON_INSTRUCTIONS_AVAILABLE,
			PF_SECOND_LEVEL_ADDRESS_TRANSLATION,
			PF_VIRT_FIRMWARE_ENABLED,
			PF_RDWRFSGSBASE_AVAILABLE,
			PF_FASTFAIL_AVAILABLE,
			PF_ARM_DIVIDE_INSTRUCTION_AVAILABLE,
			PF_ARM_64BIT_LOADSTORE_ATOMIC,
			PF_ARM_EXTERNAL_CACHE_AVAILABLE,
			PF_ARM_FMAC_INSTRUCTIONS_AVAILABLE
		}

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetNumaHighestNodeNumber(out uint HighestNodeNumber);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetNumaProcessorNode(byte Processor, [MarshalAs(UnmanagedType.LPStr)] [Out] StringBuilder NodeNumber);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint EnumSystemFirmwareTables(uint FirmwareTableProviderSignature, IntPtr pFirmwareTableBuffer, uint BufferSize);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetSystemFirmwareTable(uint FirmwareTableProviderSignature, uint FirmwareTableID, IntPtr pFirmwareTableBuffer, uint BufferSize);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFirmwareType(ref FIRMWARE_TYPE FirmwareType);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetNumaNodeProcessorMaskEx(ushort Node, ref GROUP_AFFINITY ProcessorMask);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsProcessorFeaturePresent(ProcessorFeature ProcessorFeature);

		[DllImport(Kernel32)]
		public static extern ushort GetActiveProcessorGroupCount();

		[DllImport(Kernel32)]
		public static extern uint GetActiveProcessorCount(ushort GroupNumber);
	}
}
#pragma warning restore 0649
