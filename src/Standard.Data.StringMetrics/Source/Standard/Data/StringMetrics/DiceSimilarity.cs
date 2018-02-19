using System;

namespace Standard.Data.StringMetrics
{
    /// <summary>
    /// Dice Similarity algorithm provides a similarity measure between two strings using the vector space of present terms.
    /// </summary>
    public sealed class DiceSimilarity : AbstractStringMetric
    {
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        public DiceSimilarity() : this(new TokenizerWhitespace())
        {
        }

        public DiceSimilarity(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 3.4457139008736704E-07;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if (((firstWord != null) && 
                (secondWord != null)) && 
                (this.tokenUtility.CreateMergedSet(this.tokenizer.Tokenize(firstWord), this.tokenizer.Tokenize(secondWord)).Count > 0))
            {
                return ((2.0 * this.tokenUtility.CommonSetTerms()) / ((double)(this.tokenUtility.FirstSetTokenCount + this.tokenUtility.SecondSetTokenCount)));
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
                double length = firstWord.Length;
                double num2 = secondWord.Length;
                return ((length + num2) * ((length + num2) * this.estimatedTimingConstant));
            }
            return 0.0;
        }

        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            return this.GetSimilarity(firstWord, secondWord);
        }
    }
}
