namespace Standard.IO.Compression
{
	/// <summary>
	/// Interface for LZ4 frame descriptor.
	/// </summary>
	internal interface ILZ4FrameDescriptor
	{
		/// <summary>
        /// Content length. This value is not always known.
        /// </summary>
		long? ContentLength { get; }

		/// <summary>
        /// Indicates whether content checksum is provided.
        /// </summary>
		bool ContentChecksum { get; }

		/// <summary>
        /// Indicates whether blocks are chained (dependent) or not (independent).
        /// </summary>
		bool Chaining { get; }

		/// <summary>
        /// Indicates whether block checksums are provided.
        /// </summary>
		bool BlockChecksum { get; }

		/// <summary>
        /// Dictionary identifier. This value may be `null`.
        /// </summary>
		uint? Dictionary { get; }

		/// <summary>
        /// Block size.
        /// </summary>
		int BlockSize { get; }
	}
}
