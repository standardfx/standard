namespace Standard.Win32
{
   partial class NativeMethods
   {
      // http://msdn.microsoft.com/en-us/library/windows/desktop/aa370674%28v=vs.85%29.aspx

      /// <summary>(0) The operation completed successfully.</summary>
      public const uint NERR_Success = 0;

      /// <summary>(2250) The network connection could not be found.</summary>
      public const uint NERR_UseNotFound = 2250;

      /// <summary>(2310) This shared resource does not exist.</summary>
      public const uint NERR_NetNameNotFound = 2310;

      /// <summary>(2314) There is not an open file with that identification number.</summary>
      public const uint NERR_FileIdNotFound = 2314;
   }
}
