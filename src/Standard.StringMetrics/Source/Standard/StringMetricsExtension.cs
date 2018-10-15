using System.Collections.Generic;
using Standard.StringMetrics;

namespace Standard
{
    /// <summary>
    /// Extension methods for comparing string similarity.
    /// </summary>
    public static class StringMetricsExtension
    {
        /// <summary>
        /// Compares two strings for similarity, using <see cref="SimMetricAlgorithm.Levenstein"/> algorithm and a threshold value of 0.7.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        public static bool NearEquals(this string firstWord, string secondWord)
        {
            return NearEquals(firstWord, secondWord, 0.7, SimMetricAlgorithm.Levenstein);
        }

        /// <summary>
        /// Compares two strings for similarity, using <see cref="SimMetricAlgorithm.Levenstein"/> algorithm.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <param name="threshold">Any similarity score at or above this value will be considered similar.</param>
        /// <returns>`true` if the similarity score is equal or above <paramref name="threshold"/>. Otherwise, `false`.</returns>
        public static bool NearEquals(this string firstWord, string secondWord, double threshold)
        {
            return NearEquals(firstWord, secondWord, threshold, SimMetricAlgorithm.Levenstein);
        }

        /// <summary>
        /// Compares two strings for similarity.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <param name="threshold">Any similarity score at or above this value will be considered similar.</param>
        /// <param name="algorithm">Specify the algorithm to use for comparing <paramref name="firstWord"/> and <paramref name="secondWord"/>. The default 
        /// algorithm is <see cref="SimMetricAlgorithm.Levenstein"/>.</param>
        /// <returns>`true` if the similarity score is equal or above <paramref name="threshold"/>. Otherwise, `false`.</returns>
        /// <remarks><![CDATA[
        /// The following code demonstrates how to filter a list of strings based on the degree of similarity:
        /// ```C#
        /// string word = "fooler"
        /// var list = new List<string>() { "fowler", "fish", "crawler" };
        /// var filtered = List<string>();
        /// foreach (string item in list)
        /// {
        ///     if (item.NearEquals(word))
        ///         filtered.Add(item);
        /// }
        /// Console.WriteLine("You typed '{0}'. Did you mean: {1}", word, filtered);
        /// ```
        /// ]]></remarks>
        public static bool NearEquals(this string firstWord, string secondWord, double threshold, SimMetricAlgorithm algorithm)
        {
            IStringMetric sim = StringMetricFactory.FromAlgorithm(algorithm);
            double diff = sim.GetSimilarity(firstWord, secondWord);
            return (diff < threshold) ? true : false;
        }
    }
}
