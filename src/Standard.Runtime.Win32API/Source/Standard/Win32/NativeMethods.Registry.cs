using System;
using System.Runtime.InteropServices;

namespace Standard.Win32
{
    partial class NativeMethods
    {
		/// <summary>
		/// enum used by RegOpenKeyEx
		/// </summary>
		public enum SAM_DESIRED : long
		{
			KEY_QUERY_VALUE = 0x1,
			KEY_SET_VALUE = 0x2,
			KEY_ALL_ACCESS = 0xf003f,
			KEY_CREATE_SUB_KEY = 0x4,
			KEY_ENUMERATE_SUB_KEYS = 0x8,
			KEY_NOTIFY = 0x10,
			KEY_CREATE_LINK = 0x20,
			READ_CONTROL = 0x20000,
			WRITE_DAC = 0x40000,
			WRITE_OWNER = 0x80000,
			SYNCHRONIZE = 0x100000,

			STANDARD_RIGHTS_REQUIRED = 0xf0000,

			STANDARD_RIGHTS_READ = READ_CONTROL,
			STANDARD_RIGHTS_WRITE = READ_CONTROL,
			STANDARD_RIGHTS_EXECUTE = READ_CONTROL,

			KEY_READ = STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY,

			KEY_WRITE = STANDARD_RIGHTS_WRITE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY,

			KEY_EXECUTE = KEY_READ
		}

		/// <summary>
		/// constant enum for registry roots
		/// </summary>
		public enum REGISTRY_ROOT : long
		{
			HKEY_CLASSES_ROOT = 0x80000000,
			HKEY_CURRENT_USER = 0x80000001,
			HKEY_LOCAL_MACHINE = 0x80000002,
			HKEY_USERS = 0x80000003
		}

		[DllImport(Advapi32)]
        public static extern long RegSetKeySecurity(IntPtr ptrKey, SECURITY_INFORMATION SecurityInformation, SECURITY_DESCRIPTOR pSecurityDescriptor);

        [DllImport(Advapi32, EntryPoint = "RegOpenKeyExA")]
		public static extern long RegOpenKeyEx(REGISTRY_ROOT hKey, string lpSubKey, long ulOptions, SAM_DESIRED samDesired, ref IntPtr ptrKey);

        [DllImport(Advapi32)]
		public static extern long RegCloseKey(IntPtr ptrKey);

        [DllImport(Advapi32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RegLoadKey(REGISTRY_ROOT hKey, string lpSubKey, string lpFile);

        [DllImport(Advapi32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RegUnLoadKey(REGISTRY_ROOT hKey, string lpSubKey);
    }
}
