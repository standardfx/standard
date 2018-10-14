using System;
using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Q Grams Distance algorithm provides a similarity measure between two strings using the qGram approach, check matching qGrams/possible matching qGrams.
    /// </summary>
    public sealed class QGramsDistance : AbstractStringMetric
    {
        private const double defaultMismatchScore = 0.0;
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="QGramsDistance"/> class.
        /// </summary>
        public QGramsDistance() : this(new TokenizerQGram3Extended())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QGramsDistance"/> class.
        /// </summary>
        /// <param name="tokenizerToUse">The tokenizer to use for parsing the input.</param>
        public QGramsDistance(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 0.0001340000017080456;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        private double GetActualSimilarity(Collection<string> firstTokens, Collection<string> secondTokens)
        {
            Collection<string> collection = this.tokenUtility.CreateMergedSet(firstTokens, secondTokens);
            int num = 0;
            foreach (string str in collection)
            {
                int num2 = 0;
                for (int i = 0; i < firstTokens.Count; i++)
                {
                    if (firstTokens[i].Equals(str))
                        num2++;
                }
                int num4 = 0;
                for (int j = 0; j < secondTokens.Count; j++)
                {
                    if (secondTokens[j].Equals(str))
                        num4++;
                }

                if (num2 > num4)
                    num += num2 - num4;
                else
                    num += num4 - num2;
            }
            return (double) num;
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                double unnormalizedSimilarity = this.GetUnnormalizedSimilarity(firstWord, secondWord);
                int num2 = this.tokenUtility.FirstTokenCount + this.tokenUtility.SecondTokenCount;
                if (num2 != 0)
                    return ((num2 - unnormalizedSimilarity) / ((double)num2));
            }
            return 0.0;
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
        public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                double length = firstWord.Length;
                double num2 = secondWord.Length;
                return ((length * num2) * this.estimatedTimingConstant);
            }
            return 0.0;
        }

        /// <see cref="AbstractStringMetric.GetUnnormalizedSimilarity(string, string)"/>
        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            Collection<string> firstTokens = this.tokenizer.Tokenize(firstWord);
            Collection<string> secondTokens = this.tokenizer.Tokenize(secondWord);
            this.tokenUtility.CreateMergedList(firstTokens, secondTokens);
            return this.GetActualSimilarity(firstTokens, secondTokens);
        }
    }
}
