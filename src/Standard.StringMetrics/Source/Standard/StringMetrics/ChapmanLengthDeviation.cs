using System;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Chapman Length Deviation algorithm uses the length deviation of the word strings to determine if the strings are similar in size. This apporach is not intended to be used single handedly but rather alongside other approaches.
    /// </summary>
    internal sealed class ChapmanLengthDeviation : AbstractStringMetric
    {
        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            double length = firstWord.Length;
            double num2 = secondWord.Length;

            if (length >= num2)
                return (num2 / length);

            return (length / num2);
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
