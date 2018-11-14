using System;
using Standard.IO.Compression.LZ4Encoding;

namespace Standard.IO.Compression
{
    /// <summary>
    /// LZ4 encoder settings.
    /// </summary>
    public class LZ4EncoderSettings
    {
        internal static LZ4EncoderSettings Default = new LZ4EncoderSettings();

        /// <summary>
        /// Content length. It is not enforced, and can be set to any value, but it will be
        /// written to the stream so it can be used while decoding. If you don't know the length
        /// just leave default value.
        /// </summary>
        public long? ContentLength { get; set; } = null;

        /// <summary>
        /// Indicates whether blocks should be chained (dependent) or not (independent). Dependent blocks
        /// (with chaining) provide better compression ratio but are a little but slower and take
        /// more memory. 
        /// </summary>
        public bool ChainBlocks { get; set; } = true;

        /// <summary>
        /// Block size. You can use any block size, but default values for LZ4 are `64k`, `256k`, `1m`,
        /// and `4m`. `64k` is good enough for dependent blocks, but for independent blocks bigger is
        /// better. 
        /// </summary>
        public int BlockSize { get; set; } = LZ4MemoryHelper.K64;

        /// <summary>
        /// Indicates whether content checksum is provided.
        /// </summary>
        /// <remarks>
        /// Content checksum is not available in this version.
        /// </remarks>
        public bool ContentChecksum
        {
            get { return false; }
        }

        /// <summary>
        /// Indicates whether block checksum is provided.
        /// </summary>
        /// <remarks>
        /// Block checksum is not available in this version.
        /// </remarks>
        public bool BlockChecksum
        {
            get { return false; }
        }

        /// <summary>
        /// Dictionary identifier.
        /// </summary>
        /// <remarks>
        /// Dictionary identifier is not available in this version.
        /// </remarks>
        [CLSCompliant(false)]
        public uint? Dictionary
        {
            get { return null; }
        }
		
		/// <summary>
        /// Compression level.
        /// </summary>
		public LZ4CompressionLevel CompressionLevel { get; set; } = LZ4CompressionLevel.Level0;
		
		/// <summary>
        /// Extra memory for the process. More is usually better.
        /// </summary>
		public int ExtraMemory { get; set; }
	}
}
