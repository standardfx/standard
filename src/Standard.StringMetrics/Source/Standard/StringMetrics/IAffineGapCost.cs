namespace Standard.StringMetrics
{
    /// <summary>
    /// Interface for affinity cost implements.
    /// </summary>
    public interface IAffineGapCost
    {
        /// <summary>
        /// Returns the affinity cost of a gap.
        /// </summary>
        /// <param name="textToGap">The string to gap.</param>
        /// <param name="stringIndexStartGap">The start index position of the gap.</param>
        /// <param name="stringIndexEndGap">The end index position of the gap.</param>
        /// <returns>A number indicating the affinity gap.</returns>
        double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap);

        /// <summary>
        /// The maximum affinity cost that may be returned.
        /// </summary>
        double MaxCost { get; }

        /// <summary>
        /// The minimum affinity cost that mat be returned.
        /// </summary>
        double MinCost { get; }
    }
}
