namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// Static class with factory methods to create LZ4 decoders.
    /// </summary>
    internal static class LZ4EncodingFactory
	{
		/// <summary>
        /// Creates a LZ4 decoder using the parameters specified.
        /// </summary>
		/// <param name="chaining">Dependent blocks.</param>
		/// <param name="blockSize">Block size.</param>
		/// <param name="extraBlocks">Number of extra blocks.</param>
		/// <returns>
        /// An instance of LZ4 decoder, using the parameters specified.
        /// </returns>
		public static ILZ4Decoder CreateDecoder(bool chaining, int blockSize, int extraBlocks = 0)
        {
            return !chaining 
                ? CreateBlockDecoder(blockSize) 
                : CreateChainDecoder(blockSize, extraBlocks);
        }

        /// <summary>
        /// Creates a LZ4 encoder using the parameters specified.
        /// </summary>
        /// <param name="chaining">Dependent blocks.</param>
        /// <param name="level">Compression level.</param>
        /// <param name="blockSize">Block size.</param>
        /// <param name="extraBlocks">Number of extra blocks.</param>
		/// <returns>
        /// An instance of LZ4 encoder, using the parameters specified.
        /// </returns>
        public static ILZ4Encoder CreateEncoder(bool chaining, LZ4CompressionLevel level, int blockSize, int extraBlocks = 0)
        {
            return !chaining ? CreateBlockEncoder(level, blockSize) :
                level == LZ4CompressionLevel.Level0 ? CreateFastEncoder(blockSize, extraBlocks) :
                CreateHighEncoder(level, blockSize, extraBlocks);
        }

        private static ILZ4Encoder CreateBlockEncoder(LZ4CompressionLevel level, int blockSize)
        {
            return new LZ4BlockEncoder(level, blockSize);
        }

        private static ILZ4Encoder CreateFastEncoder(int blockSize, int extraBlocks)
        {
            return new LZ4FastChainEncoder(blockSize, extraBlocks);
        }

        private static ILZ4Encoder CreateHighEncoder(LZ4CompressionLevel level, int blockSize, int extraBlocks)
        {
            return new LZ4HighChainEncoder(level, blockSize, extraBlocks);
        }

        private static ILZ4Decoder CreateChainDecoder(int blockSize, int extraBlocks)
        {
            return new LZ4ChainDecoder(blockSize, extraBlocks);
        }

        private static ILZ4Decoder CreateBlockDecoder(int blockSize)
        {
            return new LZ4BlockDecoder(blockSize);
        }
    }
}
