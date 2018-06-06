using System;
using System.Runtime.InteropServices;
using Standard.Security.AccessControl;
using Standard.Win32.SafeHandles;

namespace Standard.Win32
{
   partial class NativeMethods
   {
      /// <summary>
      ///   Creates a new transaction object.
      /// </summary>
      /// <remarks>
      ///   <para>Use the <see cref="CloseHandle"/> function to close the transaction handle. If the last transaction handle is closed
      ///   beforea client calls the CommitTransaction function with the transaction handle, then KTM rolls back the transaction.</para>
      ///   <para>Minimum supported client: Windows Vista</para>
      ///   <para>Minimum supported server:Windows Server 2008</para>
      /// </remarks>
      /// <returns>
      ///   <para>If the function succeeds, the return value is a handle to the transaction.</para>
      ///   <para>If the function fails, the return value is INVALID_HANDLE_VALUE. To get extended error information, call the GetLastError
      ///   function.</para>
      /// </returns>
      [DllImport(Ktmw32, SetLastError = true, CharSet = CharSet.Unicode)]
      public static extern SafeKernelTransactionHandle CreateTransaction(
            [MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpTransactionAttributes, 
            IntPtr uow, 
            [MarshalAs(UnmanagedType.U4)] uint createOptions, 
            [MarshalAs(UnmanagedType.U4)] uint isolationLevel, 
            [MarshalAs(UnmanagedType.U4)] uint isolationFlags, 
            [MarshalAs(UnmanagedType.U4)] int timeout, 
            [MarshalAs(UnmanagedType.LPWStr)] string description);

      /// <summary>Requests that the specified transaction be committed.</summary>
      /// <remarks>
      ///   <para>You can commit any transaction handle that has been opened or created using the TRANSACTION_COMMIT permission; any
      ///   application can commit a transaction, not just the creator.</para>
      ///   <para>This function can only be called if the transaction is still active, not prepared, pre-prepared, or rolled back.</para>
      ///   <para>Minimum supported client: Windows Vista</para>
      ///   <para>Minimum supported server:Windows Server 2008</para>
      /// </remarks>
      /// <returns>
      ///   <para>If the function succeeds, the return value is nonzero.</para>
      ///   <para>If the function fails, the return value is 0 (zero). To get extended error information, call the GetLastError function.</para>
      /// </returns>
      [DllImport(Ktmw32, SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CommitTransaction(SafeHandle hTrans);

      /// <summary>
      ///   Requests that the specified transaction be rolled back. This function is synchronous.      
      /// </summary>
      /// <returns>
      ///   <para>If the function succeeds, the return value is nonzero.</para>
      ///   <para>If the function fails, the return value is zero. To get extended error information, call the GetLastError function. </para>
      /// </returns>
      [DllImport(Ktmw32, SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool RollbackTransaction(SafeHandle hTrans);
   }
}
