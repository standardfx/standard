// high encoder context
using LZ4Context = Standard.IO.Compression.LZ4Engine64HC.CCtxT;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// LZ4 encoder using dependent blocks with high compression.
    /// </summary>
    internal unsafe class LZ4HighChainEncoder : LZ4Encoder
	{
		private readonly LZ4Context* _context;

		/// <summary>
        /// Creates a new instance of the <see cref="LZ4HighChainEncoder"/> class.
        /// </summary>
		/// <param name="level">Compression level.</param>
		/// <param name="blockSize">Block size.</param>
		/// <param name="extraBlocks">Number of extra blocks.</param>
		public LZ4HighChainEncoder(LZ4CompressionLevel level, int blockSize, int extraBlocks = 0)
            : base(true, blockSize, extraBlocks)
		{
			if (level < LZ4CompressionLevel.Level3)
                level = LZ4CompressionLevel.Level3;

			if (level > LZ4CompressionLevel.Level12)
                level = LZ4CompressionLevel.Level12;

			_context = (LZ4Context*) LZ4MemoryHelper.AllocZero(sizeof(LZ4Context));
			LZ4Engine64HC.ResetStreamHC(_context, (int) level);
		}

		/// <see cref="UnmanagedEncodingResource.ReleaseUnmanaged()"/>
		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			LZ4MemoryHelper.Free(_context);
		}

		/// <see cref="LZ4Encoder.EncodeBlock(byte*, int, byte*, int)"/>
		protected override int EncodeBlock(byte* source, int sourceLength, byte* target, int targetLength)
        {
            return LZ4Engine64HC.CompressHCContinue(_context, source, target, sourceLength, targetLength);
        }

        /// <see cref="LZ4Encoder.CopyDict(byte*, int)"/>
        protected override int CopyDict(byte* target, int length)
        {
            return LZ4Engine64HC.SaveDictHC(_context, target, length);
        }
    }
}
