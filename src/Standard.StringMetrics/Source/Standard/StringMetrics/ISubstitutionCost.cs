namespace Standard.StringMetrics
{
    /// <summary>
    /// Interface for substitution cost implements.
    /// </summary>
    public interface ISubstitutionCost
    {
        /// <summary>
        /// Returns the cost of substituting a word with another word.
        /// </summary>
        /// <param name="firstWord">The string which may be substituted by <paramref name="secondWord"/>.</param>
        /// <param name="firstWordIndex">The index position of <paramref name="firstWord"/>.</param>
        /// <param name="secondWord">The string to substitute <paramref name="firstWord"/>.</param>
        /// <param name="secondWordIndex">The index position of <paramref name="secondWord"/>.</param>
        /// <returns>The cost of substituting <paramref name="firstWord"/> with <paramref name="secondWord"/>.</returns>
        double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex);

        /// <summary>
        /// The maximum substitution cost value.
        /// </summary>
        double MaxCost { get; }

        /// <summary>
        /// The minimum substitution cost value.
        /// </summary>
        double MinCost { get; }
    }
}