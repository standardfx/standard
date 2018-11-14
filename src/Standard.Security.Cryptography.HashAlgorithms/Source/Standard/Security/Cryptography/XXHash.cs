using System;
using System.Runtime.CompilerServices;
using RS = Standard.Security.Cryptography.HashAlgorithms.RS;

namespace Standard.Security.Cryptography
{
    /// <summary>
    /// This is the base class for <see cref="XXHash32"/> and <see cref="XXHash64"/>. Do not use directly.
    /// </summary>
    public unsafe class XXHash
    {
        /// <summary>
        /// This is a protected constructor to prevent instantiation.
        /// </summary>
        protected XXHash()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint Read32(void* p)
        {
            return *(uint*)p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong Read64(void* p)
        {
            return *(ulong*)p;
        }

        internal static void Zero(void* target, int length)
        {
            byte* targetPtr = (byte*)target;

            while (length >= sizeof(ulong))
            {
                *(ulong*)targetPtr = 0;
                targetPtr += sizeof(ulong);
                length -= sizeof(ulong);
            }

            if (length >= sizeof(uint))
            {
                *(uint*)targetPtr = 0;
                targetPtr += sizeof(uint);
                length -= sizeof(uint);
            }

            if (length >= sizeof(ushort))
            {
                *(ushort*)targetPtr = 0;
                targetPtr += sizeof(ushort);
                length -= sizeof(ushort);
            }

            if (length > 0)
            {
                *targetPtr = 0;
                // targetP++;
                // length--;
            }
        }

        internal static void Copy(void* target, void* source, int length)
        {
            byte* sourcePtr = (byte*)source;
            byte* targetPtr = (byte*)target;

            while (length >= sizeof(ulong))
            {
                *(ulong*)targetPtr = *(ulong*)sourcePtr;
                targetPtr += sizeof(ulong);
                sourcePtr += sizeof(ulong);
                length -= sizeof(ulong);
            }

            if (length >= sizeof(uint))
            {
                *(uint*)targetPtr = *(uint*)sourcePtr;
                targetPtr += sizeof(uint);
                sourcePtr += sizeof(uint);
                length -= sizeof(uint);
            }

            if (length >= sizeof(ushort))
            {
                *(ushort*)targetPtr = *(ushort*)sourcePtr;
                targetPtr += sizeof(ushort);
                sourcePtr += sizeof(ushort);
                length -= sizeof(ushort);
            }


            if (length > 0)
            {
                *targetPtr = *sourcePtr;
                // targetP++;
                // sourceP++;
                // length--;
            }
        }

        internal static void Validate(byte[] bytes, int offset, int length)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes), RS.BufferCannotBeNull);

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), RS.CannotBeNegativeNumber);

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), RS.CannotBeNegativeNumber);

            if (offset + length > bytes.Length)
                throw new ArgumentException(RS.OffsetOverflow);
        }
    }
}