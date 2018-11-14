using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Standard.Security.Cryptography
{
    /// <summary>
    /// A 64-bit implementation of the XXHash algorithm.
    /// </summary>
    public unsafe partial class XXHash64 : XXHash
    {
        private const ulong Prime64Of1 = 11400714785074694791ul;
        private const ulong Prime64Of2 = 14029467366897019727ul;
        private const ulong Prime64Of3 = 1609587929392839161ul;
        private const ulong Prime64Of4 = 9650029242287828579ul;
        private const ulong Prime64Of5 = 2870177450012600261ul;

        [StructLayout(LayoutKind.Sequential)]
        private struct State
        {
            public ulong TotalLen;
            public ulong V1;
            public ulong V2;
            public ulong V3;
            public ulong V4;
            public fixed ulong Mem64[4];
            public uint MemSize;
        }

        private State _state;

        /// <summary>
        /// Hash value of an empty buffer.
        /// </summary>
        [CLSCompliant(false)]
        public const ulong EmptyHash = 17241709254077376921;

        /// <summary>
        /// Creates a new instance of the <see cref="XXHash64"/> class.
        /// </summary>
        public XXHash64()
        {
            Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Rotl64(ulong x, int r)
        {
            return (x << r) | (x >> (64 - r));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Round(ulong acc, ulong input)
        {
            return Rotl64(acc + input * Prime64Of2, 31) * Prime64Of1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong MergeRound(ulong acc, ulong val)
        {
            return (acc ^ Round(0, val)) * Prime64Of1 + Prime64Of4;
        }

        /// <see cref="DigestOf(byte[], int, int)"/>
        [CLSCompliant(false)]
        public static unsafe ulong DigestOf(void* bytes, int length)
        {
            return HashInternal(bytes, length, 0);
        }

        /// <see cref="DigestOf(byte[], int, int)"/>
        [CLSCompliant(false)]
        public static unsafe ulong DigestOf(ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytesPtr = &MemoryMarshal.GetReference(bytes))
            {
                return DigestOf(bytesPtr, bytes.Length);
            }
        }

        /// <summary>
        /// Calculates the hash value of a specified buffer.
        /// </summary>
        /// <param name="bytes">The buffer to calculate the hash value from.</param>
        /// <param name="offset">Starting offset.</param>
        /// <param name="length">Length of buffer.</param>
        /// <returns>
        /// The hash value (or digest) of <paramref name="bytes"/>.
        /// </returns>
        [CLSCompliant(false)]
        public static unsafe ulong DigestOf(byte[] bytes, int offset, int length)
        {
            Validate(bytes, offset, length);

            fixed (byte* bytes0 = bytes)
            {
                return DigestOf(bytes0 + offset, length);
            }
        }

        private static ulong HashInternal(void* input, int len, ulong seed)
        {
            byte* p = (byte*)input;
            byte* bEnd = p + len;
            ulong h64;

            if (len >= 32)
            {
                byte* limit = bEnd - 32;
                ulong v1 = seed + Prime64Of1 + Prime64Of2;
                ulong v2 = seed + Prime64Of2;
                ulong v3 = seed + 0;
                ulong v4 = seed - Prime64Of1;

                do
                {
                    v1 = Round(v1, Read64(p + 0));
                    v2 = Round(v2, Read64(p + 8));
                    v3 = Round(v3, Read64(p + 16));
                    v4 = Round(v4, Read64(p + 24));
                    p += 32;
                }
                while (p <= limit);

                h64 = Rotl64(v1, 1) + Rotl64(v2, 7) + Rotl64(v3, 12) + Rotl64(v4, 18);
                h64 = MergeRound(h64, v1);
                h64 = MergeRound(h64, v2);
                h64 = MergeRound(h64, v3);
                h64 = MergeRound(h64, v4);
            }
            else
            {
                h64 = seed + Prime64Of5;
            }

            h64 += (ulong)len;

            while (p + 8 <= bEnd)
            {
                h64 ^= Round(0, Read64(p));
                h64 = Rotl64(h64, 27) * Prime64Of1 + Prime64Of4;
                p += 8;
            }

            if (p + 4 <= bEnd)
            {
                h64 ^= Read32(p) * Prime64Of1;
                h64 = Rotl64(h64, 23) * Prime64Of2 + Prime64Of3;
                p += 4;
            }

            while (p < bEnd)
            {
                h64 ^= (*p) * Prime64Of5;
                h64 = Rotl64(h64, 11) * Prime64Of1;
                p++;
            }

            h64 ^= h64 >> 33;
            h64 *= Prime64Of2;
            h64 ^= h64 >> 29;
            h64 *= Prime64Of3;
            h64 ^= h64 >> 32;

            return h64;
        }

        /// <summary>
        /// Resets the hash calculation engine to its default state.
        /// </summary>
        public unsafe void Reset()
        {
            fixed (State* statePtr = &_state)
            {
                ResetInternal(statePtr, 0);
            }
        }

        private static void ResetInternal(State* state, ulong seed)
        {
            Zero(state, sizeof(State));
            state->V1 = seed + Prime64Of1 + Prime64Of2;
            state->V2 = seed + Prime64Of2;
            state->V3 = seed + 0;
            state->V4 = seed - Prime64Of1;
        }

        /// <see cref="Update(byte[], int, int)"/>
        [CLSCompliant(false)]
        public unsafe void Update(byte* bytes, int length)
        {
            fixed (State* statePtr = &_state)
            {
                UpdateInternal(statePtr, bytes, length);
            }
        }

        /// <see cref="Update(byte[], int, int)"/>
        public unsafe void Update(ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytesPtr = &MemoryMarshal.GetReference(bytes))
            {
                Update(bytesPtr, bytes.Length);
            }
        }

        /// <summary>
        /// Updates the hash of the buffer specified.
        /// </summary>
        /// <param name="bytes">The buffer to update the hash value from.</param>
        /// <param name="offset">Starting offset.</param>
        /// <param name="length">Length of buffer.</param>
        public unsafe void Update(byte[] bytes, int offset, int length)
        {
            Validate(bytes, offset, length);

            fixed (byte* bytesPtr = bytes)
            {
                Update(bytesPtr + offset, length);
            }
        }

        private static void UpdateInternal(State* state, void* input, int len)
        {
            byte* p = (byte*)input;
            byte* bEnd = p + len;

            state->TotalLen += (ulong)len;

            if (state->MemSize + len < 32)
            {
                // fill in tmp buffer
                Copy((byte*)state->Mem64 + state->MemSize, input, len);
                state->MemSize += (uint)len;
                return;
            }

            if (state->MemSize > 0)
            {
                // tmp buffer is full
                Copy((byte*)state->Mem64 + state->MemSize, input, (int)(32 - state->MemSize));
                state->V1 = Round(state->V1, Read64(state->Mem64 + 0));
                state->V2 = Round(state->V2, Read64(state->Mem64 + 1));
                state->V3 = Round(state->V3, Read64(state->Mem64 + 2));
                state->V4 = Round(state->V4, Read64(state->Mem64 + 3));
                p += 32 - state->MemSize;
                state->MemSize = 0;
            }

            if (p + 32 <= bEnd)
            {
                byte* limit = bEnd - 32;
                ulong v1 = state->V1;
                ulong v2 = state->V2;
                ulong v3 = state->V3;
                ulong v4 = state->V4;

                do
                {
                    v1 = Round(v1, Read64(p + 0));
                    v2 = Round(v2, Read64(p + 8));
                    v3 = Round(v3, Read64(p + 16));
                    v4 = Round(v4, Read64(p + 24));
                    p += 32;
                }
                while (p <= limit);

                state->V1 = v1;
                state->V2 = v2;
                state->V3 = v3;
                state->V4 = v4;
            }

            if (p < bEnd)
            {
                Copy(state->Mem64, p, (int)(bEnd - p));
                state->MemSize = (uint)(bEnd - p);
            }
        }

        /// <summary>
        /// Returns the hash value calculated at the current instance.
        /// </summary>
        /// <returns>
        /// The hash value calculated at the current instance. This value is subject to change until the 
        /// entire payload is calculated.
        /// </returns>
        [CLSCompliant(false)]
        public unsafe ulong Digest()
        {
            fixed (State* statePtr = &_state)
            {
                return DigestInternal(statePtr);
            }
        }

        /// <see cref="Digest()"/>
        public byte[] DigestBytes()
        {
            return BitConverter.GetBytes(Digest());
        }

        private static ulong DigestInternal(State* state)
        {
            byte* p = (byte*)state->Mem64;
            byte* bEnd = (byte*)state->Mem64 + state->MemSize;
            ulong h64;

            if (state->TotalLen >= 32)
            {
                ulong v1 = state->V1;
                ulong v2 = state->V2;
                ulong v3 = state->V3;
                ulong v4 = state->V4;

                h64 = Rotl64(v1, 1) + Rotl64(v2, 7) + Rotl64(v3, 12) + Rotl64(v4, 18);
                h64 = MergeRound(h64, v1);
                h64 = MergeRound(h64, v2);
                h64 = MergeRound(h64, v3);
                h64 = MergeRound(h64, v4);
            }
            else
            {
                h64 = state->V3 + Prime64Of5;
            }

            h64 += (ulong)state->TotalLen;

            while (p + 8 <= bEnd)
            {
                h64 ^= Round(0, Read64(p));
                h64 = Rotl64(h64, 27) * Prime64Of1 + Prime64Of4;
                p += 8;
            }

            if (p + 4 <= bEnd)
            {
                h64 ^= Read32(p) * Prime64Of1;
                h64 = Rotl64(h64, 23) * Prime64Of2 + Prime64Of3;
                p += 4;
            }

            while (p < bEnd)
            {
                h64 ^= *p * Prime64Of5;
                h64 = Rotl64(h64, 11) * Prime64Of1;
                p++;
            }

            h64 ^= h64 >> 33;
            h64 *= Prime64Of2;
            h64 ^= h64 >> 29;
            h64 *= Prime64Of3;
            h64 ^= h64 >> 32;

            return h64;
        }

        /// <summary>
        /// Converts this <see cref="XXHash32"/> into a <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="HashAlgorithm"/> representation of this <see cref="XXHash32"/>.
        /// </returns>
        public HashAlgorithm ToHashAlgorithm()
        {
            return new HashAlgorithmAdapter(sizeof(uint), Reset, Update, DigestBytes);
        }

        /// <summary>
        /// Cast a <see cref="XXHash64"/> into a <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="obj">The <see cref="XXHash64"/> object to cast.</param>
        public static explicit operator HashAlgorithm(XXHash64 obj)
        {
            return obj.ToHashAlgorithm();
        }
    }
}