using System;

namespace Standard.StringMetrics
{
    /// <summary>
    /// The Chapman Mean Length algorithm provides a similarity measure between two strings from size of the mean length of the vectors. This approach is supposed to be used to determine which metrics may be best to apply rather than giveing a valid response itself.
    /// </summary>
    public sealed class ChapmanMeanLength : AbstractStringMetric
    {
        private const int chapmanMeanLengthMaxString = 500;
        private const double defaultMismatchScore = 0.0;
        private const double defaultPerfectScore = 1.0;

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            double num = secondWord.Length + firstWord.Length;
            if (num > 500.0)
                return 1.0;

            double num2 = (500.0 - num) / 500.0;
            return (1.0 - (((num2 * num2) * num2) * num2));
        }

        /// <see cref="AbstractStringMetric.GetSimilarityExplained(string, string)"/>
        /// <remarks>
        /// This method is not implement. Attempting to use this method will result in a <see cref="NotImplementedException"/>.
        /// </remarks>
        public override string GetSimilarityExplained(string firstWord, string secondWord)
        {
            throw new NotImplementedException();
        }

        /// <see cref="AbstractStringMetric.GetSimilarityTimingEstimated(string, string)"/>
        /// <remarks>
        /// This method always returns 0.0.
        /// </remarks>
        public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
        {
            return 0.0;
        }

        /// <see cref="AbstractStringMetric.GetUnnormalizedSimilarity(string, string)"/>
        /// <remarks>
        /// This method does the same thing as <see cref="GetSimilarity(string, string)"/>.
        /// </remarks>
        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            return this.GetSimilarity(firstWord, secondWord);
        }
    }
}
