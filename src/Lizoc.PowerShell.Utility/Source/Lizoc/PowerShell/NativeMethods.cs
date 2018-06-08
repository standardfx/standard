using System;
using System.Runtime.InteropServices;
using System.Text;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell
{
    internal static class NativeMethods
    {
        private const string User32 = "user32.dll";
        private const string Shlwapi = "shlwapi.dll";
        private const string Shell32 = "shell32.dll";
        private const string Wininet = "wininet.dll";

        [Flags]
        private enum InternetConnectionState_E : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        [DllImport(Wininet, SetLastError = true)]
        private extern static bool InternetGetConnectedState(ref InternetConnectionState_E lpdwFlags, int dwReserved);

        public static bool IsConnectedToInternet()
        {
            InternetConnectionState_E flags = 0;
            bool isConnected = InternetGetConnectedState(ref flags, 0);
            return isConnected;
        }

        [DllImport(User32, SetLastError = true)]
        public static extern bool LockWorkStation();

        [DllImport(Shlwapi, CharSet = CharSet.Unicode)]
        private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, string ppvReserved);

        public static string GetIndirectString(string indirectString)
        {
            try 
            {
                int retval;

                // build the output buffer reference
                StringBuilder lptStr = new StringBuilder(1024);

                // indirectString contains the MUI formatted string
                retval = SHLoadIndirectString(indirectString, lptStr, 1024, null);
                if (retval != 0)
                    throw new InvalidOperationException(string.Format(RS.ApiErrorDetail, "SHLoadIndirectString", retval));
                else
                    return lptStr.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(RS.ApiError, ex.Message));
            }
        }

        [DllImport(Shell32)]
        private static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] 
            Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr pszPath
        );

        public static string GetKnownFolderPath(Guid rfid)
        {
            IntPtr pszPath;

            // don't throw error
            // lots of guids are os ver dependent.
            if (SHGetKnownFolderPath(rfid, 0, IntPtr.Zero, out pszPath) != 0)
                return string.Empty;

            string path = Marshal.PtrToStringUni(pszPath);
            Marshal.FreeCoTaskMem(pszPath);
            return path;
        }        
    }
}
