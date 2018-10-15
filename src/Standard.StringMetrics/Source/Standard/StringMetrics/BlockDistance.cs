using System;
using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Block distance algorithm uses vector space block distance to determine a similarity.
    /// </summary>
    internal sealed class BlockDistance : AbstractStringMetric
    {
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDistance"/> class.
        /// </summary>
        public BlockDistance() 
            : this(new TokenizerWhitespace())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDistance"/> class, using the tokenizer specified.
        /// </summary>
        /// <param name="tokenizerToUse">The tokenizer to use for parsing the input.</param>
        public BlockDistance(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 6.4457140979357064E-05;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        private double GetActualSimilarity(Collection<string> firstTokens, Collection<string> secondTokens)
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

                if (num2 > num3)
                    num += num2 - num3;
                else
                    num += num3 - num2;
            }

            return (double)num;
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            Collection<string> firstTokens = this.tokenizer.Tokenize(firstWord);
            Collection<string> secondTokens = this.tokenizer.Tokenize(secondWord);
            int num = firstTokens.Count + secondTokens.Count;
            double actualSimilarity = this.GetActualSimilarity(firstTokens, secondTokens);
            return ((num - actualSimilarity) / ((double) num));
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
            double count = this.tokenizer.Tokenize(firstWord).Count;
            double num2 = this.tokenizer.Tokenize(secondWord).Count;
            return ((((count + num2) * count) + ((count + num2) * num2)) * this.estimatedTimingConstant);
        }

        /// <see cref="AbstractStringMetric.GetUnnormalizedSimilarity(string, string)"/>
        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            Collection<string> firstTokens = this.tokenizer.Tokenize(firstWord);
            Collection<string> secondTokens = this.tokenizer.Tokenize(secondWord);
            return this.GetActualSimilarity(firstTokens, secondTokens);
        }
    }
}
