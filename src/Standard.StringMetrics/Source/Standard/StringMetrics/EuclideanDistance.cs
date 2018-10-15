using System;
using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Euclidean Distancey algorithm provides a similarity measure between two strings using the vector space of combined terms as the dimensions.
    /// </summary>
    public sealed class EuclideanDistance : AbstractStringMetric
    {
        private const double defaultMismatchScore = 0.0;
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistance"/> class.
        /// </summary>
        public EuclideanDistance() 
            : this(new TokenizerWhitespace())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistance"/> class, using the tokenizer specified.
        /// </summary>
        /// <param name="tokenizerToUse">The tokenizer to use for parsing the input.</param>
        public EuclideanDistance(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 7.4457137088757008E-05;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        private double GetActualDistance(Collection<string> firstTokens, Collection<string> secondTokens)
        {
            Collection<string> collection = this.tokenUtility.CreateMergedList(firstTokens, secondTokens);
            int num = 0;
            foreach (string str in collection)
            {
                int num2 = 0;
                int num3 = 0;
                if (firstTokens.Contains(str))
                    num2++;

                if (secondTokens.Contains(str))
                    num3++;

                num += (num2 - num3) * (num2 - num3);
            }

            return Math.Sqrt((double)num);
        }

        /// <summary>
        /// Returns the euclid distance between two strings.
        /// </summary>
        /// <param name="firstWord">The first string to compare.</param>
        /// <param name="secondWord">The second string to compare.</param>
        /// <returns>The euclid distance between <paramref name="firstWord"/> and <paramref name="secondWord"/>.</returns>
        public double GetEuclidDistance(string firstWord, string secondWord)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                Collection<string> firstTokens = this.tokenizer.Tokenize(firstWord);
                Collection<string> secondTokens = this.tokenizer.Tokenize(secondWord);
                return this.GetActualDistance(firstTokens, secondTokens);
            }

            return 0.0;
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                double unnormalizedSimilarity = this.GetUnnormalizedSimilarity(firstWord, secondWord);
                double num2 = Math.Sqrt((double)(this.tokenUtility.FirstTokenCount + this.tokenUtility.SecondTokenCount));
                return ((num2 - unnormalizedSimilarity) / num2);
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
                double count = this.tokenizer.Tokenize(firstWord).Count;
                double num2 = this.tokenizer.Tokenize(secondWord).Count;
                return ((((count + num2) * count) + ((count + num2) * num2)) * this.estimatedTimingConstant);
            }
            return 0.0;
        }

        /// <see cref="AbstractStringMetric.GetUnnormalizedSimilarity(string, string)"/>
        /// <remarks>
        /// This method does the same thing as <see cref="GetEuclidDistance(string, string)"/>.
        /// </remarks>
        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            return this.GetEuclidDistance(firstWord, secondWord);
        }
    }
}
