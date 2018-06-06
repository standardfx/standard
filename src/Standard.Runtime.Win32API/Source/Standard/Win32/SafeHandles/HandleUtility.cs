using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Standard.Runtime.Win32API;

namespace Standard.Win32.SafeHandles
{
   public static class HandleUtility
   {
      /// <summary>Check is the current handle is not null, not closed and not invalid.</summary>
      /// <param name="handle">The current handle to check.</param>
      /// <returns><see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
      /// <exception cref="ArgumentException"/>
      public static bool IsValid(SafeHandle handle)
      {
         if (handle == null || handle.IsClosed || handle.IsInvalid)
         {
            if (handle != null)
               handle.Close();

            return false;
         }

         return true;
      }

      /// <summary>Check is the current handle is not null, not closed and not invalid.</summary>
      /// <param name="handle">The current handle to check.</param>
      /// <returns><see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
      /// <exception cref="ArgumentException"/>
      public static void AssertValid(SafeHandle handle)
      {
         if (!IsValid(handle))
            throw new ArgumentException(RS.HandleInvalid);
      }

      /// <summary>Check is the current handle is not null, not closed and not invalid.</summary>
      /// <param name="handle">The current handle to check.</param>
      /// <param name="lastError">The error code associated with this exception.</param>
      /// <returns><see langword="true"/> on success, <see langword="false"/> otherwise.</returns>
      /// <exception cref="ArgumentException"/>
      public static void AssertValid(SafeHandle handle, int lastError)
      {
         if (!IsValid(handle))
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, RS.HandleInvalidWin32Error, lastError));
      }
   }
}
