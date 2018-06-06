using System;
using System.Runtime.InteropServices;
using Standard.Security.Privileges;
using System.Text;
using Standard.Win32.SafeHandles;
using Standard.Security.AccessControl;

namespace Standard.Win32
{
    partial class NativeMethods
    {
		public enum SECURITY_INFORMATION
		{
			OWNER_SECURITY_INFORMATION = 1,
			GROUP_SECURITY_INFORMATION = 2,
			DACL_SECURITY_INFORMATION = 4,
			SACL_SECURITY_INFORMATION = 8,
		}

		[Flags]
		public enum SecurityInformationClass : uint
		{
			Owner = 0x00001,
			Group = 0x00002,
			Dacl = 0x00004,
			Sacl = 0x00008,
			Label = 0x00010,
			Attribute = 0x00020,
			Scope = 0x00040
		}

		/// <summary>The GENERIC_MAPPING structure defines the mapping of generic access rights to specific and standard access rights for an object. When a client application requests generic access to an object, that request is mapped to the access rights defined in this structure.</summary>
		/// <devdoc>https://msdn.microsoft.com/en-us/library/windows/desktop/aa446633(v=vs.85).aspx</devdoc>
		[StructLayout(LayoutKind.Sequential)]
		public struct GENERIC_MAPPING
		{
			public uint GenericRead;
			public uint GenericWrite;
			public uint GenericExecute;
			public uint GenericAll;
		}

		// https://msdn.microsoft.com/en-us/library/windows/desktop/aa379607(v=vs.85).aspx

		/// <summary>
		/// <para>Each type of securable object has a set of access rights that correspond to operations specific to that type of object. In addition to these object-specific access rights, there is a set of standard access rights that correspond to operations common to most types of securable objects.</para>
		/// <para>The access mask format includes a set of bits for the standard access rights. The following Windows constants for standard access rights are defined in Winnt.h.</para>
		/// </summary>
		[Flags]
		public enum StdAccess : uint
		{
			None = 0x0,

			SYNCHRONIZE = 0x100000,
			STANDARD_RIGHTS_REQUIRED = 0xF0000,

			MAXIMUM_ALLOWED = 0x2000000,
		}

		[Flags]
		public enum AccessTypeMasks
		{
			Delete = 65536,
			ReadControl = 131072,
			WriteDAC = 262144,
			WriteOwner = 524288,
			Synchronize = 1048576,
			StandardRightsRequired = 983040,
			StandardRightsRead = ReadControl,
			StandardRightsWrite = ReadControl,
			StandardRightsExecute = ReadControl,
			StandardRightsAll = 2031616,
			SpecificRightsAll = 65535
		}

		public enum GenericRights : uint
		{
			GENERIC_READ = 0x80000000,
			GENERIC_WRITE = 0x40000000,
			GENERIC_EXECUTE = 0x20000000,
			GENERIC_ALL = 0x10000000
		}

		/*
		internal enum MappedGenericRights : uint
		{
			FILE_GENERIC_EXECUTE = FileSystemRights.ExecuteFile | FileSystemRights.ReadPermissions | FileSystemRights.ReadAttributes | FileSystemRights.Synchronize,
			FILE_GENERIC_READ = FileSystemRights.ReadAttributes | FileSystemRights.ReadData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.ReadPermissions | FileSystemRights.Synchronize,
			FILE_GENERIC_WRITE = FileSystemRights.AppendData | FileSystemRights.WriteAttributes | FileSystemRights.WriteData | FileSystemRights.WriteExtendedAttributes | FileSystemRights.ReadPermissions | FileSystemRights.Synchronize,
			FILE_GENERIC_ALL = FileSystemRights.FullControl
		}
		*/

		public enum TokenInformationClass
		{
			None,
			TokenUser,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin,
			TokenElevationType,
			TokenLinkedToken,
			TokenElevation,
			TokenHasRestrictions,
			TokenAccessInformation,
			TokenVirtualizationAllowed,
			TokenVirtualizationEnabled,
			TokenIntegrityLevel,
			TokenUIAccess,
			TokenMandatoryPolicy,
			TokenLogonSid,
			MaxTokenInfoClass
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Luid
        {
			public uint LowPart;
			public uint HighPart;

            public static Luid NullLuid
            {
                get
                {
                    Luid Empty;
                    Empty.LowPart = 0;
                    Empty.HighPart = 0;

                    return Empty;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct TokenPrivileges
        {
            internal uint PrivilegeCount;
            internal LuidAndAttributes Privilege;
            //internal Luid Luid
            //internal uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct LuidAndAttributes
        {
            internal Luid Luid;
            internal PrivilegeAttributes Attributes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
		public struct SECURITY_DESCRIPTOR
        {
            public byte Revision;
            public byte Sbz1;
            public long Control;
            public long Owner;
            public long Group;
            public ACL Sacl;
            public ACL Dacl;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
		public struct ACL
        {
            public byte AclRevision;
            public byte Sbz1;
            public int AclSize;
            public int AceCount;
            public int Sbz2;
        }

		/// <summary>The GetNamedSecurityInfo function retrieves a copy of the security descriptor for an object specified by name.
		/// <returns>
		/// <para>If the function succeeds, the return value is ERROR_SUCCESS.</para>
		/// <para>If the function fails, the return value is a nonzero error code defined in WinError.h.</para>
		/// </returns>
		/// <remarks>
		/// <para>Minimum supported client: Windows XP [desktop apps only]</para>
		/// <para>Minimum supported server: Windows Server 2003 [desktop apps only]</para>
		/// </remarks>
		/// </summary>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetNamedSecurityInfoW")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern uint GetNamedSecurityInfo(
			[MarshalAs(UnmanagedType.LPWStr)] string pObjectName,
			ObjectType objectType,
			SecurityInformation securityInfo,
			out IntPtr pSidOwner,
			out IntPtr pSidGroup,
			out IntPtr pDacl,
			out IntPtr pSacl,
			out SafeGlobalMemoryBufferHandle pSecurityDescriptor);

		/// <summary>The GetSecurityInfo function retrieves a copy of the security descriptor for an object specified by a handle.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero.
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern uint GetSecurityInfo(
			SafeHandle handle,
			ObjectType objectType,
			SecurityInformation securityInfo,
			out IntPtr pSidOwner,
			out IntPtr pSidGroup,
			out IntPtr pDacl,
			out IntPtr pSacl,
			out SafeGlobalMemoryBufferHandle pSecurityDescriptor);

		/// <summary>The SetSecurityInfo function sets specified security information in the security descriptor of a specified object. 
		/// The caller identifies the object by a handle.</summary>
		/// <returns>
		/// If the function succeeds, the function returns ERROR_SUCCESS.
		/// If the function fails, it returns a nonzero error code defined in WinError.h.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern uint SetSecurityInfo(
			SafeHandle handle,
			ObjectType objectType,
			SecurityInformation securityInfo,
			IntPtr psidOwner,
			IntPtr psidGroup,
			IntPtr pDacl,
			IntPtr pSacl);

		/// <summary>The SetNamedSecurityInfo function sets specified security information in the security descriptor of a specified object. The caller identifies the object by name.
		/// <returns>
		/// <para>If the function succeeds, the function returns ERROR_SUCCESS.</para>
		/// <para>If the function fails, it returns a nonzero error code defined in WinError.h.</para>
		/// </returns>
		/// <remarks>
		/// <para>Minimum supported client: Windows XP [desktop apps only]</para>
		/// <para>Minimum supported server: Windows Server 2003 [desktop apps only]</para>
		/// </remarks>
		/// </summary>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetNamedSecurityInfoW")]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern uint SetNamedSecurityInfo(
			[MarshalAs(UnmanagedType.LPWStr)] string pObjectName,
			ObjectType objectType,
			SecurityInformation securityInfo,
			IntPtr pSidOwner,
			IntPtr pSidGroup,
			IntPtr pDacl,
			IntPtr pSacl);

		/// <summary>The GetSecurityDescriptorDacl function retrieves a pointer to the discretionary access control list (DACL) in a specified security descriptor.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero.
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetSecurityDescriptorDacl(
			SafeGlobalMemoryBufferHandle pSecurityDescriptor,
			[MarshalAs(UnmanagedType.Bool)] out bool lpbDaclPresent,
			out IntPtr pDacl,
			[MarshalAs(UnmanagedType.Bool)] out bool lpbDaclDefaulted);

		/// <summary>The GetSecurityDescriptorSacl function retrieves a pointer to the system access control list (SACL) in a specified security descriptor.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero.
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetSecurityDescriptorSacl(
			SafeGlobalMemoryBufferHandle pSecurityDescriptor,
			[MarshalAs(UnmanagedType.Bool)] out bool lpbSaclPresent,
			out IntPtr pSacl,
			[MarshalAs(UnmanagedType.Bool)] out bool lpbSaclDefaulted);

		/// <summary>The GetSecurityDescriptorGroup function retrieves the primary group information from a security descriptor.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero.
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetSecurityDescriptorGroup(
			SafeGlobalMemoryBufferHandle pSecurityDescriptor,
			out IntPtr pGroup,
			[MarshalAs(UnmanagedType.Bool)] out bool lpbGroupDefaulted);

		/// <summary>The GetSecurityDescriptorControl function retrieves a security descriptor control and revision information.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero.
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetSecurityDescriptorControl(
			SafeGlobalMemoryBufferHandle pSecurityDescriptor,
			out SecurityDescriptorControl pControl,
			out uint lpdwRevision);

		/// <summary>The GetSecurityDescriptorOwner function retrieves the owner information from a security descriptor.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero.
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetSecurityDescriptorOwner(
			SafeGlobalMemoryBufferHandle pSecurityDescriptor,
			out IntPtr pOwner,
			[MarshalAs(UnmanagedType.Bool)] out bool lpbOwnerDefaulted);

		/// <summary>The GetSecurityDescriptorLength function returns the length, in bytes, of a structurally valid security descriptor. The length includes the length of all associated structures.</summary>
		/// <returns>
		/// If the function succeeds, the function returns the length, in bytes, of the SECURITY_DESCRIPTOR structure.
		/// If the SECURITY_DESCRIPTOR structure is not valid, the return value is undefined.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern uint GetSecurityDescriptorLength(SafeGlobalMemoryBufferHandle pSecurityDescriptor);

		/// <summary>The InitializeSecurityDescriptor function initializes a new security descriptor.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero. 
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern long InitializeSecurityDescriptor(ref SECURITY_DESCRIPTOR pSecurityDescriptor, long dwRevision);

		/// <summary>The SetSecurityDescriptorOwner function sets the owner information of an absolute-format security descriptor. It replaces any owner information already present in the security descriptor.</summary>
		/// <returns>
		/// If the function succeeds, the function returns nonzero. 
		/// If the function fails, it returns zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>Minimum supported client: Windows XP [desktop apps only]</remarks>
		/// <remarks>Minimum supported server: Windows Server 2003 [desktop apps only]</remarks>
		[DllImport(Advapi32, SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern long SetSecurityDescriptorOwner(ref SECURITY_DESCRIPTOR pSecurityDescriptor, byte[] pOwner, long bOwnerDefaulted);
	}
}
