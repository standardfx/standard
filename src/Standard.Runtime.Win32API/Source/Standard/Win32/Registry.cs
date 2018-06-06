using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Standard.Runtime.Win32API;

namespace Standard.Win32
{
    public static class Registry
    {
        public static IntPtr GetHandle(RegistryKey regKey)
        {
            Type type = regKey.GetType();
            FieldInfo fieldInfo = type.GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic);
            return (IntPtr)fieldInfo.GetValue(regKey); 
        }
    
        public static void LoadHive(string hivePath, string mountPoint)
        {
            hivePath = NormalizeHivePath(hivePath);
            mountPoint = NormalizeMountPoint(mountPoint);

            NativeMethods.REGISTRY_ROOT hkey = mountPoint.StartsWith("HKLM")
                ? NativeMethods.REGISTRY_ROOT.HKEY_LOCAL_MACHINE
                : NativeMethods.REGISTRY_ROOT.HKEY_USERS;

            string subkey = mountPoint.StartsWith("HKLM")
                ? mountPoint.Substring(5)
                : mountPoint.Substring(4);

            NativeMethods.RegLoadKey(hkey, subkey, hivePath);
        }

        public static void UnloadHive(string mountPoint)
        {
            mountPoint = NormalizeMountPoint(mountPoint);

            NativeMethods.REGISTRY_ROOT hkey = mountPoint.StartsWith("HKLM")
                ? NativeMethods.REGISTRY_ROOT.HKEY_LOCAL_MACHINE
                : NativeMethods.REGISTRY_ROOT.HKEY_USERS;

            string subkey = mountPoint.StartsWith("HKLM")
                ? mountPoint.Substring(5)
                : mountPoint.Substring(4);

            NativeMethods.RegUnLoadKey(hkey, subkey);
        }

        private static string NormalizeHivePath(string hivePath)
        {
            if (string.IsNullOrEmpty(hivePath))
                throw new ArgumentNullException("hivePath");

            hivePath = Path.GetFullPath(hivePath);
            if (!File.Exists(hivePath))
                throw new FileNotFoundException(RS.RegistryHiveFileNotFound, hivePath);
            
            return hivePath;
        }

        private static string NormalizeMountPoint(string mountPoint)
        {
            if (string.IsNullOrEmpty(mountPoint))
                throw new ArgumentNullException("mountPoint");

            mountPoint = mountPoint.Replace("/", "\\");
            string[] mountPointArr = mountPoint.Split('\\');
            if (mountPointArr.Length < 2)
                throw new ArgumentException(string.Format(RS.InvalidRegistryPath, mountPoint), "mountPoint");

            if (!(mountPointArr[0] == "HKLM" || mountPointArr[0] == "HKEY_LOCAL_MACHINE" ||
                mountPointArr[0] == "HKU" || mountPointArr[0] == "HKEY_USERS"))
            {
                throw new ArgumentException(RS.InvalidRegistryMountPoint, "mountPoint");
            }

            foreach (string keyName in mountPointArr)
            {
                if (string.IsNullOrEmpty(keyName))
                    throw new ArgumentException(string.Format(RS.InvalidRegistryPath, mountPoint), "mountPoint");
            }

            string hkey = (mountPointArr[0] == "HKLM" || mountPointArr[0] == "HKEY_LOCAL_MACHINE") 
                ? "HKLM"
                : "HKU";

            string subkey = mountPoint.Substring(mountPointArr[0].Length + 1).TrimEnd('\\');

            return (hkey + "\\" + subkey);
        }
    }
}
