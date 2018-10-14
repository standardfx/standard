using System;
using System.Globalization;
using System.Security;

namespace Standard
{
    partial class FastConvert
    {
        // --- FromString ---

        /// <summary>
        /// Converts a string representation of byte value to <see cref="Byte"/>.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>
        /// A <see cref="Byte"/> object.
        /// </returns>
        [SecuritySafeCritical]
        public static unsafe byte ToByte(string str)
        {
            unchecked
            {
                return (byte)ToInt32(str);
            }
        }

        /// <summary>
        /// Converts a string representation of 16-bit numeric value to <see cref="Int16"/>.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>
        /// A <see cref="Int16"/> object.
        /// </returns>
        [SecuritySafeCritical]
        public static unsafe short ToInt16(string str)
        {
            unchecked
            {
                return (short)ToInt32(str);
            }
        }

        /// <summary>
        /// Converts a string representation of 16-bit unsigned numeric value to <see cref="UInt16"/>.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>
        /// A <see cref="UInt16"/> object.
        /// </returns>
        [SecuritySafeCritical]
        [CLSCompliant(false)]
        public static unsafe ushort ToUInt16(string str)
        {
            unchecked
            {
                return (ushort)ToInt32(str);
            }
        }

        /// <summary>
        /// Converts a string representation of 32-bit numeric value to <see cref="Int32"/>.
        /// </summary>
        /// <param name="strNum">The string to convert.</param>
        /// <returns>
        /// An <see cref="Int32"/> object.
        /// </returns>
        [SecuritySafeCritical]
        public static unsafe int ToInt32(string strNum)
        {
            int val = 0;
            int neg = 1;
            fixed (char* ptr = strNum)
            {
                char* str = ptr;
                if (*str == '-')
                {
                    neg = -1;
                    ++str;
                }

                while (*str != '\0')
                {
                    val = val * 10 + (*str++ - '0');
                }
            }
            return val * neg;
        }

        /// <summary>
        /// Converts a string representation of 32-bit unsigned numeric value to <see cref="UInt32"/>.
        /// </summary>
        /// <param name="strNum">The string to convert.</param>
        /// <returns>
        /// A <see cref="UInt32"/> object.
        /// </returns>
        [CLSCompliant(false)]
        [SecuritySafeCritical]
        public static unsafe uint ToUInt32(string strNum)
        {
            uint val = 0;
            fixed (char* ptr = strNum)
            {
                char* str = ptr;
                if (*str == '-')
                {
                    val = (uint)-val;
                    ++str;
                }

                while (*str != '\0')
                {
                    val = val * 10 + (uint)(*str++ - '0');
                }
            }
            return val;
        }

        /// <summary>
        /// Converts a string representation of 64-bit numeric value to <see cref="Int64"/>.
        /// </summary>
        /// <param name="strNum">The string to convert.</param>
        /// <returns>
        /// An <see cref="Int64"/> object.
        /// </returns>
        [SecuritySafeCritical]
        public static unsafe long ToInt64(string strNum)
        {
            long val = 0;
            long neg = 1;
            fixed (char* ptr = strNum)
            {
                char* str = ptr;
                if (*str == '-')
                {
                    neg = -1;
                    ++str;
                }

                while (*str != '\0')
                {
                    val = val * 10 + (*str++ - '0');
                }
            }
            return val * neg;
        }

        /// <summary>
        /// Converts a string representation of 64-bit unsigned numeric value to <see cref="UInt64"/>.
        /// </summary>
        /// <param name="strNum">The string to convert.</param>
        /// <returns>
        /// A <see cref="UInt64"/> object.
        /// </returns>
        [CLSCompliant(false)]
        [SecuritySafeCritical]
        public static unsafe ulong ToUInt64(string strNum)
        {
            ulong val = 0;
            fixed (char* ptr = strNum)
            {
                char* str = ptr;
                while (*str != '\0')
                {
                    val = val * 10 + (ulong)(*str++ - '0');
                }
            }
            return val;
        }

        /// <summary>
        /// Converts a string to double precision floating point number.
        /// </summary>
        /// <param name="numStr">The string to convert.</param>
        /// <returns>
        /// A <see cref="Double"/> object.
        /// </returns>
        [SecuritySafeCritical]
        public static unsafe double ToDouble(string numStr)
        {
            double val = 0.0;
            double neg = 1;

            fixed (char* ptr = numStr)
            {
                char* p = ptr;
                if (*p == '-')
                {
                    neg = -1;
                    ++p;
                }

                int count = 0;
                while (*p != '\0')
                {
                    if (*p == '.')
                    {
                        double rem = 0.0;
                        double div = 1;
                        ++p;
                        while (*p != '\0')
                        {
                            if (*p == 'E' || *p == 'e')
                            {
                                var e = 0;
                                val += rem * (Math.Pow(10, -1 * count));
                                ++p;
                                var ePlusMinus = 1;
                                if (*p == '-' || *p == '+')
                                {
                                    if (*p == '-')
                                        ePlusMinus = -1;
                                    ++p;
                                }

                                while (*p != '\0')
                                {
                                    e = e * 10 + (*p++ - '0');
                                }
                                val *= Math.Pow(10, e * ePlusMinus);
                                return val * neg;
                            }
                            rem = (rem * 10.0) + (*p - '0');
                            div *= 10.0;
                            ++p;
                            count++;
                        }
                        val += rem / div;
                        return ((double)(decimal)val) * neg;
                    }
                    val = (val * 10) + (*p - '0');
                    ++p;
                }
            }
            return ((double)(decimal)val) * neg;
        }

        /// <summary>
        /// Converts a string to single precision floating point number.
        /// </summary>
        /// <param name="numStr">The string to convert.</param>
        /// <returns>
        /// A <see cref="Single"/> object.
        /// </returns>
        public static unsafe float ToSingle(string numStr)
        {
            return (float)ToDouble(numStr);
        }

        /// <summary>
        /// Converts a string representation to a decimal number.
        /// </summary>
        /// <param name="numStr">The string to convert.</param>
        /// <returns>
        /// A <see cref="Decimal"/> object.
        /// </returns>
        public static decimal ToDecimal(string numStr)
        {
            return new decimal(ToDouble(numStr));
        }


        // --- ToString ---

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="snum">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="snum"/> value.
        /// </returns>
        [SecuritySafeCritical]
        public unsafe static string ToString(int snum)
        {
            char* s = stackalloc char[12];
            char* ps = s;
            int num1 = snum, num2, num3, div;
            if (snum < 0)
            {
                *ps++ = '-';
                //Can't negate int min
                if (snum == -2147483648)
                    return "-2147483648";
                num1 = -num1;
            }

            if (num1 < 10000)
            {
                if (num1 < 10) goto L1;
                if (num1 < 100) goto L2;
                if (num1 < 1000) goto L3;
            }
            else
            {
                num2 = num1 / 10000;
                num1 -= num2 * 10000;
                if (num2 < 10000)
                {
                    if (num2 < 10) goto L5;
                    if (num2 < 100) goto L6;
                    if (num2 < 1000) goto L7;
                }
                else
                {
                    num3 = num2 / 10000;
                    num2 -= num3 * 10000;
                    if (num3 >= 10)
                    {
                        *ps++ = (char)('0' + (char)(div = (num3 * 6554) >> 16));
                        num3 -= div * 10;
                    }
                    *ps++ = (char)('0' + (num3));
                }

                *ps++ = (char)('0' + (div = (num2 * 8389) >> 23));
                num2 -= div * 1000;
                
                L7:
                *ps++ = (char)('0' + (div = (num2 * 5243) >> 19));
                num2 -= div * 100;
            
                L6:
                *ps++ = (char)('0' + (div = (num2 * 6554) >> 16));
                num2 -= div * 10;
                
                L5:
                *ps++ = (char)('0' + (num2));
            }

            *ps++ = (char)('0' + (div = (num1 * 8389) >> 23));
            num1 -= div * 1000;
        
            L3:
            *ps++ = (char)('0' + (div = (num1 * 5243) >> 19));
            num1 -= div * 100;
        
            L2:
            *ps++ = (char)('0' + (div = (num1 * 6554) >> 16));
            num1 -= div * 10;
        
            L1:
            *ps++ = (char)('0' + (num1));

            return new string(s);
        }

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="snum">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="snum"/> value.
        /// </returns>
        [SecuritySafeCritical]
        [CLSCompliant(false)]
        public unsafe static string ToString(uint snum)
        {
            char* s = stackalloc char[11];
            char* ps = s;
            uint num1 = snum, num2, num3, div;

            if (snum == 0)
                return "0";
            else if (snum == 4294967295)
                return "4294967295";

            if (num1 < 10000)
            {
                if (num1 < 10) goto L1;
                if (num1 < 100) goto L2;
                if (num1 < 1000) goto L3;
            }
            else
            {
                num2 = num1 / 10000;
                num1 -= num2 * 10000;
                if (num2 < 10000)
                {
                    if (num2 < 10) goto L5;
                    if (num2 < 100) goto L6;
                    if (num2 < 1000) goto L7;
                }
                else
                {
                    num3 = num2 / 10000;
                    num2 -= num3 * 10000;
                    if (num3 >= 10)
                    {
                        *ps++ = (char)('0' + (char)(div = (num3 * 6554) >> 16));
                        num3 -= div * 10;
                    }
                    *ps++ = (char)('0' + (num3));
                }

                *ps++ = (char)('0' + (div = (num2 * 8389) >> 23));
                num2 -= div * 1000;
                
                L7:
                *ps++ = (char)('0' + (div = (num2 * 5243) >> 19));
                num2 -= div * 100;
            
                L6:
                *ps++ = (char)('0' + (div = (num2 * 6554) >> 16));
                num2 -= div * 10;
                
                L5:
                *ps++ = (char)('0' + (num2));
            }

            *ps++ = (char)('0' + (div = (num1 * 8389) >> 23));
            num1 -= div * 1000;
        
            L3:
            *ps++ = (char)('0' + (div = (num1 * 5243) >> 19));
            num1 -= div * 100;
        
            L2:
            *ps++ = (char)('0' + (div = (num1 * 6554) >> 16));
            num1 -= div * 10;
        
            L1:
            *ps++ = (char)('0' + (num1));

            return new string(s);
        }

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="snum">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="snum"/> value.
        /// </returns>
        [SecuritySafeCritical]
        public unsafe static string ToString(long snum)
        {
            char* s = stackalloc char[21];
            char* ps = s;
            long num1 = snum, num2, num3, num4, num5, div;

            if (snum < 0)
            {
                *ps++ = '-';
                if (snum == -9223372036854775808) 
                    return "-9223372036854775808";
                num1 = -snum;
            }

            if (num1 < 10000)
            {
                if (num1 < 10) goto L1;
                if (num1 < 100) goto L2;
                if (num1 < 1000) goto L3;
            }
            else
            {
                num2 = num1 / 10000;
                num1 -= num2 * 10000;
                if (num2 < 10000)
                {
                    if (num2 < 10) goto L5;
                    if (num2 < 100) goto L6;
                    if (num2 < 1000) goto L7;
                }
                else
                {
                    num3 = num2 / 10000;
                    num2 -= num3 * 10000;
                    if (num3 < 10000)
                    {
                        if (num3 < 10) goto L9;
                        if (num3 < 100) goto L10;
                        if (num3 < 1000) goto L11;
                    }
                    else
                    {
                        num4 = num3 / 10000;
                        num3 -= num4 * 10000;
                        if (num4 < 10000)
                        {
                            if (num4 < 10) goto L13;
                            if (num4 < 100) goto L14;
                            if (num4 < 1000) goto L15;
                        }
                        else
                        {
                            num5 = num4 / 10000;
                            num4 -= num5 * 10000;
                            if (num5 < 10000)
                            {
                                if (num5 < 10) goto L17;
                                if (num5 < 100) goto L18;
                            }

                            *ps++ = (char)('0' + (div = (num5 * 5243) >> 19));
                            num5 -= div * 100;

                            L18:
                            *ps++ = (char)('0' + (div = (num5 * 6554) >> 16));
                            num5 -= div * 10;
                        
                            L17:
                            *ps++ = (char)('0' + (num5));
                        }
                        *ps++ = (char)('0' + (div = (num4 * 8389) >> 23));
                        num4 -= div * 1000;
                    
                        L15:
                        *ps++ = (char)('0' + (div = (num4 * 5243) >> 19));
                        num4 -= div * 100;
                    
                        L14:
                        *ps++ = (char)('0' + (div = (num4 * 6554) >> 16));
                        num4 -= div * 10;
                    
                        L13:
                        *ps++ = (char)('0' + (num4));
                    }
                    *ps++ = (char)('0' + (div = (num3 * 8389) >> 23));
                    num3 -= div * 1000;
                
                    L11:
                    *ps++ = (char)('0' + (div = (num3 * 5243) >> 19));
                    num3 -= div * 100;
                
                    L10:
                    *ps++ = (char)('0' + (div = (num3 * 6554) >> 16));
                    num3 -= div * 10;
                    
                    L9:
                    *ps++ = (char)('0' + (num3));
                }
                *ps++ = (char)('0' + (div = (num2 * 8389) >> 23));
                num2 -= div * 1000;
            
                L7:
                *ps++ = (char)('0' + (div = (num2 * 5243) >> 19));
                num2 -= div * 100;
            
                L6:
                *ps++ = (char)('0' + (div = (num2 * 6554) >> 16));
                num2 -= div * 10;
            
                L5:
                *ps++ = (char)('0' + (num2));
            }
            *ps++ = (char)('0' + (div = (num1 * 8389) >> 23));
            num1 -= div * 1000;
        
            L3:
            *ps++ = (char)('0' + (div = (num1 * 5243) >> 19));
            num1 -= div * 100;
        
            L2:
            *ps++ = (char)('0' + (div = (num1 * 6554) >> 16));
            num1 -= div * 10;
        
            L1:
            *ps++ = (char)('0' + (num1));

            return new string(s);
        }

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="snum">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="snum"/> value.
        /// </returns>
        [SecuritySafeCritical]
        [CLSCompliant(false)]
        public unsafe static string ToString(ulong snum)
        {
            char* s = stackalloc char[20];
            char* ps = s;
            ulong num1 = snum, num2, num3, num4, num5, div;

            if (snum == 0)
                return "0";
            else if (snum == 18446744073709551615)
                return "18446744073709551615";

            if (num1 < 10000)
            {
                if (num1 < 10) goto L1;
                if (num1 < 100) goto L2;
                if (num1 < 1000) goto L3;
            }
            else
            {
                num2 = num1 / 10000;
                num1 -= num2 * 10000;
                if (num2 < 10000)
                {
                    if (num2 < 10) goto L5;
                    if (num2 < 100) goto L6;
                    if (num2 < 1000) goto L7;
                }
                else
                {
                    num3 = num2 / 10000;
                    num2 -= num3 * 10000;
                    if (num3 < 10000)
                    {
                        if (num3 < 10) goto L9;
                        if (num3 < 100) goto L10;
                        if (num3 < 1000) goto L11;
                    }
                    else
                    {
                        num4 = num3 / 10000;
                        num3 -= num4 * 10000;
                        if (num4 < 10000)
                        {
                            if (num4 < 10) goto L13;
                            if (num4 < 100) goto L14;
                            if (num4 < 1000) goto L15;
                        }
                        else
                        {
                            num5 = num4 / 10000;
                            num4 -= num5 * 10000;
                            if (num5 < 10000)
                            {
                                if (num5 < 10) goto L17;
                                if (num5 < 100) goto L18;
                            }

                            *ps++ = (char)('0' + (div = (num5 * 5243) >> 19));
                            num5 -= div * 100;

                            L18:
                            *ps++ = (char)('0' + (div = (num5 * 6554) >> 16));
                            num5 -= div * 10;
                        
                            L17:
                            *ps++ = (char)('0' + (num5));
                        }
                        *ps++ = (char)('0' + (div = (num4 * 8389) >> 23));
                        num4 -= div * 1000;
                    
                        L15:
                        *ps++ = (char)('0' + (div = (num4 * 5243) >> 19));
                        num4 -= div * 100;
                    
                        L14:
                        *ps++ = (char)('0' + (div = (num4 * 6554) >> 16));
                        num4 -= div * 10;
                    
                        L13:
                        *ps++ = (char)('0' + (num4));
                    }
                    *ps++ = (char)('0' + (div = (num3 * 8389) >> 23));
                    num3 -= div * 1000;
                
                    L11:
                    *ps++ = (char)('0' + (div = (num3 * 5243) >> 19));
                    num3 -= div * 100;
                
                    L10:
                    *ps++ = (char)('0' + (div = (num3 * 6554) >> 16));
                    num3 -= div * 10;
                    
                    L9:
                    *ps++ = (char)('0' + (num3));
                }
                *ps++ = (char)('0' + (div = (num2 * 8389) >> 23));
                num2 -= div * 1000;
            
                L7:
                *ps++ = (char)('0' + (div = (num2 * 5243) >> 19));
                num2 -= div * 100;
            
                L6:
                *ps++ = (char)('0' + (div = (num2 * 6554) >> 16));
                num2 -= div * 10;
            
                L5:
                *ps++ = (char)('0' + (num2));
            }
            *ps++ = (char)('0' + (div = (num1 * 8389) >> 23));
            num1 -= div * 1000;
        
            L3:
            *ps++ = (char)('0' + (div = (num1 * 5243) >> 19));
            num1 -= div * 100;
        
            L2:
            *ps++ = (char)('0' + (div = (num1 * 6554) >> 16));
            num1 -= div * 10;
        
            L1:
            *ps++ = (char)('0' + (num1));

            return new string(s);
        }

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="value"/> value.
        /// </returns>
        public static string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="value"/> value.
        /// </returns>
        public static string ToString(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="value"/> value.
        /// </returns>
        [CLSCompliant(false)]
        public static string ToString(sbyte value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a number to string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>
        /// A string representation of the <paramref name="value"/> value.
        /// </returns>
        public static string ToString(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a character to string.
        /// </summary>
        /// <param name="chr">The character to convert to string.</param>
        /// <returns>
        /// A string representation of the <paramref name="chr"/> value.
        /// </returns>
        public static string ToString(char chr)
        {
            return chr.ToString();
        }
    }
}
