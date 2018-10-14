namespace Standard.StringMetrics
{
    /// <summary>
    /// Interface for string metric implements.
    /// </summary>
    public interface IStringMetric
    {
        /// <summary>
        /// Returns a number which indicates the degree of similarity between two strings.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <returns>A number indicating the degree of similarity between <paramref name="firstWord"/> and <paramref name="secondWord"/>. A large number means a high degree of similarity.</returns>
        double GetSimilarity(string firstWord, string secondWord);

        /// <summary>
        /// Explains the result from <see cref="GetSimilarity(string, string)"/>.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <returns>A string which explains the result from <see cref="GetSimilarity(string, string)"/>.</returns>
        string GetSimilarityExplained(string firstWord, string secondWord);

        /// <summary>
        /// Calculates the time taken for comparing string similarity.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <returns>The actual time taken to compare <paramref name="firstWord"/> and <paramref name="secondWord"/>.</returns>
        long GetSimilarityTimingActual(string firstWord, string secondWord);

        /// <summary>
        /// Estimates the time required to compare two strings.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <returns>The estimated time required to compare <paramref name="firstWord"/> and <paramref name="secondWord"/>.</returns>
        double GetSimilarityTimingEstimated(string firstWord, string secondWord);

        /// <summary>
        /// Returns a number which indicates the degree of similarity between two strings before normalization.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <returns>A number indicating the degree of similarity between <paramref name="firstWord"/> and <paramref name="secondWord"/> before normalization. A large number means a high degree of similarity.</returns>
        double GetUnnormalizedSimilarity(string firstWord, string secondWord);
    }
}
