namespace Standard.IO.Compression
{
    /// <summary>
    /// Various compression levels applicable to the LZ4 algorithm. Higher levels offer better compression 
    /// ratios, but at the expense of slower speed.
    /// </summary>
	public enum LZ4CompressionLevel
	{
        /// <summary>
        /// Compress data using the `Fast` mode (level 0).
        /// </summary>
        Level0 = 0,

        /// <summary>
        /// Compress data using `High Compress` mode (level 3).
        /// </summary>
		Level3 = 3,

        /// <summary>
        /// Compress data using `High Compress` mode (level 4).
        /// </summary>
		Level4 = 4,

        /// <summary>
        /// Compress data using `High Compress` mode (level 5).
        /// </summary>
		Level5 = 5,

        /// <summary>
        /// Compress data using `High Compress` mode (level 6).
        /// </summary>
		Level6 = 6,

        /// <summary>
        /// Compress data using `High Compress` mode (level 7).
        /// </summary>
		Level7 = 7,

        /// <summary>
        /// Compress data using `High Compress` mode (level 8).
        /// </summary>
		Level8 = 8,

        /// <summary>
        /// Compress data using `High Compress` mode (level 9).
        /// </summary>
		Level9 = 9,

        /// <summary>
        /// Compress data using `Optimal Compress` mode (level 10).
        /// </summary>
		Level10 = 10,

        /// <summary>
        /// Compress data using `Optimal Compress` mode (level 11).
        /// </summary>
		Level11 = 11,

        /// <summary>
        /// Compress data using `Optimal Compress` mode (level 12).
        /// </summary>
		Level12 = 12,

        /// <summary>
        /// Minimum compression ratio. This compression level compress data at the fastest speed, but smaller compressed size 
        /// can be achieved using a higher compression level.
        /// </summary>
        /// <remarks>
        /// This is an alias to <see cref="Level0"/>.
        /// </remarks>
        Min = Level0,

        /// <summary>
        /// Maximum compression ratio. This compression level offers the smallest compressed size, but takes the longest time.
        /// </summary>
        /// <remarks>
        /// This is an alias to <see cref="Level12"/>.
        /// </remarks>
        Max = Level12,
	}
}
