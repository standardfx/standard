using System;

namespace Standard.Data.StringMetrics
{
    /// <summary>
    /// Cosine Similarity algorithm provides a similarity measure between two strings from the angular divergence within term based vector space.
    /// </summary>
    public sealed class CosineSimilarity : AbstractStringMetric
    {
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        public CosineSimilarity() : this(new TokenizerWhitespace())
        {
        }

        public CosineSimilarity(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 3.8337140040312079E-07;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if (((firstWord != null) && 
                (secondWord != null)) && 
                (this.tokenUtility.CreateMergedSet(this.tokenizer.Tokenize(firstWord), this.tokenizer.Tokenize(secondWord)).Count > 0))
            {
                return (((double)this.tokenUtility.CommonSetTerms()) / (Math.Pow((double) this.tokenUtility.FirstSetTokenCount, 0.5) * Math.Pow((double) this.tokenUtility.SecondSetTokenCount, 0.5)));
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
