using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Standard.Security.AccessControl;
using FileMode = System.IO.FileMode;
using FileShare = System.IO.FileShare;

namespace Standard.Win32
{
   partial class NativeMethods
   {
       /// <summary>Defines the access rights to use when creating access and audit rules.</summary>
       [Flags]
       public enum FileSystemDesiredAccess : uint
       {
           /// <summary>Reserved. Do not use.</summary>
           None = 0,

           /// <summary>Specifies the right to open and copy a file or folder. This does not include the right to read file system attributes, extended file system attributes, or access and audit rules.</summary>
           ReadData = 1,

           /// <summary>Specifies the right to read the contents of a directory.</summary>
           /// <remarks>This has the same underlying value as <c>ReadData</c>, but is used for directories.</remarks>
           ListDirectory = ReadData,

           /// <summary>Specifies the right to open and write to a file or folder. This does not include the right to open and write file system attributes, extended file system attributes, or access and audit rules.</summary>
           WriteData = 2,

           /// <summary>Specifies the right to create a file.</summary>
           /// <remarks>This has the same underlying value as <c>WriteData</c>, but is used for directories.</remarks>
           CreateFiles = WriteData,

           /// <summary>Specifies the right to append data to the end of a file.</summary>
           AppendData = 4,

           /// <summary>Specifies the right to create a folder.</summary>
           /// <remarks>This has the same underlying value as <c>AppendData</c>, but is used for directories.</remarks>
           CreateDirectories = AppendData,

           /// <summary>Specifies the right to open and copy extended file system attributes from a folder or file. For example, this value specifies the right to view author and content information. This does not include the right to read data, file system attributes, or access and audit rules.</summary>
           ReadExtendedAttributes = 8,

           /// <summary>Specifies the right to open and write extended file system attributes to a folder or file. This does not include the ability to write data, attributes, or access and audit rules.</summary>
           WriteExtendedAttributes = 16,

           /// <summary>Specifies the right to run an application file.</summary>
           ExecuteFile = 32,

           /// <summary>Specifies the right to list the contents of a folder and to run applications contained within that folder.</summary>
           /// <remarks>This has the same underlying value as <c>ExecuteFile</c>, but is used for directories.</remarks>
           Traverse = ExecuteFile,

           /// <summary>Specifies the right to delete a folder and any files contained within that folder.</summary>
           /// <devdoc>
           /// DeleteSubdirectoriesAndFiles only makes sense on directories, but the shell explicitly sets it for files in its UI. 
           /// So we'll include it in FullControl
           /// </devdoc>
           DeleteSubdirectoriesAndFiles = 64,

           /// <summary>Specifies the right to open and copy file system attributes from a folder or file. For example, this value specifies the right to view the file creation or modified date. This does not include the right to read data, extended file system attributes, or access and audit rules.</summary>
           ReadAttributes = 128,

           /// <summary>Specifies the right to open and copy a file or folder. This does not include the right to read file system attributes, extended file system attributes, or access and audit rules.</summary>
           WriteAttributes = 256,

           /// <summary>Specifies the right to create folders and files, and to add or remove data from files. This right includes the WriteData right, AppendData right, WriteExtendedAttributes right, and WriteAttributes right.</summary>
           Write =  WriteData | AppendData | WriteExtendedAttributes | WriteAttributes,

           /// <summary>Specifies the right to delete a folder or file.</summary>
           Delete = 65536,

           /// <summary>Specifies the right to open and copy access and audit rules from a folder or file. This does not include the right to read data, file system attributes, and extended file system attributes.</summary>
           ReadPermissions = 131072,

           /// <summary>Specifies the right to open and copy folders or files as read-only. This right includes the ReadData right, ReadExtendedAttributes right, ReadAttributes right, and ReadPermissions right.</summary>
           Read = ReadData | ReadExtendedAttributes | ReadAttributes | ReadPermissions,

           /// <summary>Specifies the right to open and copy folders or files as read-only, and to run application files. This right includes the Read right and the ExecuteFile right.</summary>
           ReadAndExecute = Read | ExecuteFile,

           /// <summary>Specifies the right to read, write, list folder contents, delete folders and files, and run application files. This right includes the ReadAndExecute right, the Write right, and the Delete right.</summary>
           Modify = ReadAndExecute | Write | Delete,

           /// <summary>Specifies the right to change the security and audit rules associated with a file or folder.</summary>
           ChangePermissions = 262144,

           /// <summary>Specifies the right to change the owner of a folder or file. Note that owners of a resource have full access to that resource.</summary>
           TakeOwnership = 524288,

           /// <summary>Specifies whether the application can wait for a file handle to synchronize with the completion of an I/O operation.</summary>
           /// <devdoc>From the Core File Services team, CreateFile always requires SYNCHRONIZE access.</devdoc>
           Synchronize = 1048576,

           /// <summary>Specifies the right to exert full control over a folder or file, and to modify access control and audit rules. This value represents the right to do anything with a file and is the combination of all rights in this enumeration.</summary>
           FullControl = 2032127,

           /// <summary>Read access (generic).</summary>
           /// <devdoc>#Todo</devdoc>
           GenericRead = 0x80000000,

           /// <summary>Write access (generic).</summary>
           GenericWrite = 0x40000000,

           /// <summary>Generic write access (generic).</summary>
           GenericExecute = 0x20000000,

           /// <summary>All possible access rights (generic).</summary>
           GenericAll = 0x10000000
       }

        /// <summary>
        ///   Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical
        ///   disk, volume, console buffer, tape drive, communications resource, mailslot, and pipe.
        /// </summary>
        /// <remarks>Minimum supported client: Windows XP.</remarks>
        /// <remarks>Minimum supported server: Windows Server 2003.</remarks>
        /// <returns>
        ///   If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot. If the
        ///   function fails, the return value is Native.ERROR_INVALID_HANDLE. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW")]
        public static extern SafeFileHandle CreateFile(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            [MarshalAs(UnmanagedType.U4)] FileSystemDesiredAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileSystemDesiredAccess dwFlagsAndAttributes, 
            IntPtr hTemplateFile);

        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW")]
        public static extern SafeFileHandle CreateFile(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            uint dwDesiredAccess,
            int dwShareMode,
            [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes, 
            IntPtr hTemplateFile);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool ReadFile(
            SafeFileHandle handle, 
            [Out] byte[] lpBuffer, 
            uint nNumberOfBytesToRead, 
            out uint lpNumberOfBytesRead, 
            [In] ref NativeOverlapped lpOverlapped); 

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool WriteFile(
            SafeFileHandle handle, 
            byte[] lpBuffer, 
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten, 
            [In] ref NativeOverlapped lpOverlapped); 

        /// <summary>
        ///   Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file, file stream, directory, physical
        ///   disk, volume, console buffer, tape drive, communications resource, mailslot, and pipe.
        /// </summary>
        /// <remarks>Minimum supported client: Windows Vista [desktop apps only].</remarks>
        /// <remarks>Minimum supported server: Windows Server 2008 [desktop apps only].</remarks>
        /// <returns>
        ///   If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot. If the
        ///   function fails, the return value is Native.ERROR_INVALID_HANDLE". To get extended error information, call GetLastError.
        /// </returns>
        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateFileTransactedW")]
        public static extern SafeFileHandle CreateFileTransacted(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            [MarshalAs(UnmanagedType.U4)] FileSystemDesiredAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileSystemDesiredAccess dwFlagsAndAttributes, 
            IntPtr hTemplateFile,
            SafeHandle hTransaction, 
            IntPtr pusMiniVersion, 
            IntPtr pExtendedParameter);

        /// <summary>Creates or opens a named or unnamed file mapping object for a specified file.</summary>
        /// <remarks>Minimum supported client: Windows XP.</remarks>
        /// <remarks>Minimum supported server: Windows Server 2003.</remarks>
        /// <returns>
        ///   If the function succeeds, the return value is a handle to the newly created file mapping object. If the function fails, the return
        ///   value is <see langword="null"/>.
        /// </returns>
        [DllImport(Kernel32, SetLastError = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileMappingW")]
        public static extern SafeFileHandle CreateFileMapping(
            SafeFileHandle hFile, 
            SafeHandle lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] uint flProtect, 
            [MarshalAs(UnmanagedType.U4)] uint dwMaximumSizeHigh,
            [MarshalAs(UnmanagedType.U4)] uint dwMaximumSizeLow, 
            [MarshalAs(UnmanagedType.LPWStr)] string lpName);
    }
}
