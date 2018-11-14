using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Standard.Security.Cryptography
{
    /// <summary>
    /// A 32-bit implementation of the XXHash algorithm.
    /// </summary>
    public unsafe partial class XXHash32 : XXHash
    {
        private const uint Prime32Of1 = 2654435761u;
        private const uint Prime32Of2 = 2246822519u;
        private const uint Prime32Of3 = 3266489917u;
        private const uint Prime32Of4 = 668265263u;
        private const uint Prime32Of5 = 374761393u;

        [StructLayout(LayoutKind.Sequential)]
        private struct State
        {
            public uint TotalLen32;
            public bool LargeLen;
            public uint V1;
            public uint V2;
            public uint V3;
            public uint V4;
            public fixed uint Mem32[4];
            public uint MemSize;
        }

        private State _state;
        
        /// <summary>
        /// Hash value of an empty buffer.
        /// </summary>
        [CLSCompliant(false)]
        public const uint EmptyHash = 46947589;

        /// <summary>
        /// Creates a new instance of the <see cref="XXHash32"/> class.
        /// </summary>
        public XXHash32()
        {
            Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Rotl(uint x, int r)
        {
            return (x << r) | (x >> (32 - r));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Round(uint seed, uint input)
        {
            return Rotl(seed + input * Prime32Of2, 13) * Prime32Of1;
        }

        /// <see cref="DigestOf(byte[], int, int)"/>
        [CLSCompliant(false)]
        public static unsafe uint DigestOf(void* bytes, int length)
        {
            return HashInternal(bytes, length, 0);
        }

        /// <see cref="DigestOf(byte[], int, int)"/>
        [CLSCompliant(false)]
        public static unsafe uint DigestOf(ReadOnlySpan<byte> bytes)
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
        public static unsafe uint DigestOf(byte[] bytes, int offset, int length)
        {
            Validate(bytes, offset, length);

            fixed (byte* bytes0 = bytes)
            {
                return DigestOf(bytes0 + offset, length);
            }
        }

        private static uint HashInternal(void* input, int len, uint seed)
        {
            byte* p = (byte*)input;
            byte* bEnd = p + len;
            uint h32;

            if (len >= 16)
            {
                byte* limit = bEnd - 16;
                uint v1 = seed + Prime32Of1 + Prime32Of2;
                uint v2 = seed + Prime32Of2;
                uint v3 = seed + 0;
                uint v4 = seed - Prime32Of1;

                do
                {
                    v1 = Round(v1, Read32(p + 0));
                    v2 = Round(v2, Read32(p + 4));
                    v3 = Round(v3, Read32(p + 8));
                    v4 = Round(v4, Read32(p + 12));
                    p += 16;
                }
                while (p <= limit);

                h32 = Rotl(v1, 1) + Rotl(v2, 7) + Rotl(v3, 12) + Rotl(v4, 18);
            }
            else
            {
                h32 = seed + Prime32Of5;
            }

            h32 += (uint)len;

            while (p + 4 <= bEnd)
            {
                h32 = Rotl(h32 + Read32(p) * Prime32Of3, 17) * Prime32Of4;
                p += 4;
            }

            while (p < bEnd)
            {
                h32 = Rotl(h32 + *p * Prime32Of5, 11) * Prime32Of1;
                p++;
            }

            h32 ^= h32 >> 15;
            h32 *= Prime32Of2;
            h32 ^= h32 >> 13;
            h32 *= Prime32Of3;
            h32 ^= h32 >> 16;

            return h32;
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

        private static void ResetInternal(State* state, uint seed)
        {
            Zero(state, sizeof(State));
            state->V1 = seed + Prime32Of1 + Prime32Of2;
            state->V2 = seed + Prime32Of2;
            state->V3 = seed + 0;
            state->V4 = seed - Prime32Of1;
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

            state->TotalLen32 += (uint)len;
            state->LargeLen |= len >= 16 || state->TotalLen32 >= 16;

            if (state->MemSize + len < 16)
            {
                // fill in tmp buffer
                Copy((byte*)state->Mem32 + state->MemSize, input, len);
                state->MemSize += (uint)len;
                return;
            }

            if (state->MemSize > 0)
            {
                // some data left from previous update
                Copy((byte*)state->Mem32 + state->MemSize, input, (int)(16 - state->MemSize));
                uint* p32 = state->Mem32;
                state->V1 = Round(state->V1, Read32(p32 + 0));
                state->V2 = Round(state->V2, Read32(p32 + 1));
                state->V3 = Round(state->V3, Read32(p32 + 2));
                state->V4 = Round(state->V4, Read32(p32 + 3));
                p += 16 - state->MemSize;
                state->MemSize = 0;
            }

            if (p <= bEnd - 16)
            {
                byte* limit = bEnd - 16;
                uint v1 = state->V1;
                uint v2 = state->V2;
                uint v3 = state->V3;
                uint v4 = state->V4;

                do
                {
                    v1 = Round(v1, Read32(p + 0));
                    v2 = Round(v2, Read32(p + 4));
                    v3 = Round(v3, Read32(p + 8));
                    v4 = Round(v4, Read32(p + 12));
                    p += 16;
                }
                while (p <= limit);

                state->V1 = v1;
                state->V2 = v2;
                state->V3 = v3;
                state->V4 = v4;
            }

            if (p < bEnd)
            {
                Copy(state->Mem32, p, (int)(bEnd - p));
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
        public unsafe uint Digest()
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

        private static uint DigestInternal(State* state)
        {
            byte* p = (byte*)state->Mem32;
            byte* bEnd = (byte*)state->Mem32 + state->MemSize;
            uint h32;

            if (state->LargeLen)
            {
                h32 = Rotl(state->V1, 1) + 
                    Rotl(state->V2, 7) + 
                    Rotl(state->V3, 12) + 
                    Rotl(state->V4, 18);
            }
            else
            {
                h32 = state->V3 + Prime32Of5;
            }

            h32 += state->TotalLen32;

            while (p + 4 <= bEnd)
            {
                h32 += Read32(p) * Prime32Of3;
                h32 = Rotl(h32, 17) * Prime32Of4;
                p += 4;
            }

            while (p < bEnd)
            {
                h32 += (*p) * Prime32Of5;
                h32 = Rotl(h32, 11) * Prime32Of1;
                p++;
            }

            h32 ^= h32 >> 15;
            h32 *= Prime32Of2;
            h32 ^= h32 >> 13;
            h32 *= Prime32Of3;
            h32 ^= h32 >> 16;

            return h32;
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
        /// Cast a <see cref="XXHash32"/> into a <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="obj">The <see cref="XXHash32"/> object to cast.</param>
        public static explicit operator HashAlgorithm(XXHash32 obj)
        {
            return obj.ToHashAlgorithm();
        }
    }
}