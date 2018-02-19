using System;
using System.Collections.ObjectModel;

namespace Standard.Data.StringMetrics
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

        public EuclideanDistance() : this(new TokenizerWhitespace())
        {
        }

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

        public override string GetSimilarityExplained(string firstWord, string secondWord)
        {
            throw new NotImplementedException();
        }

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

        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            return this.GetEuclidDistance(firstWord, secondWord);
        }
    }
}
