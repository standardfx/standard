using System;
using System.IO;
using System.Runtime.InteropServices;
using Standard.IO.Compression.LZ4Encoding;
using Standard.IO.Compression.LZ4;

namespace Standard.IO.Compression
{
	/// <summary>
	/// Utility class for performing common LZ4 block compression.
	/// </summary>
	public class LZ4Codec
	{
        private const byte VersionMask = 0x07;
        private const byte CurrentVersion = 0 & VersionMask; // 3 bits

        /// <summary>
        /// Calculates the maximum size after compression.
        /// </summary>
        /// <param name="length">Length of input buffer.</param>
        /// <returns>
        /// Maximum length after compression.
        /// </returns>
        public static int MaximumOutputSize(int length)
        {
            return LZ4Engine.CompressBound(length);
        }

        /// <see cref="Encode(byte[], int, int, byte[], int, int, LZ4CompressionLevel)"/>
        [CLSCompliant(false)]
        public static unsafe int Encode(byte* source, int sourceLength, byte* target, int targetLength, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
		{
			if (sourceLength <= 0)
				return 0;

			int encoded = level == LZ4CompressionLevel.Level0
				? LZ4Engine64.CompressDefault(source, target, sourceLength, targetLength)
				: LZ4Engine64HC.CompressHC(source, target, sourceLength, targetLength, (int)level);

            return encoded <= 0 ? -1 : encoded;
		}

        /// <see cref="Encode(byte[], int, int, byte[], int, int, LZ4CompressionLevel)"/>
        public static unsafe int Encode(ReadOnlySpan<byte> source, Span<byte> target, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
		{
			int sourceLength = source.Length;
			if (sourceLength <= 0)
				return 0;

			int targetLength = target.Length;
			fixed (byte* sourcePtr = &MemoryMarshal.GetReference(source))
			fixed (byte* targetPtr = &MemoryMarshal.GetReference(target))
            {
                return Encode(sourcePtr, sourceLength, targetPtr, targetLength, level);
            }
        }

        /// <summary>
        /// Compresses data from one buffer into another.
        /// </summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceOffset">Input buffer offset.</param>
        /// <param name="sourceLength">Input buffer length.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetOffset">Output buffer offset.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <param name="level">Compression level. Lower compression level offers better speed at the expense of larger compressed size.</param>
        /// <returns>
        /// Number of bytes written, or negative value if output buffer is too small.
        /// </returns>
        public static unsafe int Encode(byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
		{
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (sourceOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceOffset), RS.CannotBeNegativeNumber);

            if (sourceLength < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceLength), RS.CannotBeNegativeNumber);

            if (sourceOffset + sourceLength > source.Length)
                throw new ArgumentException(RS.OffsetSelectionLargerThanLength);

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (targetOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(targetOffset), RS.CannotBeNegativeNumber);

            if (targetLength < 0)
                throw new ArgumentOutOfRangeException(nameof(targetLength), RS.CannotBeNegativeNumber);

            if (targetOffset + targetLength > target.Length)
                throw new ArgumentException(RS.OffsetSelectionLargerThanLength);

			fixed (byte* sourcePtr = source)
			fixed (byte* targetPtr = target)
            {
                return Encode(
                    sourcePtr + sourceOffset, sourceLength,
                    targetPtr + targetOffset, targetLength,
                    level);
            }
        }

        /// <see cref="Decode(byte[], int, int, byte[], int, int)"/>
        [CLSCompliant(false)]
		public static unsafe int Decode(byte* source, int sourceLength, byte* target, int targetLength)
		{
			if (sourceLength <= 0)
				return 0;

			int decoded = LZ4Engine.DecompressSafe(source, target, sourceLength, targetLength);
			return decoded <= 0 ? -1 : decoded;
		}

        /// <see cref="Decode(byte[], int, int, byte[], int, int)"/>
		public static unsafe int Decode(ReadOnlySpan<byte> source, Span<byte> target)
		{
			int sourceLength = source.Length;
			if (sourceLength <= 0)
				return 0;

			int targetLength = target.Length;
			fixed (byte* sourcePtr = &MemoryMarshal.GetReference(source))
			fixed (byte* targetPtr = &MemoryMarshal.GetReference(target))
            {
                return Decode(sourcePtr, sourceLength, targetPtr, targetLength);
            }
        }

		/// <summary>
        /// Decompresses data from the buffer specified.
        /// </summary>
		/// <param name="source">Input buffer.</param>
		/// <param name="sourceOffset">Input buffer offset.</param>
		/// <param name="sourceLength">Input buffer length.</param>
		/// <param name="target">Output buffer.</param>
		/// <param name="targetOffset">Output buffer offset.</param>
		/// <param name="targetLength">Output buffer length.</param>
		/// <returns>
        /// Number of bytes written, or negative value if output buffer is too small.
        /// </returns>
		public static unsafe int Decode(byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength)
		{
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (sourceOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceOffset), RS.CannotBeNegativeNumber);

            if (sourceLength < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceLength), RS.CannotBeNegativeNumber);

            if (sourceOffset + sourceLength > source.Length)
                throw new ArgumentException(RS.OffsetSelectionLargerThanLength);

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (targetOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(targetOffset), RS.CannotBeNegativeNumber);

            if (targetLength < 0)
                throw new ArgumentOutOfRangeException(nameof(targetLength), RS.CannotBeNegativeNumber);

            if (targetOffset + targetLength > target.Length)
                throw new ArgumentException(RS.TargetOffsetSelectionLargerThanLength);

			fixed (byte* sourcePtr = source)
			fixed (byte* targetPtr = target)
            {
                return Decode(
                    sourcePtr + sourceOffset, sourceLength,
                    targetPtr + targetOffset, targetLength);
            }
        }

        // -------------------------------------------------------------------------------

        private static unsafe byte[] PickleV0(byte* target, int targetLength, int sourceLength)
        {
            int diff = sourceLength - targetLength;
            int llen = diff == 0 ? 0 
                : diff < 0x100 ? 1 
                : diff < 0x10000 ? 2 
                : 4;
            byte[] result = new byte[targetLength + 1 + llen];

            fixed (byte* resultPtr = result)
            {
                int llenFlags = llen == 4 ? 3 : llen; // 2 bits
                byte flags = (byte)((llenFlags << 6) | CurrentVersion);
                LZ4MemoryHelper.Poke8(resultPtr + 0, flags);

                if (llen == 1)
                    LZ4MemoryHelper.Poke8(resultPtr + 1, (byte)diff);
                else if (llen == 2)
                    LZ4MemoryHelper.Poke16(resultPtr + 1, (ushort)diff);
                else if (llen == 4)
                    LZ4MemoryHelper.Poke32(resultPtr + 1, (uint)diff);

                LZ4MemoryHelper.Move(resultPtr + llen + 1, target, targetLength);
            }

            return result;
        }

        private static unsafe byte[] UnpickleV0(byte flags, byte* source, int sourceLength)
        {
            int llen = (flags >> 6) & 0x03; // 2 bits
            if (llen == 3)
                llen = 4;

            if (sourceLength < llen)
                throw new InvalidDataException(RS.SourceBufferTooSmall);

            int diff = (int)(llen == 0 ? 0 :
                llen == 1 ? *source :
                llen == 2 ? *(ushort*)source :
                llen == 4 ? *(uint*)source :
                throw new InvalidDataException(RS.InvalidLengthDescriptor));

            source += llen;
            sourceLength -= llen;
            int targetLength = sourceLength + diff;

            byte[] target = new byte[targetLength];
            fixed (byte* targetPtr = target)
            {
                if (diff == 0)
                {
                    Buffer.MemoryCopy(source, targetPtr, targetLength, targetLength);
                }
                else
                {
                    int decodedLength = Decode(source, sourceLength, targetPtr, targetLength);
                    if (decodedLength != targetLength)
                        throw new InvalidDataException(string.Format(RS.UnexpectedDecodeSize, targetLength, decodedLength));
                }
            }

            return target;
        }

        /// <see cref="Compress(byte[], int, int, LZ4CompressionLevel)"/>
        public static byte[] Compress(byte[] source, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
        {
            return Compress(source, 0, source.Length, level);
        }

        /// <summary>
        /// Compresses the input buffer.
        /// </summary>
        /// <param name="source">The input buffer containing data to be compressed.</param>
        /// <param name="sourceOffset">Input buffer offset within <paramref name="source"/>. Data before this index position will be ignored.</param>
        /// <param name="sourceLength">Input buffer length from <paramref name="sourceOffset"/>.</param>
        /// <param name="level">Compression level. Lower compression level offers better speed at the expense of larger compressed size.</param>
        /// <returns>
        /// The compressed output buffer.
        /// </returns>
        public static unsafe byte[] Compress(byte[] source, int sourceOffset, int sourceLength, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (sourceOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceOffset), RS.CannotBeNegativeNumber);

            if (sourceLength < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceLength), RS.CannotBeNegativeNumber);

            if (sourceOffset + sourceLength > source.Length)
                throw new ArgumentException(RS.OffsetSelectionLargerThanLength);

            fixed (byte* sourcePtr = source)
            {
                return Compress(sourcePtr + sourceOffset, sourceLength, level);
            }
        }

        /// <see cref="Compress(byte[], int, int, LZ4CompressionLevel)"/>
        public static unsafe byte[] Compress(ReadOnlySpan<byte> source, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
        {
            int sourceLength = source.Length;
            if (sourceLength <= 0)
                return Array.Empty<byte>();

            fixed (byte* sourcePtr = &MemoryMarshal.GetReference(source))
            {
                return Compress(sourcePtr, sourceLength, level);
            }
        }

        /// <see cref="Compress(byte[], int, int, LZ4CompressionLevel)"/>
        [CLSCompliant(false)]
        public static unsafe byte[] Compress(byte* source, int sourceLength, LZ4CompressionLevel level = LZ4CompressionLevel.Level0)
        {
            if (sourceLength <= 0)
                return Array.Empty<byte>();

            int targetLength = sourceLength - 1;
            byte* target = (byte*)LZ4MemoryHelper.Alloc(sourceLength);
            try
            {
                int encodedLength = Encode(source, sourceLength, target, targetLength, level);

                return encodedLength <= 0
                    ? PickleV0(source, sourceLength, sourceLength)
                    : PickleV0(target, encodedLength, sourceLength);
            }
            finally
            {
                LZ4MemoryHelper.Free(target);
            }
        }

        /// <see cref="Expand(byte[], int, int)"/>
        public static byte[] Expand(byte[] source)
        {
            return Expand(source, 0, source.Length);
        }

        /// <summary>
        /// Decompress data that were compressed using the LZ4 algorithm.
        /// </summary>
        /// <param name="source">The input buffer containing compressed data.</param>
        /// <param name="sourceOffset">Input buffer offset within <paramref name="source"/>. Data before this index position will be ignored.</param>
        /// <param name="sourceLength">Input buffer length from <paramref name="sourceOffset"/>.</param>
        /// <returns>
        /// The output buffer containing decompressed data.
        /// </returns>
        public static unsafe byte[] Expand(byte[] source, int sourceOffset, int sourceLength)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (sourceOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceOffset), RS.CannotBeNegativeNumber);

            if (sourceLength < 0)
                throw new ArgumentOutOfRangeException(nameof(sourceLength), RS.CannotBeNegativeNumber);

            if (sourceOffset + sourceLength > source.Length)
                throw new ArgumentException(RS.OffsetSelectionLargerThanLength);

            if (sourceLength <= 0)
                return Array.Empty<byte>();

            fixed (byte* sourcePtr = source)
            {
                return Expand(sourcePtr + sourceOffset, sourceLength);
            }
        }

        /// <see cref="Expand(byte[], int, int)"/>
        public static unsafe byte[] Expand(ReadOnlySpan<byte> source)
        {
            int sourceLength = source.Length;
            if (sourceLength <= 0)
                return Array.Empty<byte>();

            fixed (byte* sourcePtr = &MemoryMarshal.GetReference(source))
            {
                return Expand(sourcePtr, source.Length);
            }
        }

        /// <see cref="Expand(byte[], int, int)"/>
        [CLSCompliant(false)]
        public static unsafe byte[] Expand(byte* source, int sourceLength)
        {
            if (sourceLength <= 0)
                return Array.Empty<byte>();

            byte flags = *source;
            int version = flags & VersionMask; // 3 bits

            if (version == 0)
                return UnpickleV0(flags, source + 1, sourceLength - 1);

            throw new InvalidDataException(string.Format(RS.LZ4CompressVersionNotSupported, version));
        }
    }
}
