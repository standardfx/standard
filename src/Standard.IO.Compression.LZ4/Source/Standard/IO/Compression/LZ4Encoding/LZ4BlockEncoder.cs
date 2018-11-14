namespace Standard.IO.Compression.LZ4Encoding
{
    /// <summary>
    /// Independent block encoder. Produces larger files but uses less memory and
    /// gives better performance.
    /// </summary>
    internal unsafe class LZ4BlockEncoder : LZ4Encoder
	{
		private readonly LZ4CompressionLevel _level;

		/// <summary>
        /// Creates new instance of the <see cref="LZ4BlockEncoder"/> class.
        /// </summary>
		/// <param name="level">Compression level.</param>
		/// <param name="blockSize">Block size.</param>
		public LZ4BlockEncoder(LZ4CompressionLevel level, int blockSize)
            : base(false, blockSize, 0)
        {
            _level = level;
        }

        /// <see cref="LZ4Encoder.EncodeBlock(byte*, int, byte*, int)"/>
        protected override int EncodeBlock(byte* source, int sourceLength, byte* target, int targetLength)
        {
            return LZ4Codec.Encode(source, sourceLength, target, targetLength, _level);
        }

        /// <see cref="LZ4Encoder.CopyDict(byte*, int)"/>
        protected override int CopyDict(byte* target, int dictionaryLength)
        {
            return 0;
        }
	}
}
