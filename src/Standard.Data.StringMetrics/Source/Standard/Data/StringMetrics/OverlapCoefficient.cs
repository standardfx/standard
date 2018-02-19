using System;

namespace Standard.Data.StringMetrics
{
    /// <summary>
    /// Overlap Coefficient algorithm provides a similarity measure between two string where it is determined to what degree a string is a subset of another.
    /// </summary>
    public sealed class OverlapCoefficient : AbstractStringMetric
    {
        private const double defaultMismatchScore = 0.0;
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        public OverlapCoefficient() : this(new TokenizerWhitespace())
        {
        }

        public OverlapCoefficient(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 0.00014000000373926014;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                this.tokenUtility.CreateMergedSet(this.tokenizer.Tokenize(firstWord), this.tokenizer.Tokenize(secondWord));
                return (((double)this.tokenUtility.CommonSetTerms()) / ((double)Math.Min(this.tokenUtility.FirstSetTokenCount, this.tokenUtility.SecondSetTokenCount)));
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
                return ((count * num2) * this.estimatedTimingConstant);
            }
            return 0.0;
        }

        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            return this.GetSimilarity(firstWord, secondWord);
        }
    }
}
