namespace Standard.IO.Compression
{
	/// <summary>
	/// LZ4 frame descriptor.
	/// </summary>
	internal class LZ4FrameDescriptor : ILZ4FrameDescriptor
	{
        /// <summary>
        /// Creates s new instance of the <see cref="LZ4FrameDescriptor"/> class.
        /// </summary>
        /// <param name="contentLength">Content length.</param>
        /// <param name="contentChecksum">Content checksum flag.</param>
        /// <param name="chaining">Chaining flag.</param>
        /// <param name="blockChecksum">Block checksum flag.</param>
        /// <param name="dictionary">Dictionary identifier.</param>
        /// <param name="blockSize">Block size.</param>
        public LZ4FrameDescriptor(long? contentLength, bool contentChecksum, bool chaining, bool blockChecksum, uint? dictionary, int blockSize)
        {
            ContentLength = contentLength;
            ContentChecksum = contentChecksum;
            Chaining = chaining;
            BlockChecksum = blockChecksum;
            Dictionary = dictionary;
            BlockSize = blockSize;
        }

        /// <summary>
        /// Content length (if available).
        /// </summary>
        public long? ContentLength { get; }

		/// <see cref="ILZ4FrameDescriptor.ContentChecksum"/>
		public bool ContentChecksum { get; }

        /// <see cref="ILZ4FrameDescriptor.Chaining"/>
        public bool Chaining { get; }

        /// <see cref="ILZ4FrameDescriptor.BlockChecksum"/>
		public bool BlockChecksum { get; }

        /// <see cref="ILZ4FrameDescriptor.Dictionary"/>
		public uint? Dictionary { get; }

        /// <see cref="ILZ4FrameDescriptor.BlockSize"/>
		public int BlockSize { get; }
	}
}
