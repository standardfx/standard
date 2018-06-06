using System;
using System.Security;
using System.Runtime.InteropServices;
using Standard.Core;

namespace Standard
{
    partial class FastConvert
    {
        private static readonly uint[] _lookup32Unsafe = CreateLookup32Unsafe();
        private static unsafe readonly uint* _lookup32UnsafeP = (uint*)GCHandle.Alloc(_lookup32Unsafe, GCHandleType.Pinned).AddrOfPinnedObject();

        private static uint[] CreateLookup32Unsafe()
        {
            uint[] result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                if (BitConverter.IsLittleEndian)
                    result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
                else
                    result[i] = ((uint)s[1]) + ((uint)s[0] << 16);
            }
            return result;
        }

        /// <summary>
        /// Converts an array of bytes to hexadecimal notation.
        /// </summary>
        /// <param name="bytes">The data to be converted.</param>
        [SecuritySafeCritical]
        public static unsafe string ToBase16String(byte[] bytes)
        {
            var lookupP = _lookup32UnsafeP;
            var result = new string((char)0, bytes.Length * 2);
            fixed (byte* bytesP = bytes)
            fixed (char* resultP = result)
            {
                uint* resultP2 = (uint*)resultP;
                for (int i = 0; i < bytes.Length; i++)
                {
                    resultP2[i] = lookupP[bytesP[i]];
                }
            }

            return result;
        }

        /// <summary>
        /// Converts an array of bytes to hexadecimal notation.
        /// </summary>
        /// <param name="bytes">The data to be converted.</param>
        /// <param name="lowerCase">If <c>true</c>, use lower case letters ('a' to 'f'), otherwise upper case ('A' to 'F').</param>
        public static string ToBase16String(byte[] bytes, bool lowerCase)
        {
            if (lowerCase)
                return ToBase16String(bytes).ToLowerInvariant();
            else
                return ToBase16String(bytes);
        }

        /// <summary>
        /// Converts hexadecimal notation string to an array of bytes.
        /// </summary>
        /// <param name="value">The data to be converted.</param>
        public static byte[] FromBase16String(string value)
        {
            return FromBase16String(value.ToUpperInvariant(), false);
        }

        /// <summary>
        /// Converts hexadecimal notation string to an array of bytes.
        /// </summary>
        /// <param name="value">The data to be converted.</param>
        /// <param name="lowerCase">If <c>true</c>, the decoder will assume that the string value uses lower case letters ('a' to 'f'), otherwise upper case ('A' to 'F').</param>
        public static byte[] FromBase16String(string value, bool lowerCase)
        {
            if (value.Length % 2 == 1)
                throw new FormatException(RS.BadBase16Length);

            byte[] buffer = new byte[value.Length >> 1];

            for (int i = 0; i < (value.Length >> 1); ++i)
            {
                if (lowerCase)
                    buffer[i] = (byte)((GetHexValueLowerCase(value[i << 1]) << 4) + (GetHexValueLowerCase(value[(i << 1) + 1])));
                else
                    buffer[i] = (byte)((GetHexValueUpperCase(value[i << 1]) << 4) + (GetHexValueUpperCase(value[(i << 1) + 1])));
            }

            return buffer;
        }

        private static int GetHexValueUpperCase(char hex) 
        {
            int val = (int)hex;

            // For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
        }

        private static int GetHexValueLowerCase(char hex) 
        {
            int val = (int)hex;

            // For lowercase a-f letters:
            return val - (val < 58 ? 48 : 87);
        }
    }
}
