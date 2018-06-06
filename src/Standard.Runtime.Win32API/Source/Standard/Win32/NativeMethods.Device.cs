using System;
using System.Security;
using System.Runtime.InteropServices;

namespace Standard.Win32
{
	partial class NativeMethods
	{
		#region Consts

		/// <summary>
		/// The specified path, file name, or both exceed the system-defined maximum length.
		/// For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. 
		/// </summary>
		public const int MaxPath = 260;

		/// <summary>MaxPathUnicode = 32000</summary>
		public const int MaxPathUnicode = 32000;

		public const int MAX_DEV_LEN = 1000;

		public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
		public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;

		public const int WM_DEVICECHANGE = 0x219;

		public const int DBT_DEVICEARRIVAL = 0x8000;
		public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
		public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
		public const int DBT_DEVTYP_HANDLE = 6;

		public const short DIGCF_PRESENT = 0x2;
		public const short DIGCF_DEVICEINTERFACE = 0x10;
		public const int DIGCF_ALLCLASSES = 0x4;

		public const int SPDRP_ADDRESS = 0x1c;
		public const int SPDRP_BUSNUMBER = 0x15;
		public const int SPDRP_BUSTYPEGUID = 0x13;
		public const int SPDRP_CAPABILITIES = 0xf;
		public const int SPDRP_CHARACTERISTICS = 0x1b;
		public const int SPDRP_CLASS = 7;
		public const int SPDRP_CLASSGUID = 8;
		public const int SPDRP_COMPATIBLEIDS = 2;
		public const int SPDRP_CONFIGFLAGS = 0xa;
		public const int SPDRP_DEVICE_POWER_DATA = 0x1e;
		public const int SPDRP_DEVICEDESC = 0;
		public const int SPDRP_DEVTYPE = 0x19;
		public const int SPDRP_DRIVER = 9;
		public const int SPDRP_ENUMERATOR_NAME = 0x16;
		public const int SPDRP_EXCLUSIVE = 0x1a;
		public const int SPDRP_FRIENDLYNAME = 0xc;
		public const int SPDRP_HARDWAREID = 1;
		public const int SPDRP_LEGACYBUSTYPE = 0x14;
		public const int SPDRP_LOCATION_INFORMATION = 0xd;
		public const int SPDRP_LOWERFILTERS = 0x12;
		public const int SPDRP_MFG = 0xb;
		public const int SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0xe;
		public const int SPDRP_REMOVAL_POLICY = 0x1f;
		public const int SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x20;
		public const int SPDRP_REMOVAL_POLICY_OVERRIDE = 0x21;
		public const int SPDRP_SECURITY = 0x17;
		public const int SPDRP_SECURITY_SDS = 0x18;
		public const int SPDRP_SERVICE = 4;
		public const int SPDRP_UI_NUMBER = 0x10;
		public const int SPDRP_UI_NUMBER_DESC_FORMAT = 0x1d;
		public const int SPDRP_UPPERFILTERS = 0x11;

		public static DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc = new DEVPROPKEY
		{
			fmtid = new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2),
			pid = 4
		};

		#endregion /Consts

		#region Enums

		[Flags]
		public enum DeviceStateChange : uint
		{
			DICS_ENABLE = 1u,
			DICS_DISABLE = 2u,
			DICS_PROPCHANGE = 3u,
			DICS_START = 4u,
			DICS_STOP = 5u
		}

		[Flags]
		public enum DeviceStateChangeScope : uint
		{
			DICS_FLAG_GLOBAL = 1u,
			DICS_FLAG_CONFIGSPECIFIC = 2u,
			DICS_FLAG_CONFIGGENERAL = 4u
		}

		[Flags]
		public enum ClassInstallerFunctionCode : uint
		{
			DIF_SELECTDEVICE = 1u,
			DIF_INSTALLDEVICE = 2u,
			DIF_ASSIGNRESOURCES = 3u,
			DIF_PROPERTIES = 4u,
			DIF_REMOVE = 5u,
			DIF_FIRSTTIMESETUP = 6u,
			DIF_FOUNDDEVICE = 7u,
			DIF_SELECTCLASSDRIVERS = 8u,
			DIF_VALIDATECLASSDRIVERS = 9u,
			DIF_INSTALLCLASSDRIVERS = 10u,
			DIF_CALCDISKSPACE = 11u,
			DIF_DESTROYPRIVATEDATA = 12u,
			DIF_VALIDATEDRIVER = 13u,
			DIF_DETECT = 15u,
			DIF_INSTALLWIZARD = 16u,
			DIF_DESTROYWIZARDDATA = 17u,
			DIF_PROPERTYCHANGE = 18u,
			DIF_ENABLECLASS = 19u,
			DIF_DETECTVERIFY = 20u,
			DIF_INSTALLDEVICEFILES = 21u,
			DIF_UNREMOVE = 22u,
			DIF_SELECTBESTCOMPATDRV = 23u,
			DIF_ALLOW_INSTALL = 24u,
			DIF_REGISTERDEVICE = 25u,
			DIF_NEWDEVICEWIZARD_PRESELECT = 26u,
			DIF_NEWDEVICEWIZARD_SELECT = 27u,
			DIF_NEWDEVICEWIZARD_PREANALYZE = 28u,
			DIF_NEWDEVICEWIZARD_POSTANALYZE = 29u,
			DIF_NEWDEVICEWIZARD_FINISHINSTALL = 30u,
			DIF_UNUSED1 = 31u,
			DIF_INSTALLINTERFACES = 32u,
			DIF_DETECTCANCEL = 33u,
			DIF_REGISTER_COINSTALLERS = 34u,
			DIF_ADDPROPERTYPAGE_ADVANCED = 35u,
			DIF_ADDPROPERTYPAGE_BASIC = 36u,
			DIF_RESERVED1 = 37u,
			DIF_TROUBLESHOOTER = 38u,
			DIF_POWERMESSAGEWAKE = 39u,
			DIF_ADDREMOTEPROPERTYPAGE_ADVANCED = 40u,
			DIF_UPDATEDRIVER_UI = 41u,
			DIF_FINISHINSTALL_ACTION = 42u,
			DIF_RESERVED2 = 48u
		}

		public enum DriverType : uint
		{
			SPDIT_NODRIVER,
			SPDIT_CLASSDRIVER,
			SPDIT_COMPATDRIVER
		}

		[Flags]
		public enum DEVPROPTYPE : uint
		{
			DEVPROP_TYPEMOD_ARRAY = 4096u,
			DEVPROP_TYPEMOD_LIST = 8192u,
			DEVPROP_TYPE_EMPTY = 0u,
			DEVPROP_TYPE_NULL = 1u,
			DEVPROP_TYPE_SBYTE = 2u,
			DEVPROP_TYPE_BYTE = 3u,
			DEVPROP_TYPE_INT16 = 4u,
			DEVPROP_TYPE_UINT16 = 5u,
			DEVPROP_TYPE_INT32 = 6u,
			DEVPROP_TYPE_UINT32 = 7u,
			DEVPROP_TYPE_INT64 = 8u,
			DEVPROP_TYPE_UINT64 = 9u,
			DEVPROP_TYPE_FLOAT = 10u,
			DEVPROP_TYPE_DOUBLE = 11u,
			DEVPROP_TYPE_DECIMAL = 12u,
			DEVPROP_TYPE_GUID = 13u,
			DEVPROP_TYPE_CURRENCY = 14u,
			DEVPROP_TYPE_DATE = 15u,
			DEVPROP_TYPE_FILETIME = 16u,
			DEVPROP_TYPE_BOOLEAN = 17u,
			DEVPROP_TYPE_STRING = 18u,
			DEVPROP_TYPE_SECURITY_DESCRIPTOR = 19u,
			DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING = 20u,
			DEVPROP_TYPE_DEVPROPKEY = 21u,
			DEVPROP_TYPE_DEVPROPTYPE = 22u,
			DEVPROP_TYPE_ERROR = 23u,
			DEVPROP_TYPE_NTSTATUS = 24u,
			DEVPROP_TYPE_STRING_INDIRECT = 25u,
			DEVPROP_TYPE_BINARY = 4099u,
			DEVPROP_TYPE_STRING_LIST = 8210u
		}

		[Flags]
		public enum DeviceControlOptions : uint
		{
			DIGCF_DEFAULT = 1u,
			DIGCF_PRESENT = 2u,
			DIGCF_ALLCLASSES = 4u,
			DIGCF_PROFILE = 8u,
			DIGCF_DEVICEINTERFACE = 16u
		}

		#endregion /Enums

		#region Structs

		public struct SP_PROPCHANGE_PARAMS
		{
			public SP_CLASSINSTALL_HEADER ClassInstallHeader;
			public DeviceStateChange StateChange;
			public DeviceStateChangeScope Scope;
			public uint HwProfile;
		}

		public struct SP_CLASSINSTALL_HEADER
		{
			public uint cbSize;
			public ClassInstallerFunctionCode InstallFunction;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SP_DRVINFO_DATA_V2_W
		{
			public uint cbSize;
			public DriverType Type;
			public ulong Reserved;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string Description;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string MfgName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string ProviderName;

			public long DriverDate;
			public ulong DriverVersion;
		}

		/// <summary>An SP_DEVINFO_DATA structure defines a device instance that is a member of a device information set.</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SP_DEVINFO_DATA
		{
			/// <summary>The size, in bytes, of the SP_DEVINFO_DATA structure.</summary>
			[MarshalAs(UnmanagedType.U4)]
			public uint cbSize;

			/// <summary>The GUID of the device's setup class.</summary>
			public readonly Guid ClassGuid;

			/// <summary>An opaque handle to the device instance (also known as a handle to the devnode).</summary>
			[MarshalAs(UnmanagedType.U4)]
			public readonly uint DevInst;

			/// <summary>Reserved. For internal use only.</summary>
			private readonly IntPtr Reserved;
		}

		/// <summary>An SP_DEVICE_INTERFACE_DETAIL_DATA structure contains the path for a device interface.</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			/// <summary>The size, in bytes, of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.</summary>
			[MarshalAs(UnmanagedType.U4)]
			public uint cbSize;

			/// <summary>The device interface path. This path can be passed to Win32 functions such as CreateFile.</summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
			public readonly string DevicePath;
		}

		/// <summary>An SP_DEVICE_INTERFACE_DATA structure defines a device interface in a device information set.</summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SP_DEVICE_INTERFACE_DATA
		{
			/// <summary>The size, in bytes, of the SP_DEVICE_INTERFACE_DATA structure.</summary>
			[MarshalAs(UnmanagedType.U4)]
			public uint cbSize;

			/// <summary>The GUID for the class to which the device interface belongs.</summary>
			public readonly Guid InterfaceClassGuid;

			/// <summary>Can be one or more of the following: SPINT_ACTIVE (1), SPINT_DEFAULT (2), SPINT_REMOVED (3).</summary>
			[MarshalAs(UnmanagedType.U4)]
			public readonly uint Flags;

			/// <summary>Reserved. Do not use.</summary>
			private readonly IntPtr Reserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DEVPROPKEY
		{
			public Guid fmtid;
			public ulong pid;

			public DEVPROPKEY(string strGuid, ulong id)
			{
				this.fmtid = new Guid(strGuid);
				this.pid = id;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public class DEV_BROADCAST_DEVICEINTERFACE
		{
			public int dbcc_size;
			public int dbcc_devicetype;
			public int dbcc_reserved;
			public Guid dbcc_classguid;
			public short dbcc_name;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class DEV_BROADCAST_DEVICEINTERFACE_1
		{
			public int dbcc_size;
			public int dbcc_devicetype;
			public int dbcc_reserved;
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
			public byte[] dbcc_classguid;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
			public char[] dbcc_name;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class DEV_BROADCAST_HANDLE
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
			public int dbch_handle;
			public int dbch_hdevnotify;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class DEV_BROADCAST_HDR
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
		}

		#endregion /Structs

		// PI

		[DllImport(User32, CharSet = CharSet.Auto)]
		public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, int flags);

		[DllImport(User32, SetLastError = true)]
		public static extern bool UnregisterDeviceNotification(IntPtr handle);
	}
}
