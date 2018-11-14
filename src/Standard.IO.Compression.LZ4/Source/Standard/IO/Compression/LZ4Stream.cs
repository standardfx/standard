using System;
using System.IO;
using Standard.IO.Compression.LZ4Encoding;

namespace Standard.IO.Compression
{
	/// <summary>
	/// Utility class with factory methods to create LZ4 compression and decompression streams.
	/// </summary>
	public static class LZ4Stream
	{
		/// <summary>
        /// Creates a compression stream on top of the <see cref="Stream"/> specified.
        /// </summary>
		/// <param name="stream">Inner stream.</param>
		/// <param name="settings">Compression settings.</param>
		/// <param name="leaveOpen">Leave inner stream open after disposing.</param>
		/// <returns>
        /// The compression stream.
        /// </returns>
		public static LZ4EncoderStream Encode(Stream stream, LZ4EncoderSettings settings = null, bool leaveOpen = false)
		{
			settings = settings ?? LZ4EncoderSettings.Default;

            LZ4FrameDescriptor frameInfo = new LZ4FrameDescriptor(
				settings.ContentLength,
				settings.ContentChecksum,
				settings.ChainBlocks,
				settings.BlockChecksum,
				settings.Dictionary,
				settings.BlockSize);

			LZ4CompressionLevel level = settings.CompressionLevel;
			int extraMemory = settings.ExtraMemory;

			return new LZ4EncoderStream(
				stream,
				frameInfo,
				i => LZ4EncodingFactory.CreateEncoder(i.Chaining, level, i.BlockSize, ExtraBlocks(i.BlockSize, extraMemory)),
				leaveOpen);
		}

        /// <summary>
        /// Creates a compression stream on top of the <see cref="Stream"/> specified.
        /// </summary>
        /// <param name="stream">Inner stream.</param>
        /// <param name="level">Compression level.</param>
        /// <param name="extraMemory">Extra memory used for compression.</param>
        /// <param name="leaveOpen">Leave inner stream open after disposing.</param>
		/// <returns>
        /// The compression stream.
        /// </returns>
        public static LZ4EncoderStream Encode(Stream stream, LZ4CompressionLevel level, int extraMemory = 0, bool leaveOpen = false)
		{
            LZ4EncoderSettings settings = new LZ4EncoderSettings
                {
				    ChainBlocks = true,
				    ExtraMemory = extraMemory,
				    BlockSize = LZ4MemoryHelper.K64,
				    CompressionLevel = level
			    };

            return Encode(stream, settings, leaveOpen);
		}

		/// <summary>
        /// Creates a decompression stream on top of the <see cref="Stream"/> specified.
        /// </summary>
		/// <param name="stream">Inner stream.</param>
		/// <param name="settings">Decompression settings.</param>
		/// <param name="leaveOpen">Leave inner stream open after disposing.</param>
		/// <returns>
        /// The decompression stream.
        /// </returns>
		public static LZ4DecoderStream Decode(Stream stream, LZ4DecoderSettings settings = null, bool leaveOpen = false)
		{
			settings = settings ?? LZ4DecoderSettings.Default;
			int extraMemory = settings.ExtraMemory;

            return new LZ4DecoderStream(
				stream,
				i => LZ4EncodingFactory.CreateDecoder(i.Chaining, i.BlockSize, ExtraBlocks(i.BlockSize, extraMemory)),
				leaveOpen);
		}

        /// <summary>
        /// Creates a decompression stream on top of the <see cref="Stream"/> specified.
        /// </summary>
        /// <param name="stream">Inner stream.</param>
        /// <param name="extraMemory">Extra memory used for decompression.</param>
        /// <param name="leaveOpen">Leave inner stream open after disposing.</param>
		/// <returns>
        /// The decompression stream.
        /// </returns>
        public static LZ4DecoderStream Decode(Stream stream, int extraMemory, bool leaveOpen = false)
		{
            LZ4DecoderSettings settings = new LZ4DecoderSettings { ExtraMemory = extraMemory };
			return Decode(stream, settings, leaveOpen);
		}

		private static int ExtraBlocks(int blockSize, int extraMemory)
        {
            return Math.Max(extraMemory > 0 ? blockSize : 0, extraMemory) / blockSize;
        }
    }
}
