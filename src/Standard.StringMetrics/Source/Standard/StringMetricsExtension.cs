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
        /// Compares two strings for similarity. The returned result indicates the level of similarity between the two string.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <param name="algorithm">Specify the algorithm to use for comparing <paramref name="firstWord"/> and <paramref name="secondWord"/>. The default algorithm is <see cref="SimMetricAlgorithm.Levenstein"/>.</param>
        /// <returns>A number indicating how similar the strings are. A larger number indicates a higher degree of similarity.</returns>
        /// <remarks><![CDATA[
        /// The following code demonstrates how to filter a list of strings based on the degree of similarity:
        /// ```C#
        /// const int threshold = 0.7;
        /// string word = "fooler"
        /// var list = new List<string>() { "fowler", "fish", "crawler" };
        /// var filtered = List<string>();
        /// foreach (string item in list)
        /// {
        ///     double num = item.NearEquals(word);
        ///     if ((1 - num) <= threshold)
        ///         filtered.Add(item);
        /// }
        /// Console.WriteLine("You typed '{0}'. Did you mean: {1}", word, filtered);
        /// ```
        /// ]]></remarks>
        public static double NearEquals(this string firstWord, string secondWord, SimMetricAlgorithm algorithm = SimMetricAlgorithm.Levenstein)
        {
            AbstractStringMetric sim;

            switch (algorithm)
            {
                case SimMetricAlgorithm.BlockDistance:
                    sim = new BlockDistance();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.ChapmanLengthDeviation:
                    sim = new ChapmanLengthDeviation();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.CosineSimilarity:
                    sim = new CosineSimilarity();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.DiceSimilarity:
                    sim = new DiceSimilarity();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.EuclideanDistance:
                    sim = new EuclideanDistance();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.JaccardSimilarity:
                    sim = new JaccardSimilarity();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.Jaro:
                    sim = new Jaro();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.JaroWinkler:
                    sim = new JaroWinkler();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.MatchingCoefficient:
                    sim = new MatchingCoefficient();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.MongeElkan:
                    sim = new MongeElkan();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.NeedlemanWunch:
                    sim = new NeedlemanWunch();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.OverlapCoefficient:
                    sim = new OverlapCoefficient();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.QGramsDistance:
                    sim = new QGramsDistance();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.SmithWaterman:
                    sim = new SmithWaterman();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.SmithWatermanGotoh:
                    sim = new SmithWatermanGotoh();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.SmithWatermanGotohWindowedAffine:
                    sim = new SmithWatermanGotohWindowedAffine();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.ChapmanMeanLength:
                    sim = new ChapmanMeanLength();
                    return sim.GetSimilarity(firstWord, secondWord);
                default:
                    sim = new Levenstein();
                    return sim.GetSimilarity(firstWord, secondWord);
            }
        }
    }
}
