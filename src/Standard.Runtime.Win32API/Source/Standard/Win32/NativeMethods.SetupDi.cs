using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Standard.Win32.SafeHandles;

namespace Standard.Win32
{
	partial class NativeMethods
	{
		/// <summary>Specifies control options that filter the device information elements that are added to the device information set.</summary>
		[Flags]
		public enum SetupDiGetClassDevsExFlags
		{
			/// <summary>DIGCF_DEFAULT
			/// <para>Return only the device that is associated with the system default device interface, if one is set, for the specified device interface classes.</para>
			/// </summary>
			Default = 1, // only valid with DIGCF_DEVICEINTERFACE

			/// <summary>DIGCF_PRESENT
			/// <para>Return only devices that are currently present.</para>
			/// </summary>
			Present = 2,

			/// <summary>DIGCF_ALLCLASSES
			/// <para>Return a list of installed devices for the specified device setup classes or device interface classes.</para>
			/// </summary>
			AllClasses = 4,

			/// <summary>DIGCF_PROFILE
			/// <para>Return only devices that are a part of the current hardware profile.</para>
			/// </summary>
			Profile = 8,

			/// <summary>DIGCF_DEVICEINTERFACE
			/// <para>
			/// Return devices that support device interfaces for the specified device interface classes.
			/// This flag must be set in the Flags parameter if the Enumerator parameter specifies a Device Instance ID. 
			/// </para>
			/// </summary>
			DeviceInterface = 16,
		}

		/// <summary>Flags for SetupDiGetDeviceRegistryProperty().</summary>
		public enum SetupDiGetDeviceRegistryPropertyEnum
		{
			/// <summary>SPDRP_DEVICEDESC
			/// <para>Represents a description of a device instance.</para>
			/// </summary>
			DeviceDescription = 0,

			/// <summary>SPDRP_HARDWAREID
			/// <para>Represents the list of hardware identifiers for a device instance.</para>
			/// </summary>
			HardwareId = 1,

			/// <summary>SPDRP_COMPATIBLEIDS
			/// <para>Represents the list of compatible identifiers for a device instance.</para>
			/// </summary>
			CompatibleIds = 2,

			//SPDRP_UNUSED0 = 0x00000003,

			/// <summary>SPDRP_CLASS
			/// <para>Represents the name of the service that is installed for a device instance.</para>
			/// </summary>
			Service = 4,

			//SPDRP_UNUSED1 = 0x00000005,
			//SPDRP_UNUSED2 = 0x00000006,

			/// <summary>SPDRP_CLASS
			/// <para>Represents the name of the device setup class that a device instance belongs to.</para>
			/// </summary>
			Class = 7,

			/// <summary>SPDRP_CLASSGUID
			/// <para>Represents the <see cref="System.Guid"/> of the device setup class that a device instance belongs to.</para>
			/// </summary>
			ClassGuid = 8,

			/// <summary>SPDRP_DRIVER
			/// <para>Represents the registry entry name of the driver key for a device instance.</para>
			/// </summary>
			Driver = 9,

			///// <summary>SPDRP_CONFIGFLAGS
			///// Represents the configuration flags that are set for a device instance.
			///// </summary>
			//ConfigurationFlags = 10,

			/// <summary>SPDRP_MFG
			/// <para>Represents the name of the manufacturer of a device instance.</para>
			/// </summary>
			Manufacturer = 11,

			/// <summary>SPDRP_FRIENDLYNAME
			/// <para>Represents the friendly name of a device instance.</para>
			/// </summary>
			FriendlyName = 12,

			/// <summary>SPDRP_LOCATION_INFORMATION
			/// <para>Represents the bus-specific physical location of a device instance.</para>
			/// </summary>
			LocationInformation = 13,

			/// <summary>SPDRP_PHYSICAL_DEVICE_LOCATION
			/// <para>Encapsulates the physical device location information provided by a device's firmware to Windows.</para>
			/// </summary>
			PhysicalDeviceObjectName = 14,

			///// <summary>SPDRP_CAPABILITIES
			//// <para>Represents the capabilities of a device instance.</para>
			//// </summary>
			//Capabilities = 15,

			///// <summary>SPDRP_UI_NUMBER - Represents a number for the device instance that can be displayed in a user interface item.</summary>
			//UiNumber = 16,

			///// <summary>SPDRP_UPPERFILTERS - Represents a list of the service names of the upper-level filter drivers that are installed for a device instance.</summary>
			//UpperFilters = 17,

			///// <summary>SPDRP_LOWERFILTERS - Represents a list of the service names of the lower-level filter drivers that are installed for a device instance.</summary>
			//LowerFilters = 18,

			///// <summary>SPDRP_BUSTYPEGUID - Represents the <see cref="Guid"/> that identifies the bus type of a device instance.</summary>
			//BusTypeGuid = 19,

			///// <summary>SPDRP_LEGACYBUSTYPE - Represents the legacy bus number of a device instance.</summary>
			//LegacyBusType = 20,

			///// <summary>SPDRP_BUSNUMBER - Represents the number that identifies the bus instance that a device instance is attached to.</summary>
			//BusNumber = 21,

			/// <summary>SPDRP_ENUMERATOR_NAME
			/// <para>Represents the name of the enumerator for a device instance.</para>
			/// </summary>
			EnumeratorName = 22,

			///// <summary>SPDRP_SECURITY - Represents a security descriptor structure for a device instance.</summary>
			//Security = 23,

			///// <summary>SPDRP_SECURITY_SDS - Represents a security descriptor string for a device instance.</summary>
			//SecuritySds = 24,

			///// <summary>SPDRP_DEVTYPE - Represents the device type of a device instance.</summary>
			//DeviceType = 25,

			///// <summary>SPDRP_EXCLUSIVE - Represents a Boolean value that determines whether a device instance can be opened for exclusive use.</summary>
			//Exclusive = 26,

			///// <summary>SPDRP_CHARACTERISTICS - Represents the characteristics of a device instance.</summary>
			//Characteristics = 27,

			///// <summary>SPDRP_ADDRESS - Represents the bus-specific address of a device instance.</summary>
			//Address = 28,

			///// <summary>SPDRP_UI_NUMBER_DESC_FORMAT - Represents a printf-compatible format string that you should use to display the value of the <see cref="UiNumber"/> device property for a device instance.</summary>
			//UiNumberDescriptionFormat = 29,

			///// <summary>SPDRP_DEVICE_POWER_DATA - Represents power information about a device instance.</summary>
			//DevicePowerData = 30,

			///// <summary>SPDRP_REMOVAL_POLICY - Represents the current removal policy for a device instance.</summary>
			//RemovalPolicy = 31,

			///// <summary>SPDRP_REMOVAL_POLICY_HW_DEFAULT - Represents the default removal policy for a device instance.</summary>
			//RemovalPolicyDefault = 32,

			///// <summary>SPDRP_REMOVAL_POLICY_OVERRIDE- Represents the removal policy override for a device instance.</summary>
			//RemovalPolicyOverride = 33,

			///// <summary>SPDRP_INSTALL_STATE - Represents the installation state of a device instance.</summary>
			//InstallState = 34,

			/// <summary>SPDRP_LOCATION_PATHS
			/// <para>Represents the location of a device instance in the device tree.</para>
			/// </summary>
			LocationPaths = 35,

			/// <summary>SPDRP_BASE_CONTAINERID
			/// <para>Represents the <see cref="System.Guid"/> value of the base container identifier (ID) .The Windows Plug and Play (PnP) manager assigns this value to the device node (devnode).</para>
			/// </summary>
			BaseContainerId = 36
		}


		[DllImport(SetupApi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetupDiGetDevicePropertyKeys(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, IntPtr propertyKeyArray, uint propertyKeyCount, ref uint requiredPropertyKeyCount, uint flags);

		[DllImport(SetupApi, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiSetClassInstallParams([In] IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData, [In] ref SP_PROPCHANGE_PARAMS classInstallParams, uint classInstallParamsSize);

		[DllImport(SetupApi, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiCallClassInstaller(ClassInstallerFunctionCode installFunction, [In] IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData);

		[DllImport(SetupApi, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiBuildDriverInfoList([In] IntPtr deviceInfoSet, IntPtr deviceInfoData, DriverType type);

		[DllImport(SetupApi, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiEnumDriverInfo([In] IntPtr deviceInfoSet, [In] IntPtr deviceInfoData, DriverType type, uint memberIndex, ref SP_DRVINFO_DATA_V2_W driverInfoData);

		[DllImport(SetupApi, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiDestroyDriverInfoList([In] IntPtr deviceInfoSet, [In] IntPtr deviceInfoData, DriverType type);

		[DllImport(Newdev, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DiInstallDriver(IntPtr hwndParent, [MarshalAs(UnmanagedType.LPTStr)] [In] string fullInfPath, uint flags, ref bool needReboot);

		/// <summary>
		///   The SetupDiDestroyDeviceInfoList function deletes a device information set and frees all associated memory.
		/// </summary>
		/// <remarks>
		///   <para>SetLastError is set to <see langword="false"/>.</para>
		///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
		/// </remarks>
		/// <param name="hDevInfo">Information describing the development.</param>
		/// <returns>
		///   <para>The function returns TRUE if it is successful.</para>
		///   <para>Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.</para>
		/// </returns>
		[DllImport(SetupApi, SetLastError = false, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

		/// <summary>
		///   The SetupDiEnumDeviceInterfaces function enumerates the device interfaces that are contained in a device information set.
		/// </summary>
		/// <remarks>
		///   <para>Repeated calls to this function return an <see cref="SP_DEVICE_INTERFACE_DATA"/> structure for a different device
		///   interface.</para>
		///   <para>This function can be called repeatedly to get information about interfaces in a device information set that are
		///   associated</para>
		///   <para>with a particular device information element or that are associated with all device information elements.</para>
		///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
		/// </remarks>
		/// <param name="hDevInfo">Information describing the development.</param>
		/// <param name="devInfo">Information describing the development.</param>
		/// <param name="interfaceClassGuid">[in,out] Unique identifier for the interface class.</param>
		/// <param name="memberIndex">Zero-based index of the member.</param>
		/// <param name="deviceInterfaceData">[in,out] Information describing the device interface.</param>
		/// <returns>
		///   <para>SetupDiEnumDeviceInterfaces returns TRUE if the function completed without error.</para>
		///   <para>If the function completed with an error, FALSE is returned and the error code for the failure can be retrieved by calling
		///   GetLastError.</para>
		/// </returns>  
		[DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiEnumDeviceInterfaces(
            SafeHandle hDevInfo, 
            IntPtr devInfo, 
            ref Guid interfaceClassGuid, 
            [MarshalAs(UnmanagedType.U4)] uint memberIndex, 
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

		[DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiEnumDeviceInterfaces(
			IntPtr deviceInfoSet, 
			ref SP_DEVINFO_DATA deviceInfoData, 
			ref Guid interfaceClassGuid, 
			int memberIndex, 
			ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

		/// <summary>
		///   The SetupDiGetClassDevsEx function returns a handle to a device information set that contains requested device information elements
		///   for a local or a remote computer.
		/// </summary>
		/// <remarks>
		///   <para>The caller of SetupDiGetClassDevsEx must delete the returned device information set when it is no longer needed by calling
		///   <see cref="SetupDiDestroyDeviceInfoList"/>.</para>
		///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
		/// </remarks>
		/// <param name="classGuid">[in,out] Unique identifier for the class.</param>
		/// <param name="enumerator">The enumerator.</param>
		/// <param name="hwndParent">The parent.</param>
		/// <param name="devsExFlags">The devs ex flags.</param>
		/// <param name="deviceInfoSet">Set the device information belongs to.</param>
		/// <param name="machineName">Name of the machine.</param>
		/// <param name="reserved">The reserved.</param>
		/// <returns>
		///   <para>If the operation succeeds, SetupDiGetClassDevsEx returns a handle to a device information set that contains all installed
		///   devices that matched the supplied parameters.</para>
		///   <para>If the operation fails, the function returns INVALID_HANDLE_VALUE. To get extended error information, call
		///   GetLastError.</para>
		/// </returns>
		[DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern SafeSetupDiClassDevsExHandle SetupDiGetClassDevsEx(
            ref Guid classGuid, 
            IntPtr enumerator, 
            IntPtr hwndParent, 
            [MarshalAs(UnmanagedType.U4)] SetupDiGetClassDevsExFlags devsExFlags, 
            IntPtr deviceInfoSet, 
            [MarshalAs(UnmanagedType.LPWStr)] string machineName, 
            IntPtr reserved);

		/// <summary>
		///   The SetupDiGetDeviceInterfaceDetail function returns details about a device interface.
		/// </summary>
		/// <remarks>
		///   <para>The interface detail returned by this function consists of a device path that can be passed to Win32 functions such as
		///   CreateFile.</para>
		///   <para>Do not attempt to parse the device path symbolic name. The device path can be reused across system starts.</para>
		///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
		/// </remarks>
		/// <param name="hDevInfo">Information describing the development.</param>
		/// <param name="deviceInterfaceData">[in,out] Information describing the device interface.</param>
		/// <param name="deviceInterfaceDetailData">[in,out] Information describing the device interface detail.</param>
		/// <param name="deviceInterfaceDetailDataSize">Size of the device interface detail data.</param>
		/// <param name="requiredSize">Size of the required.</param>
		/// <param name="deviceInfoData">[in,out] Information describing the device information.</param>
		/// <returns>
		///   <para>SetupDiGetDeviceInterfaceDetail returns TRUE if the function completed without error.</para>
		///   <para>If the function completed with an error, FALSE is returned and the error code for the failure can be retrieved by calling
		///   GetLastError.</para>
		/// </returns>
		[DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(
            SafeHandle hDevInfo, 
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, 
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, 
            [MarshalAs(UnmanagedType.U4)] uint deviceInterfaceDetailDataSize, 
            IntPtr requiredSize, 
            ref SP_DEVINFO_DATA deviceInfoData);

		[DllImport(SetupApi, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(
			IntPtr deviceInfoSet, 
			ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, 
			ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, 
			int deviceInterfaceDetailDataSize, 
			ref int requiredSize, 
			IntPtr deviceInfoData);

		/// <summary>
		///   The SetupDiGetDeviceRegistryProperty function retrieves a specified Plug and Play device property.
		/// </summary>
		/// <remarks><para>Available in Microsoft Windows 2000 and later versions of Windows.</para></remarks>
		/// <param name="deviceInfoSet">Set the device information belongs to.</param>
		/// <param name="deviceInfoData">[in,out] Information describing the device information.</param>
		/// <param name="property">The property.</param>
		/// <param name="propertyRegDataType">[out] Type of the property register data.</param>
		/// <param name="propertyBuffer">Buffer for property data.</param>
		/// <param name="propertyBufferSize">Size of the property buffer.</param>
		/// <param name="requiredSize">Size of the required.</param>
		/// <returns>
		///   <para>SetupDiGetDeviceRegistryProperty returns TRUE if the call was successful.</para>
		///   <para>Otherwise, it returns FALSE and the logged error can be retrieved by making a call to GetLastError.</para>
		///   <para>SetupDiGetDeviceRegistryProperty returns the ERROR_INVALID_DATA error code if the requested property does not exist for a
		///   device or if the property data is not valid.</para>
		/// </returns>
		[DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceRegistryProperty(
            SafeHandle deviceInfoSet, 
            ref SP_DEVINFO_DATA deviceInfoData, 
            SetupDiGetDeviceRegistryPropertyEnum property, 
            [MarshalAs(UnmanagedType.U4)] out uint propertyRegDataType, 
            SafeGlobalMemoryBufferHandle propertyBuffer, 
            [MarshalAs(UnmanagedType.U4)] uint propertyBufferSize, 
            IntPtr requiredSize);

		[DllImport(SetupApi, EntryPoint = "SetupDiGetDeviceRegistryProperty")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceRegistryProperty(
			IntPtr deviceInfoSet, 
			ref SP_DEVINFO_DATA deviceInfoData, 
			int propertyVal, 
			ref int propertyRegDataType, 
			byte[] propertyBuffer, 
			int propertyBufferSize, 
			ref int requiredSize);

		[DllImport(SetupApi, EntryPoint = "SetupDiGetDevicePropertyW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceProperty(
			IntPtr deviceInfoSet,
			[In] ref SP_DEVINFO_DATA deviceInfoData,
			[In] ref DEVPROPKEY propertyKey,
			out DEVPROPTYPE propertyType,
			byte[] propertyBuffer,
			uint propertyBufferSize,
			ref uint requiredSize,
			uint flags);

		[DllImport(SetupApi, SetLastError = true)]
		public static extern bool SetupDiEnumDeviceInfo(
			IntPtr deviceInfoSet, 
			int memberIndex, 
			ref SP_DEVINFO_DATA deviceInfoData);

		[DllImport(SetupApi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetupDiCreateDeviceInfoList([In] IntPtr classGuid, [In] IntPtr hwndParent);

		[DllImport(SetupApi, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetupDiGetClassDevs(
			ref Guid classGuid,
			[MarshalAs(UnmanagedType.LPWStr)] [In] string enumerator,
			[In] IntPtr hwndParent,
			DeviceControlOptions flags);

		[DllImport(SetupApi, CharSet = CharSet.Auto, EntryPoint = "SetupDiGetDeviceInterfaceDetail", SetLastError = true)]
		public static extern bool SetupDiGetDeviceInterfaceDetailBuffer(
			IntPtr deviceInfoSet, 
			ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
			IntPtr deviceInterfaceDetailData, 
			int deviceInterfaceDetailDataSize, 
			ref int requiredSize, 
			IntPtr deviceInfoData);
	}
}
