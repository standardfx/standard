using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Standard.Win32.SafeHandles;

namespace Standard.Win32
{
   partial class NativeMethods
   {
      /// <summary>The CM_Connect_Machine function creates a connection to a remote machine.</summary>
      /// <remarks>
      ///   <para>Beginning in Windows 8 and Windows Server 2012 functionality to access remote machines has been removed.</para>
      ///   <para>You cannot access remote machines when running on these versions of Windows.</para>
      ///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
      /// </remarks>
      /// <param name="uncServerName">Name of the unc server.</param>
      /// <param name="phMachine">[out] The ph machine.</param>
      /// <returns>
      ///   <para>If the operation succeeds, the function returns CR_SUCCESS.</para>
      ///   <para>Otherwise, it returns one of the CR_-prefixed error codes defined in Cfgmgr32.h.</para>
      /// </returns>
      [DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.I4)]
      public static extern int CM_Connect_Machine(
            [MarshalAs(UnmanagedType.LPWStr)] string uncServerName, 
            out SafeCmConnectMachineHandle phMachine);

      /// <summary>
      ///   The CM_Get_Device_ID_Ex function retrieves the device instance ID for a specified device instance on a local or a remote machine.
      /// </summary>
      /// <remarks>
      ///   <para>Beginning in Windows 8 and Windows Server 2012 functionality to access remote machines has been removed.</para>
      ///   <para>You cannot access remote machines when running on these versions of Windows.</para>
      ///   <para>&#160;</para>
      ///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
      /// </remarks>
      /// <param name="dnDevInst">The dn development instance.</param>
      /// <param name="buffer">The buffer.</param>
      /// <param name="bufferLen">Length of the buffer.</param>
      /// <param name="ulFlags">The ul flags.</param>
      /// <param name="hMachine">The machine.</param>
      /// <returns>
      ///   <para>If the operation succeeds, the function returns CR_SUCCESS.</para>
      ///   <para>Otherwise, it returns one of the CR_-prefixed error codes defined in Cfgmgr32.h.</para>
      /// </returns>
      [DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.I4)]
      public static extern int CM_Get_Device_ID_Ex(
            [MarshalAs(UnmanagedType.U4)] uint dnDevInst, 
            SafeGlobalMemoryBufferHandle buffer, 
            [MarshalAs(UnmanagedType.U4)] uint bufferLen, 
            [MarshalAs(UnmanagedType.U4)] uint ulFlags, 
			SafeCmConnectMachineHandle hMachine);

      /// <summary>
      ///   The CM_Disconnect_Machine function removes a connection to a remote machine.
      /// </summary>
      /// <remarks>
      ///   <para>Beginning in Windows 8 and Windows Server 2012 functionality to access remote machines has been removed.</para>
      ///   <para>You cannot access remote machines when running on these versions of Windows.</para>
      ///   <para>SetLastError is set to <see langword="false"/>.</para>
      ///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
      /// </remarks>
      /// <param name="hMachine">The machine.</param>
      /// <returns>
      ///   <para>If the operation succeeds, the function returns CR_SUCCESS.</para>
      ///   <para>Otherwise, it returns one of the CR_-prefixed error codes defined in Cfgmgr32.h.</para>
      /// </returns>
      [DllImport(SetupApi, SetLastError = false, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.I4)]
	  public static extern int CM_Disconnect_Machine(IntPtr hMachine);

      /// <summary>
      ///   The CM_Get_Parent_Ex function obtains a device instance handle to the parent node of a specified device node (devnode) in a local
      ///   or a remote machine's device tree.
      /// </summary>
      /// <remarks>
      ///   <para>Beginning in Windows 8 and Windows Server 2012 functionality to access remote machines has been removed.</para>
      ///   <para>You cannot access remote machines when running on these versions of Windows.</para>
      ///   <para>Available in Microsoft Windows 2000 and later versions of Windows.</para>
      /// </remarks>
      /// <param name="pdnDevInst">[out] The pdn development instance.</param>
      /// <param name="dnDevInst">The dn development instance.</param>
      /// <param name="ulFlags">The ul flags.</param>
      /// <param name="hMachine">The machine.</param>
      /// <returns>
      ///   <para>If the operation succeeds, the function returns CR_SUCCESS.</para>
      ///   <para>Otherwise, it returns one of the CR_-prefixed error codes defined in Cfgmgr32.h.</para>
      /// </returns>
      [DllImport(SetupApi, SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.I4)]
      public static extern int CM_Get_Parent_Ex(
            [MarshalAs(UnmanagedType.U4)] out uint pdnDevInst, 
            [MarshalAs(UnmanagedType.U4)] uint dnDevInst, 
            [MarshalAs(UnmanagedType.U4)] uint ulFlags, 
            SafeCmConnectMachineHandle hMachine);
   }
}
