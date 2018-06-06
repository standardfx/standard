namespace Standard.Win32
{
   partial class NativeMethods
   {
      // Configuration Manager Error Codes

      /// <summary>(0) The operation completed successfully.</summary>
      public const uint CR_SUCCESS = 0;

      public const uint CR_DEFAULT = 1;
      public const uint CR_OUT_OF_MEMORY = 2;
      public const uint CR_INVALID_POINTER = 3;
      public const uint CR_INVALID_FLAG = 4;
      public const uint CR_INVALID_DEVNODE = 5;
      public const uint CR_INVALID_DEVINST = CR_INVALID_DEVNODE;
      public const uint CR_INVALID_RES_DES = 6;
      public const uint CR_INVALID_LOG_CONF = 7;
      public const uint CR_INVALID_ARBITRATOR = 8;
      public const uint CR_INVALID_NODELIST = 9;
      public const uint CR_DEVNODE_HAS_REQS = 10;
      public const uint CR_DEVINST_HAS_REQS = CR_DEVNODE_HAS_REQS;
      public const uint CR_INVALID_RESOURCEID = 11;
      public const uint CR_DLVXD_NOT_FOUND = 12; // WIN 95 ONLY 
      public const uint CR_NO_SUCH_DEVNODE = 13;
      public const uint CR_NO_SUCH_DEVINST = CR_NO_SUCH_DEVNODE;
      public const uint CR_NO_MORE_LOG_CONF = 14;
      public const uint CR_NO_MORE_RES_DES = 15;
      public const uint CR_ALREADY_SUCH_DEVNODE = 16;
      public const uint CR_ALREADY_SUCH_DEVINST = CR_ALREADY_SUCH_DEVNODE;
      public const uint CR_INVALID_RANGE_LIST = 17;
      public const uint CR_INVALID_RANGE = 18;
      public const uint CR_FAILURE = 19;
      public const uint CR_NO_SUCH_LOGICAL_DEV = 20;
      public const uint CR_CREATE_BLOCKED = 21;
      public const uint CR_NOT_SYSTEM_VM = 22; // WIN 95 ONLY 
      public const uint CR_REMOVE_VETOED = 23;
      public const uint CR_APM_VETOED = 24;
      public const uint CR_INVALID_LOAD_TYPE = 25;
      public const uint CR_BUFFER_SMALL = 26;
      public const uint CR_NO_ARBITRATOR = 27;
      public const uint CR_NO_REGISTRY_HANDLE = 28;
      public const uint CR_REGISTRY_ERROR = 29;
      public const uint CR_INVALID_DEVICE_ID = 30;
      public const uint CR_INVALID_DATA = 31;
      public const uint CR_INVALID_API = 32;
      public const uint CR_DEVLOADER_NOT_READY = 33;
      public const uint CR_NEED_RESTART = 34;
      public const uint CR_NO_MORE_HW_PROFILES = 35;
      public const uint CR_DEVICE_NOT_THERE = 36;
      public const uint CR_NO_SUCH_VALUE = 37;
      public const uint CR_WRONG_TYPE = 38;
      public const uint CR_INVALID_PRIORITY = 39;
      public const uint CR_NOT_DISABLEABLE = 40;
      public const uint CR_FREE_RESOURCES = 41;
      public const uint CR_QUERY_VETOED = 42;
      public const uint CR_CANT_SHARE_IRQ = 43;
      public const uint CR_NO_DEPENDENT = 44;
      public const uint CR_SAME_RESOURCES = 45;
      public const uint CR_NO_SUCH_REGISTRY_KEY = 46;
      public const uint CR_INVALID_MACHINENAME = 47; // NT ONLY 
      public const uint CR_REMOTE_COMM_FAILURE = 48; // NT ONLY 
      public const uint CR_MACHINE_UNAVAILABLE = 49; // NT ONLY 
      public const uint CR_NO_CM_SERVICES = 50; // NT ONLY 
      public const uint CR_ACCESS_DENIED = 51; // NT ONLY 
      public const uint CR_CALL_NOT_IMPLEMENTED = 52;
      public const uint CR_INVALID_PROPERTY = 53;
      public const uint CR_DEVICE_INTERFACE_ACTIVE = 54;
      public const uint CR_NO_SUCH_DEVICE_INTERFACE = 55;
      public const uint CR_INVALID_REFERENCE_STRING = 56;
      public const uint NUM_CR_RESULTS = 57;
   }
}
