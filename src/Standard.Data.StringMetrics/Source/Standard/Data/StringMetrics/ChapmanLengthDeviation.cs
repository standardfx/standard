using System;

namespace Standard.Data.StringMetrics
{
    /// <summary>
    /// Chapman Length Deviation algorithm uses the length deviation of the word strings to determine if the strings are similar in size. This apporach is not intended to be used single handedly but rather alongside other approaches.
    /// </summary>
    public sealed class ChapmanLengthDeviation : AbstractStringMetric
    {
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

        public override string GetSimilarityExplained(string firstWord, string secondWord)
        {
            throw new NotImplementedException();
        }

        public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
        {
            return 0.0;
        }

        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            return this.GetSimilarity(firstWord, secondWord);
        }
    }
}
