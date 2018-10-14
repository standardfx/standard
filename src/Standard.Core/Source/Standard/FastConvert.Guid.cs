using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Standard.Core;

namespace Standard
{
    partial class FastConvert
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        internal struct GuidStruct
        {
            [FieldOffset(0)]
            private Guid Value;

            [FieldOffset(0)]
            public readonly byte B00;
            [FieldOffset(1)]
            public readonly byte B01;
            [FieldOffset(2)]
            public readonly byte B02;
            [FieldOffset(3)]
            public readonly byte B03;
            [FieldOffset(4)]
            public readonly byte B04;
            [FieldOffset(5)]
            public readonly byte B05;

            [FieldOffset(6)]
            public readonly byte B06;
            [FieldOffset(7)]
            public readonly byte B07;
            [FieldOffset(8)]
            public readonly byte B08;
            [FieldOffset(9)]
            public readonly byte B09;

            [FieldOffset(10)]
            public readonly byte B10;
            [FieldOffset(11)]
            public readonly byte B11;

            [FieldOffset(12)]
            public readonly byte B12;
            [FieldOffset(13)]
            public readonly byte B13;
            [FieldOffset(14)]
            public readonly byte B14;
            [FieldOffset(15)]
            public readonly byte B15;

            public GuidStruct(Guid invisibleMembers)
                : this()
            {
                Value = invisibleMembers;
            }
        }

        private static readonly char[] WriteGuidLookup = new char[] 
            { 
                '0', '0', '0', '1', '0', '2', '0', '3', '0', '4', '0', '5', '0', '6', '0', '7', '0', '8', '0', '9', '0', 'a', '0', 'b', '0', 'c', '0', 'd', '0', 'e', '0', 'f', 
                '1', '0', '1', '1', '1', '2', '1', '3', '1', '4', '1', '5', '1', '6', '1', '7', '1', '8', '1', '9', '1', 'a', '1', 'b', '1', 'c', '1', 'd', '1', 'e', '1', 'f', 
                '2', '0', '2', '1', '2', '2', '2', '3', '2', '4', '2', '5', '2', '6', '2', '7', '2', '8', '2', '9', '2', 'a', '2', 'b', '2', 'c', '2', 'd', '2', 'e', '2', 'f', 
                '3', '0', '3', '1', '3', '2', '3', '3', '3', '4', '3', '5', '3', '6', '3', '7', '3', '8', '3', '9', '3', 'a', '3', 'b', '3', 'c', '3', 'd', '3', 'e', '3', 'f', 
                '4', '0', '4', '1', '4', '2', '4', '3', '4', '4', '4', '5', '4', '6', '4', '7', '4', '8', '4', '9', '4', 'a', '4', 'b', '4', 'c', '4', 'd', '4', 'e', '4', 'f', 
                '5', '0', '5', '1', '5', '2', '5', '3', '5', '4', '5', '5', '5', '6', '5', '7', '5', '8', '5', '9', '5', 'a', '5', 'b', '5', 'c', '5', 'd', '5', 'e', '5', 'f', 
                '6', '0', '6', '1', '6', '2', '6', '3', '6', '4', '6', '5', '6', '6', '6', '7', '6', '8', '6', '9', '6', 'a', '6', 'b', '6', 'c', '6', 'd', '6', 'e', '6', 'f', 
                '7', '0', '7', '1', '7', '2', '7', '3', '7', '4', '7', '5', '7', '6', '7', '7', '7', '8', '7', '9', '7', 'a', '7', 'b', '7', 'c', '7', 'd', '7', 'e', '7', 'f', 
                '8', '0', '8', '1', '8', '2', '8', '3', '8', '4', '8', '5', '8', '6', '8', '7', '8', '8', '8', '9', '8', 'a', '8', 'b', '8', 'c', '8', 'd', '8', 'e', '8', 'f', 
                '9', '0', '9', '1', '9', '2', '9', '3', '9', '4', '9', '5', '9', '6', '9', '7', '9', '8', '9', '9', '9', 'a', '9', 'b', '9', 'c', '9', 'd', '9', 'e', '9', 'f', 
                'a', '0', 'a', '1', 'a', '2', 'a', '3', 'a', '4', 'a', '5', 'a', '6', 'a', '7', 'a', '8', 'a', '9', 'a', 'a', 'a', 'b', 'a', 'c', 'a', 'd', 'a', 'e', 'a', 'f', 
                'b', '0', 'b', '1', 'b', '2', 'b', '3', 'b', '4', 'b', '5', 'b', '6', 'b', '7', 'b', '8', 'b', '9', 'b', 'a', 'b', 'b', 'b', 'c', 'b', 'd', 'b', 'e', 'b', 'f', 
                'c', '0', 'c', '1', 'c', '2', 'c', '3', 'c', '4', 'c', '5', 'c', '6', 'c', '7', 'c', '8', 'c', '9', 'c', 'a', 'c', 'b', 'c', 'c', 'c', 'd', 'c', 'e', 'c', 'f', 
                'd', '0', 'd', '1', 'd', '2', 'd', '3', 'd', '4', 'd', '5', 'd', '6', 'd', '7', 'd', '8', 'd', '9', 'd', 'a', 'd', 'b', 'd', 'c', 'd', 'd', 'd', 'e', 'd', 'f', 
                'e', '0', 'e', '1', 'e', '2', 'e', '3', 'e', '4', 'e', '5', 'e', '6', 'e', '7', 'e', '8', 'e', '9', 'e', 'a', 'e', 'b', 'e', 'c', 'e', 'd', 'e', 'e', 'e', 'f', 
                'f', '0', 'f', '1', 'f', '2', 'f', '3', 'f', '4', 'f', '5', 'f', '6', 'f', '7', 'f', '8', 'f', '9', 'f', 'a', 'f', 'b', 'f', 'c', 'f', 'd', 'f', 'e', 'f', 'f' 
            };

        /// <summary>
        /// Converts a <see cref="Guid"/> instance to string.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> instance to be converted.</param>
        public static string ToString(Guid guid)
        {
            return ToString(guid, "d");
        }

        /// <summary>
        /// Converts a <see cref="Guid"/> to string.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> instance to be converted.</param>
        /// <param name="format">
        /// One of the following (not case sensitive):
        /// 
        ///          Code | Example
        ///          -----|--------
        ///          d    | 1314FAD4-7505-439D-ABD2-DBD89242928C
        ///          n    | 1314FAD47505439DABD2DBD89242928C
        ///          b    | {1314FAD4-7505-439D-ABD2-DBD89242928C}
        ///          p    | (1314FAD4-7505-439D-ABD2-DBD89242928C)
        ///          x    | {0x409cb578,0x5dfc,0x436c,{0xb0,0xaa,0x95,0x56,0x18,0x69,0x0e,0x38}}
        /// </param>
        /// <returns>
        /// String representation of a <see cref="Guid"/>.
        /// </returns>
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(Guid guid, string format)
        {
            if (guid == null)
                throw new ArgumentNullException("guid");

            if (format == null || format.Length == 0)
                format = "d";
 
            if (format.Length != 1) 
                throw new FormatException(RS.InvalidGuidFormatChar);

            string guidString;
            char formatChar = format[0];

            if (formatChar == 'd' || formatChar == 'D')
            {
                // 1314FAD4-7505-439D-ABD2-DBD89242928C
                // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ
                //
                // Guid is guaranteed to be a 36 character string
                guidString = new string('\0', 36);
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        // get all the consts in place
                        guidChars[8] = '-';
                        guidChars[13] = '-';
                        guidChars[18] = '-';
                        guidChars[23] = '-';

                        // Bytes are in a different order than you might expect
                        // For: 35 91 8b c9 - 19 6d - 40 ea  - 97 79  - 88 9d 79 b7 53 f0 
                        // Get: C9 8B 91 35   6D 19   EA 40    97 79    88 9D 79 B7 53 F0 
                        // Ix:   0  1  2  3    4  5    6  7     8  9    10 11 12 13 14 15
                        //
                        // And we have to account for dashes
                        //
                        // So the map is like so:
                        // bytes[0]  -> chars[3]  -> buffer[ 6, 7]
                        // bytes[1]  -> chars[2]  -> buffer[ 4, 5]
                        // bytes[2]  -> chars[1]  -> buffer[ 2, 3]
                        // bytes[3]  -> chars[0]  -> buffer[ 0, 1]
                        // bytes[4]  -> chars[5]  -> buffer[11,12]
                        // bytes[5]  -> chars[4]  -> buffer[ 9,10]
                        // bytes[6]  -> chars[7]  -> buffer[16,17]
                        // bytes[7]  -> chars[6]  -> buffer[14,15]
                        // bytes[8]  -> chars[8]  -> buffer[19,20]
                        // bytes[9]  -> chars[9]  -> buffer[21,22]
                        // bytes[10] -> chars[10] -> buffer[24,25]
                        // bytes[11] -> chars[11] -> buffer[26,27]
                        // bytes[12] -> chars[12] -> buffer[28,29]
                        // bytes[13] -> chars[13] -> buffer[30,31]
                        // bytes[14] -> chars[14] -> buffer[32,33]
                        // bytes[15] -> chars[15] -> buffer[34,35]
                        GuidStruct visibleMembers = new GuidStruct(guid);

                        // bytes[0]
                        var b = visibleMembers.B00 * 2;
                        guidChars[6] = WriteGuidLookup[b];
                        guidChars[7] = WriteGuidLookup[b + 1];

                        // bytes[1]
                        b = visibleMembers.B01 * 2;
                        guidChars[4] = WriteGuidLookup[b];
                        guidChars[5] = WriteGuidLookup[b + 1];

                        // bytes[2]
                        b = visibleMembers.B02 * 2;
                        guidChars[2] = WriteGuidLookup[b];
                        guidChars[3] = WriteGuidLookup[b + 1];

                        // bytes[3]
                        b = visibleMembers.B03 * 2;
                        guidChars[0] = WriteGuidLookup[b];
                        guidChars[1] = WriteGuidLookup[b + 1];

                        // bytes[4]
                        b = visibleMembers.B04 * 2;
                        guidChars[11] = WriteGuidLookup[b];
                        guidChars[12] = WriteGuidLookup[b + 1];

                        // bytes[5]
                        b = visibleMembers.B05 * 2;
                        guidChars[9] = WriteGuidLookup[b];
                        guidChars[10] = WriteGuidLookup[b + 1];

                        // bytes[6]
                        b = visibleMembers.B06 * 2;
                        guidChars[16] = WriteGuidLookup[b];
                        guidChars[17] = WriteGuidLookup[b + 1];

                        // bytes[7]
                        b = visibleMembers.B07 * 2;
                        guidChars[14] = WriteGuidLookup[b];
                        guidChars[15] = WriteGuidLookup[b + 1];

                        // bytes[8]
                        b = visibleMembers.B08 * 2;
                        guidChars[19] = WriteGuidLookup[b];
                        guidChars[20] = WriteGuidLookup[b + 1];

                        // bytes[9]
                        b = visibleMembers.B09 * 2;
                        guidChars[21] = WriteGuidLookup[b];
                        guidChars[22] = WriteGuidLookup[b + 1];

                        // bytes[10]
                        b = visibleMembers.B10 * 2;
                        guidChars[24] = WriteGuidLookup[b];
                        guidChars[25] = WriteGuidLookup[b + 1];

                        // bytes[11]
                        b = visibleMembers.B11 * 2;
                        guidChars[26] = WriteGuidLookup[b];
                        guidChars[27] = WriteGuidLookup[b + 1];

                        // bytes[12]
                        b = visibleMembers.B12 * 2;
                        guidChars[28] = WriteGuidLookup[b];
                        guidChars[29] = WriteGuidLookup[b + 1];

                        // bytes[13]
                        b = visibleMembers.B13 * 2;
                        guidChars[30] = WriteGuidLookup[b];
                        guidChars[31] = WriteGuidLookup[b + 1];

                        // bytes[14]
                        b = visibleMembers.B14 * 2;
                        guidChars[32] = WriteGuidLookup[b];
                        guidChars[33] = WriteGuidLookup[b + 1];

                        // bytes[15]
                        b = visibleMembers.B15 * 2;
                        guidChars[34] = WriteGuidLookup[b];
                        guidChars[35] = WriteGuidLookup[b + 1];
                    }
                }
            }
            else if (formatChar == 'n' || formatChar == 'N')
            {
                // 1314FAD47505439DABD2DBD89242928C
                // 0123456789ABCDEFGHIJKLMNOPQRSTUV
                //
                // Guid is guaranteed to be a 32 character string

                guidString = new string('\0', 32);
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        GuidStruct visibleMembers = new GuidStruct(guid);

                        // bytes[0]
                        var b = visibleMembers.B00 * 2;
                        guidChars[6] = WriteGuidLookup[b];
                        guidChars[7] = WriteGuidLookup[b + 1];

                        // bytes[1]
                        b = visibleMembers.B01 * 2;
                        guidChars[4] = WriteGuidLookup[b];
                        guidChars[5] = WriteGuidLookup[b + 1];

                        // bytes[2]
                        b = visibleMembers.B02 * 2;
                        guidChars[2] = WriteGuidLookup[b];
                        guidChars[3] = WriteGuidLookup[b + 1];

                        // bytes[3]
                        b = visibleMembers.B03 * 2;
                        guidChars[0] = WriteGuidLookup[b];
                        guidChars[1] = WriteGuidLookup[b + 1];

                        // bytes[4]
                        b = visibleMembers.B04 * 2;
                        guidChars[10] = WriteGuidLookup[b];
                        guidChars[11] = WriteGuidLookup[b + 1];

                        // bytes[5]
                        b = visibleMembers.B05 * 2;
                        guidChars[8] = WriteGuidLookup[b];
                        guidChars[9] = WriteGuidLookup[b + 1];

                        // bytes[6]
                        b = visibleMembers.B06 * 2;
                        guidChars[14] = WriteGuidLookup[b];
                        guidChars[15] = WriteGuidLookup[b + 1];

                        // bytes[7]
                        b = visibleMembers.B07 * 2;
                        guidChars[12] = WriteGuidLookup[b];
                        guidChars[13] = WriteGuidLookup[b + 1];

                        // bytes[8]
                        b = visibleMembers.B08 * 2;
                        guidChars[16] = WriteGuidLookup[b];
                        guidChars[17] = WriteGuidLookup[b + 1];

                        // bytes[9]
                        b = visibleMembers.B09 * 2;
                        guidChars[18] = WriteGuidLookup[b];
                        guidChars[19] = WriteGuidLookup[b + 1];

                        // bytes[10]
                        b = visibleMembers.B10 * 2;
                        guidChars[20] = WriteGuidLookup[b];
                        guidChars[21] = WriteGuidLookup[b + 1];

                        // bytes[11]
                        b = visibleMembers.B11 * 2;
                        guidChars[22] = WriteGuidLookup[b];
                        guidChars[23] = WriteGuidLookup[b + 1];

                        // bytes[12]
                        b = visibleMembers.B12 * 2;
                        guidChars[24] = WriteGuidLookup[b];
                        guidChars[25] = WriteGuidLookup[b + 1];

                        // bytes[13]
                        b = visibleMembers.B13 * 2;
                        guidChars[26] = WriteGuidLookup[b];
                        guidChars[27] = WriteGuidLookup[b + 1];

                        // bytes[14]
                        b = visibleMembers.B14 * 2;
                        guidChars[28] = WriteGuidLookup[b];
                        guidChars[29] = WriteGuidLookup[b + 1];

                        // bytes[15]
                        b = visibleMembers.B15 * 2;
                        guidChars[30] = WriteGuidLookup[b];
                        guidChars[31] = WriteGuidLookup[b + 1];
                    }
                }
            }
            else if (formatChar == 'b' || formatChar == 'B')
            {
                // {1314FAD4-7505-439D-ABD2-DBD89242928C}
                // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZab
                //
                // Guid is guaranteed to be a 38 character string
                guidString = new string('\0', 38);
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        // get all the consts in place
                        guidChars[0] = '{';
                        guidChars[9] = '-';
                        guidChars[14] = '-';
                        guidChars[19] = '-';
                        guidChars[24] = '-';
                        guidChars[37] = '}';

                        GuidStruct visibleMembers = new GuidStruct(guid);

                        // bytes[0]
                        var b = visibleMembers.B00 * 2;
                        guidChars[7] = WriteGuidLookup[b];
                        guidChars[8] = WriteGuidLookup[b + 1];

                        // bytes[1]
                        b = visibleMembers.B01 * 2;
                        guidChars[5] = WriteGuidLookup[b];
                        guidChars[6] = WriteGuidLookup[b + 1];

                        // bytes[2]
                        b = visibleMembers.B02 * 2;
                        guidChars[3] = WriteGuidLookup[b];
                        guidChars[4] = WriteGuidLookup[b + 1];

                        // bytes[3]
                        b = visibleMembers.B03 * 2;
                        guidChars[1] = WriteGuidLookup[b];
                        guidChars[2] = WriteGuidLookup[b + 1];

                        // bytes[4]
                        b = visibleMembers.B04 * 2;
                        guidChars[12] = WriteGuidLookup[b];
                        guidChars[13] = WriteGuidLookup[b + 1];

                        // bytes[5]
                        b = visibleMembers.B05 * 2;
                        guidChars[10] = WriteGuidLookup[b];
                        guidChars[11] = WriteGuidLookup[b + 1];

                        // bytes[6]
                        b = visibleMembers.B06 * 2;
                        guidChars[17] = WriteGuidLookup[b];
                        guidChars[18] = WriteGuidLookup[b + 1];

                        // bytes[7]
                        b = visibleMembers.B07 * 2;
                        guidChars[15] = WriteGuidLookup[b];
                        guidChars[16] = WriteGuidLookup[b + 1];

                        // bytes[8]
                        b = visibleMembers.B08 * 2;
                        guidChars[20] = WriteGuidLookup[b];
                        guidChars[21] = WriteGuidLookup[b + 1];

                        // bytes[9]
                        b = visibleMembers.B09 * 2;
                        guidChars[22] = WriteGuidLookup[b];
                        guidChars[23] = WriteGuidLookup[b + 1];

                        // bytes[10]
                        b = visibleMembers.B10 * 2;
                        guidChars[25] = WriteGuidLookup[b];
                        guidChars[26] = WriteGuidLookup[b + 1];

                        // bytes[11]
                        b = visibleMembers.B11 * 2;
                        guidChars[27] = WriteGuidLookup[b];
                        guidChars[28] = WriteGuidLookup[b + 1];

                        // bytes[12]
                        b = visibleMembers.B12 * 2;
                        guidChars[29] = WriteGuidLookup[b];
                        guidChars[30] = WriteGuidLookup[b + 1];

                        // bytes[13]
                        b = visibleMembers.B13 * 2;
                        guidChars[31] = WriteGuidLookup[b];
                        guidChars[32] = WriteGuidLookup[b + 1];

                        // bytes[14]
                        b = visibleMembers.B14 * 2;
                        guidChars[33] = WriteGuidLookup[b];
                        guidChars[34] = WriteGuidLookup[b + 1];

                        // bytes[15]
                        b = visibleMembers.B15 * 2;
                        guidChars[35] = WriteGuidLookup[b];
                        guidChars[36] = WriteGuidLookup[b + 1];
                    }
                }
            }
            else if (formatChar == 'p' || formatChar == 'P')
            {
                // (1314FAD4-7505-439D-ABD2-DBD89242928C)
                // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZab
                //
                // Guid is guaranteed to be a 38 character string
                guidString = new string('\0', 38);
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        // get all the consts in place
                        guidChars[0] = '(';
                        guidChars[9] = '-';
                        guidChars[14] = '-';
                        guidChars[19] = '-';
                        guidChars[24] = '-';
                        guidChars[37] = ')';

                        GuidStruct visibleMembers = new GuidStruct(guid);

                        // bytes[0]
                        var b = visibleMembers.B00 * 2;
                        guidChars[7] = WriteGuidLookup[b];
                        guidChars[8] = WriteGuidLookup[b + 1];

                        // bytes[1]
                        b = visibleMembers.B01 * 2;
                        guidChars[5] = WriteGuidLookup[b];
                        guidChars[6] = WriteGuidLookup[b + 1];

                        // bytes[2]
                        b = visibleMembers.B02 * 2;
                        guidChars[3] = WriteGuidLookup[b];
                        guidChars[4] = WriteGuidLookup[b + 1];

                        // bytes[3]
                        b = visibleMembers.B03 * 2;
                        guidChars[1] = WriteGuidLookup[b];
                        guidChars[2] = WriteGuidLookup[b + 1];

                        // bytes[4]
                        b = visibleMembers.B04 * 2;
                        guidChars[12] = WriteGuidLookup[b];
                        guidChars[13] = WriteGuidLookup[b + 1];

                        // bytes[5]
                        b = visibleMembers.B05 * 2;
                        guidChars[10] = WriteGuidLookup[b];
                        guidChars[11] = WriteGuidLookup[b + 1];

                        // bytes[6]
                        b = visibleMembers.B06 * 2;
                        guidChars[17] = WriteGuidLookup[b];
                        guidChars[18] = WriteGuidLookup[b + 1];

                        // bytes[7]
                        b = visibleMembers.B07 * 2;
                        guidChars[15] = WriteGuidLookup[b];
                        guidChars[16] = WriteGuidLookup[b + 1];

                        // bytes[8]
                        b = visibleMembers.B08 * 2;
                        guidChars[20] = WriteGuidLookup[b];
                        guidChars[21] = WriteGuidLookup[b + 1];

                        // bytes[9]
                        b = visibleMembers.B09 * 2;
                        guidChars[22] = WriteGuidLookup[b];
                        guidChars[23] = WriteGuidLookup[b + 1];

                        // bytes[10]
                        b = visibleMembers.B10 * 2;
                        guidChars[25] = WriteGuidLookup[b];
                        guidChars[26] = WriteGuidLookup[b + 1];

                        // bytes[11]
                        b = visibleMembers.B11 * 2;
                        guidChars[27] = WriteGuidLookup[b];
                        guidChars[28] = WriteGuidLookup[b + 1];

                        // bytes[12]
                        b = visibleMembers.B12 * 2;
                        guidChars[29] = WriteGuidLookup[b];
                        guidChars[30] = WriteGuidLookup[b + 1];

                        // bytes[13]
                        b = visibleMembers.B13 * 2;
                        guidChars[31] = WriteGuidLookup[b];
                        guidChars[32] = WriteGuidLookup[b + 1];

                        // bytes[14]
                        b = visibleMembers.B14 * 2;
                        guidChars[33] = WriteGuidLookup[b];
                        guidChars[34] = WriteGuidLookup[b + 1];

                        // bytes[15]
                        b = visibleMembers.B15 * 2;
                        guidChars[35] = WriteGuidLookup[b];
                        guidChars[36] = WriteGuidLookup[b + 1];
                    }
                }
            }
            else if (formatChar == 'x' || formatChar == 'X')
            {
                // {0x409cb578,0x5dfc,0x436c,{0xb0,0xaa,0x95,0x56,0x18,0x69,0x0e,0x38}}
                // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$*<>
                //
                // Guid is guaranteed to be a 68 character string
                guidString = new string('\0', 68);
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        // get all the consts in place
                        guidChars[0] = '{';
                        guidChars[1] = '0';
                        guidChars[2] = 'x';
                        guidChars[11] = ',';
                        guidChars[12] = '0';
                        guidChars[13] = 'x';
                        guidChars[18] = ',';
                        guidChars[19] = '0';
                        guidChars[20] = 'x';
                        guidChars[25] = ',';
                        guidChars[26] = '{';
                        guidChars[27] = '0';
                        guidChars[28] = 'x';
                        guidChars[31] = ',';
                        guidChars[32] = '0';
                        guidChars[33] = 'x';
                        guidChars[36] = ',';
                        guidChars[37] = '0';
                        guidChars[38] = 'x';
                        guidChars[41] = ',';
                        guidChars[42] = '0';
                        guidChars[43] = 'x';
                        guidChars[46] = ',';
                        guidChars[47] = '0';
                        guidChars[48] = 'x';
                        guidChars[51] = ',';
                        guidChars[52] = '0';
                        guidChars[53] = 'x';
                        guidChars[56] = ',';
                        guidChars[57] = '0';
                        guidChars[58] = 'x';
                        guidChars[61] = ',';
                        guidChars[62] = '0';
                        guidChars[63] = 'x';
                        guidChars[66] = '}';
                        guidChars[67] = '}';

                        GuidStruct visibleMembers = new GuidStruct(guid);

                        // bytes[0]
                        var b = visibleMembers.B00 * 2;
                        guidChars[9] = WriteGuidLookup[b];
                        guidChars[10] = WriteGuidLookup[b + 1];

                        // bytes[1]
                        b = visibleMembers.B01 * 2;
                        guidChars[7] = WriteGuidLookup[b];
                        guidChars[8] = WriteGuidLookup[b + 1];

                        // bytes[2]
                        b = visibleMembers.B02 * 2;
                        guidChars[5] = WriteGuidLookup[b];
                        guidChars[6] = WriteGuidLookup[b + 1];

                        // bytes[3]
                        b = visibleMembers.B03 * 2;
                        guidChars[3] = WriteGuidLookup[b];
                        guidChars[4] = WriteGuidLookup[b + 1];

                        // bytes[4]
                        b = visibleMembers.B04 * 2;
                        guidChars[16] = WriteGuidLookup[b];
                        guidChars[17] = WriteGuidLookup[b + 1];

                        // bytes[5]
                        b = visibleMembers.B05 * 2;
                        guidChars[14] = WriteGuidLookup[b];
                        guidChars[15] = WriteGuidLookup[b + 1];

                        // bytes[6]
                        b = visibleMembers.B06 * 2;
                        guidChars[23] = WriteGuidLookup[b];
                        guidChars[24] = WriteGuidLookup[b + 1];

                        // bytes[7]
                        b = visibleMembers.B07 * 2;
                        guidChars[21] = WriteGuidLookup[b];
                        guidChars[22] = WriteGuidLookup[b + 1];

                        // bytes[8]
                        b = visibleMembers.B08 * 2;
                        guidChars[29] = WriteGuidLookup[b];
                        guidChars[30] = WriteGuidLookup[b + 1];

                        // bytes[9]
                        b = visibleMembers.B09 * 2;
                        guidChars[34] = WriteGuidLookup[b];
                        guidChars[35] = WriteGuidLookup[b + 1];

                        // bytes[10]
                        b = visibleMembers.B10 * 2;
                        guidChars[39] = WriteGuidLookup[b];
                        guidChars[40] = WriteGuidLookup[b + 1];

                        // bytes[11]
                        b = visibleMembers.B11 * 2;
                        guidChars[44] = WriteGuidLookup[b];
                        guidChars[45] = WriteGuidLookup[b + 1];

                        // bytes[12]
                        b = visibleMembers.B12 * 2;
                        guidChars[49] = WriteGuidLookup[b];
                        guidChars[50] = WriteGuidLookup[b + 1];

                        // bytes[13]
                        b = visibleMembers.B13 * 2;
                        guidChars[54] = WriteGuidLookup[b];
                        guidChars[55] = WriteGuidLookup[b + 1];

                        // bytes[14]
                        b = visibleMembers.B14 * 2;
                        guidChars[59] = WriteGuidLookup[b];
                        guidChars[60] = WriteGuidLookup[b + 1];

                        // bytes[15]
                        b = visibleMembers.B15 * 2;
                        guidChars[64] = WriteGuidLookup[b];
                        guidChars[65] = WriteGuidLookup[b + 1];
                    }
                }
            }
            else
            {
                throw new FormatException(RS.InvalidGuidFormatChar);
            }

            return guidString;
        }

        /// <summary>
        /// Converts a string to <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The string representation of a <see cref="Guid"/>.</param>
        /// <param name="format">One of the following (not case sensitive): d, n, b, p, x</param>
        public static Guid ToGuid(string value, string format)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            string guidString = value.Trim();
            if (guidString.Length == 0)
                throw new FormatException(RS.GuidEmptyString);

            if (format == null || format.Length == 0)
                format = "d";
 
            if (format.Length != 1) 
                throw new FormatException(RS.InvalidGuidFormatChar);

            char formatChar = format[0];
            switch (formatChar)
            {
                case 'b':
                case 'B':
                    return ToGuidInternal(guidString, 'b');

                case 'p':
                case 'P':
                    return ToGuidInternal(guidString, 'p');

                case 'x':
                case 'X':
                    return ToGuidInternal(guidString, 'x');

                case 'd':
                case 'D':
                    return ToGuidInternal(guidString, 'd');

                case 'n':
                case 'N':
                    return ToGuidInternal(guidString, 'n');

                default:
                    throw new FormatException(RS.InvalidGuidFormatChar);
            }
        }

        /// <summary>
        /// Converts a string to <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The string representation of a <see cref="Guid"/>.</param>
        public static Guid ToGuid(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            string guidString = value.Trim();
            if (guidString.Length == 0)
                throw new FormatException(RS.GuidEmptyString);

            bool dashesExistInString = (guidString.IndexOf('-', 0) >= 0);                
            bool bracesExistInString = guidString.IndexOf('{', 0) >= 0;
            if (dashesExistInString && bracesExistInString)
            {
                // format b
                return ToGuidInternal(guidString, 'b');
            }
            else if (!dashesExistInString && bracesExistInString)
            {
                // format x
                return ToGuidInternal(guidString, 'x');
            }
            else
            {
                bool parenthesisExistInString = (guidString.IndexOf('(', 0) >= 0);
                if (dashesExistInString && parenthesisExistInString)
                {
                    // format p
                    return ToGuidInternal(guidString, 'p');
                }
                else if (dashesExistInString)
                {
                    // format d
                    return ToGuidInternal(guidString, 'd');
                }
                else
                {
                    // format n
                    return ToGuidInternal(guidString, 'n');
                }
            }
        }

        private static Guid ToGuidInternal(string value, char formatChar)
        {
            string nFormatValue = RemoveGuidFormatChars(value, formatChar);
            string endianSwap = new string('\0', 32);
            unsafe
            {
                fixed (char* guidChars = endianSwap)
                {
                    // bytes[0]
                    guidChars[0] = nFormatValue[6];
                    guidChars[1] = nFormatValue[7];

                    // bytes[1]
                    guidChars[2] = nFormatValue[4];
                    guidChars[3] = nFormatValue[5];

                    // bytes[2]
                    guidChars[4] = nFormatValue[2];
                    guidChars[5] = nFormatValue[3];

                    // bytes[3]
                    guidChars[6] = nFormatValue[0];
                    guidChars[7] = nFormatValue[1];

                    // bytes[4]
                    guidChars[8] = nFormatValue[10];
                    guidChars[9] = nFormatValue[11];

                    // bytes[5]
                    guidChars[10] = nFormatValue[8];
                    guidChars[11] = nFormatValue[9];

                    // bytes[6]
                    guidChars[12] = nFormatValue[14];
                    guidChars[13] = nFormatValue[15];

                    // bytes[7]
                    guidChars[14] = nFormatValue[12];
                    guidChars[15] = nFormatValue[13];

                    // bytes[8]
                    guidChars[16] = nFormatValue[16];
                    guidChars[17] = nFormatValue[17];

                    // bytes[9]
                    guidChars[18] = nFormatValue[18];
                    guidChars[19] = nFormatValue[19];

                    // bytes[10]
                    guidChars[20] = nFormatValue[20];
                    guidChars[21] = nFormatValue[21];

                    // bytes[11]
                    guidChars[22] = nFormatValue[22];
                    guidChars[23] = nFormatValue[23];

                    // bytes[12]
                    guidChars[24] = nFormatValue[24];
                    guidChars[25] = nFormatValue[25];

                    // bytes[13]
                    guidChars[26] = nFormatValue[26];
                    guidChars[27] = nFormatValue[27];

                    // bytes[14]
                    guidChars[28] = nFormatValue[28];
                    guidChars[29] = nFormatValue[29];

                    // bytes[15]
                    guidChars[30] = nFormatValue[30];
                    guidChars[31] = nFormatValue[31];
                }
            }

            return new Guid(FromBase16String(endianSwap));
        }

        private static string RemoveGuidFormatChars(string value, char formatChar)
        {
            if (formatChar == 'n')
                return value;
                
            string guidString = new string('\0', 32);

            if (formatChar == 'd')
            {
                // 1314FAD4-7505-439D-ABD2-DBD89242928C
                // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        guidChars[0] = value[0];
                        guidChars[1] = value[1];
                        guidChars[2] = value[2];
                        guidChars[3] = value[3];
                        guidChars[4] = value[4];
                        guidChars[5] = value[5];
                        guidChars[6] = value[6];
                        guidChars[7] = value[7];
                        guidChars[8] = value[9];
                        guidChars[9] = value[10];
                        guidChars[10] = value[11];
                        guidChars[11] = value[12];
                        guidChars[12] = value[14];
                        guidChars[13] = value[15];
                        guidChars[14] = value[16];
                        guidChars[15] = value[17];
                        guidChars[16] = value[19];
                        guidChars[17] = value[20];
                        guidChars[18] = value[21];
                        guidChars[19] = value[22];
                        guidChars[20] = value[24];
                        guidChars[21] = value[25];
                        guidChars[22] = value[26];
                        guidChars[23] = value[27];
                        guidChars[24] = value[28];
                        guidChars[25] = value[29];
                        guidChars[26] = value[30];
                        guidChars[27] = value[31];
                        guidChars[28] = value[32];
                        guidChars[29] = value[33];
                        guidChars[30] = value[34];
                        guidChars[31] = value[35];
                    }
                }
            }
            else if (formatChar == 'p' || formatChar == 'b')
            {
                // (1314FAD4-7505-439D-ABD2-DBD89242928C)
                // {1314FAD4-7505-439D-ABD2-DBD89242928C}
                // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZab
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        guidChars[0] = value[1];
                        guidChars[1] = value[2];
                        guidChars[2] = value[3];
                        guidChars[3] = value[4];
                        guidChars[4] = value[5];
                        guidChars[5] = value[6];
                        guidChars[6] = value[7];
                        guidChars[7] = value[8];
                        guidChars[8] = value[10];
                        guidChars[9] = value[11];
                        guidChars[10] = value[12];
                        guidChars[11] = value[13];
                        guidChars[12] = value[15];
                        guidChars[13] = value[16];
                        guidChars[14] = value[17];
                        guidChars[15] = value[18];
                        guidChars[16] = value[20];
                        guidChars[17] = value[21];
                        guidChars[18] = value[22];
                        guidChars[19] = value[23];
                        guidChars[20] = value[25];
                        guidChars[21] = value[26];
                        guidChars[22] = value[27];
                        guidChars[23] = value[28];
                        guidChars[24] = value[29];
                        guidChars[25] = value[30];
                        guidChars[26] = value[31];
                        guidChars[27] = value[32];
                        guidChars[28] = value[33];
                        guidChars[29] = value[34];
                        guidChars[30] = value[35];
                        guidChars[31] = value[36];
                    }
                }
            }
            else if (formatChar == 'x')
            {
                // {0x409cb578,0x5dfc,0x436c,{0xb0,0xaa,0x95,0x56,0x18,0x69,0x0e,0x38}}
                // 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$*<>
                unsafe
                {
                    fixed (char* guidChars = guidString)
                    {
                        guidChars[0] = value[3];
                        guidChars[1] = value[4];
                        guidChars[2] = value[5];
                        guidChars[3] = value[6];
                        guidChars[4] = value[7];
                        guidChars[5] = value[8];
                        guidChars[6] = value[9];
                        guidChars[7] = value[10];

                        guidChars[8] = value[14];
                        guidChars[9] = value[15];
                        guidChars[10] = value[16];
                        guidChars[11] = value[17];

                        guidChars[12] = value[21];
                        guidChars[13] = value[22];
                        guidChars[14] = value[23];
                        guidChars[15] = value[24];

                        guidChars[16] = value[29];
                        guidChars[17] = value[30];

                        guidChars[18] = value[34];
                        guidChars[19] = value[35];

                        guidChars[20] = value[39];
                        guidChars[21] = value[40];

                        guidChars[22] = value[44];
                        guidChars[23] = value[45];

                        guidChars[24] = value[49];
                        guidChars[25] = value[50];

                        guidChars[26] = value[54];
                        guidChars[27] = value[55];

                        guidChars[28] = value[59];
                        guidChars[29] = value[60];

                        guidChars[30] = value[64];
                        guidChars[31] = value[65];
                    }
                }                
            }

            return guidString;
        }
    }
}
