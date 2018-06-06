using System;
using System.Security;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Standard
{
    // ref.
    // https://stackoverflow.com/questions/4502676/c-sharp-compare-two-securestrings-for-equality

    public static class SecureStringExtension
    {
        [SecuritySafeCritical]
        public static unsafe string GetValue(this SecureString s1)
        {
            if (s1 == null)  
                throw new ArgumentNullException(nameof(s1));

            if (s1.Length == 0)
                return string.Empty;

            IntPtr bstr = IntPtr.Zero;  

#if NETFX
            RuntimeHelpers.PrepareConstrainedRegions();  
#endif

            try
            {
#if NETSTANDARD
                bstr = SecureStringMarshal.SecureStringToGlobalAllocUnicode(s1);
                return Marshal.PtrToStringUni(bstr);
#else
                bstr = Marshal.SecureStringToBSTR(s1);  
                return Marshal.PtrToStringAuto(bstr);
#endif

            }
            finally
            {
#if NETSTANDARD
                if (bstr != IntPtr.Zero)  
                    Marshal.ZeroFreeGlobalAllocUnicode(bstr);  
#else
                if (bstr != IntPtr.Zero)  
                    Marshal.ZeroFreeBSTR(bstr);  
#endif
            }
        }

        //<#
        // .SYNOPSIS
        //      Checks that the underlying string value of a @[SecureString] instance is equal to another @[SecureString] instance.
        //
        // .PARAMETER s1
        //      The first @[SecureString] instance.
        //
        // .PARAMETER s2
        //      The second @[SecureString] instance.
        //
        // .OUTPUT
        //      If the underlying string value of @[s1] is equal to @[s2], `true`. Otherwise, `false`.
        //
        // .REMARKS
        //      The .NET Standard implementation uses @[SecureStringMarshal.SecureStringToGlobalAllocUnicode(SecureString)] internally, while 
        //      implementation on .NET Framework uses @[Marshal.SecureStringToBSTR(SecureString)].
        //#>
        [SecuritySafeCritical]
        public static unsafe bool ValueEquals(this SecureString s1, SecureString s2)
        {
            if (s1 == null)  
                throw new ArgumentNullException(nameof(s1));

            if (s2 == null)
                throw new ArgumentNullException(nameof(s2));

            if (s1.Length != s2.Length)
                return false;


            IntPtr bstr1 = IntPtr.Zero;  
            IntPtr bstr2 = IntPtr.Zero;  

#if NETFX
            RuntimeHelpers.PrepareConstrainedRegions();  
#endif

            try 
            {  
#if NETSTANDARD
                bstr1 = SecureStringMarshal.SecureStringToGlobalAllocUnicode(s1);
                bstr2 = SecureStringMarshal.SecureStringToGlobalAllocUnicode(s2);
#else
                bstr1 = Marshal.SecureStringToBSTR(s1);  
                bstr2 = Marshal.SecureStringToBSTR(s2);  
#endif
                unsafe 
                {  
                    for (char* ptr1 = (char*)bstr1.ToPointer(), ptr2 = (char*)bstr2.ToPointer();  *ptr1 != 0 && *ptr2 != 0;  ++ptr1, ++ptr2)  
                    {  
                        if (*ptr1 != *ptr2)  
                        {  
                            return false;  
                        }  
                    }  
                }  

                return true;  
            }  
            finally 
            {  
#if NETSTANDARD
                if (bstr1 != IntPtr.Zero)  
                    Marshal.ZeroFreeGlobalAllocUnicode(bstr1);  

                if (bstr2 != IntPtr.Zero)  
                    Marshal.ZeroFreeGlobalAllocUnicode(bstr2);  
#else
                if (bstr1 != IntPtr.Zero)  
                    Marshal.ZeroFreeBSTR(bstr1);  

                if (bstr2 != IntPtr.Zero)  
                    Marshal.ZeroFreeBSTR(bstr2);  
#endif
            }  
        }
    }
}