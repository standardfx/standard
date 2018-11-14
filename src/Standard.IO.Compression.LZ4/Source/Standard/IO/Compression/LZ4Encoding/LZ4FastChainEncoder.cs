// fast encoder context
using LZ4Context = Standard.IO.Compression.LZ4Engine.StreamT;

namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// LZ4 encoder using dependent blocks with fast compression.
    /// </summary>
    internal unsafe class LZ4FastChainEncoder: LZ4Encoder
	{
		private readonly LZ4Context* _context;

		/// <summary>
        /// Creates a new instance of the <see cref="LZ4FastChainEncoder"/> class.
        /// </summary>
		/// <param name="blockSize">Block size.</param>
		/// <param name="extraBlocks">Number of extra blocks.</param>
		public LZ4FastChainEncoder(int blockSize, int extraBlocks = 0)
            : base(true, blockSize, extraBlocks)
		{
			_context = (LZ4Context*) LZ4MemoryHelper.AllocZero(sizeof(LZ4Context));
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
            return LZ4Engine64.CompressFastContinue(_context, source, target, sourceLength, targetLength, 1);
        }

        /// <see cref="LZ4Encoder.CopyDict(byte*, int)"/>
        protected override int CopyDict(byte* target, int length)
        {
            return LZ4Engine.SaveDict(_context, target, length);
        }
    }
}
