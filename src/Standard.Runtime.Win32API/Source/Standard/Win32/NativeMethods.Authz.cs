using System;
using System.Runtime.InteropServices;
using Standard.Win32.SafeHandles;
using Standard.Security.AccessControl;

namespace Standard.Win32
{
    partial class NativeMethods
    {
		public const string AUTHZ_OBJECTUUID_WITHCAP = "9a81c2bd-a525-471d-a4ed-49907c0b23da";

		public enum AuthzRpcClientVersion : ushort
        {
            V1 = 1
        }

        [Flags]
		public enum AuthzResourceManagerFlags : uint
        {
            NO_AUDIT = 0x1,
        }

        [Flags]
		public enum AuthzInitFlags : uint
        {
            Default = 0x0,
            SkipTokenGroups = 0x2,
            RequireS4ULogon = 0x4,
            ComputePrivileges = 0x8,
        }

		public enum AuthzACFlags : uint // DWORD
        {
            None = 0,
            NoDeepCopySD
        }

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct AUTHZ_RPC_INIT_INFO_CLIENT
		{
			public AuthzRpcClientVersion version;
			public string objectUuid;
			public string protocol;
			public string server;
			public string endPoint;
			public string options;
			public string serverSpn;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct AUTHZ_ACCESS_REQUEST
		{
			public StdAccess DesiredAccess;
			public byte[] PrincipalSelfSid;
			public IntPtr ObjectTypeList;
			public int ObjectTypeListLength;
			public IntPtr OptionalArguments;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct AUTHZ_ACCESS_REPLY
		{
			public int ResultListLength;
			public IntPtr GrantedAccessMask;
			public IntPtr SaclEvaluationResults;
			public IntPtr Error;
		}

		[DllImport(Authz, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AuthzFreeResourceManager(IntPtr handle);

		[DllImport(Authz, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AuthzInitializeRemoteResourceManager(
			IntPtr rpcInitInfo,
			out SafeAuthzRMHandle authRM);

		[DllImport(Authz, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AuthzInitializeResourceManager(
			AuthzResourceManagerFlags flags,
			IntPtr pfnAccessCheck,
			IntPtr pfnComputeDynamicGroups,
			IntPtr pfnFreeDynamicGroups,
			string szResourceManagerName,
			out SafeAuthzRMHandle phAuthzResourceManager);

		[DllImport(Authz, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AuthzInitializeContextFromSid(
			AuthzInitFlags flags,
			byte[] rawUserSid,
			SafeAuthzRMHandle authzRM,
			IntPtr expirationTime,
			Luid Identifier,
			IntPtr DynamicGroupArgs,
			out IntPtr authzClientContext);

		[DllImport(Authz, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AuthzAccessCheck(
			AuthzACFlags flags,
			IntPtr hAuthzClientContext,
			ref AUTHZ_ACCESS_REQUEST pRequest,
			IntPtr AuditEvent,
			byte[] rawSecurityDescriptor,
			IntPtr[] OptionalSecurityDescriptorArray,
			UInt32 OptionalSecurityDescriptorCount,
			ref AUTHZ_ACCESS_REPLY pReply,
			IntPtr cachedResults);

		[DllImport(Authz, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AuthzFreeContext(IntPtr authzClientContext);
	}
}

