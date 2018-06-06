using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Standard.Win32.SafeHandles
{
   /// <summary>
   /// Represents a wrapper class for a handle used by the CM_Connect_Machine/CM_Disconnect_Machine Win32 API functions.
   /// </summary>
   public sealed class SafeCmConnectMachineHandle : SafeHandleZeroOrMinusOneIsInvalid
   {
      /// <summary>
	  /// Initializes a new instance of the <see cref="SafeCmConnectMachineHandle"/> class.
	  /// </summary>
      public SafeCmConnectMachineHandle() 
		 : base(true)
      { }

      protected override bool ReleaseHandle()
      {
         return NativeMethods.CM_Disconnect_Machine(handle) == NativeMethods.NO_ERROR;
      }
   }
}
