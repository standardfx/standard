using System;
using System.Collections.ObjectModel;

namespace Standard.Data.StringMetrics
{
    /// <summary>
    /// Jaccard Similarity algorithm provides a similarity measure between two strings.
    /// </summary>
    public sealed class JaccardSimilarity : AbstractStringMetric
    {
        private const double defaultMismatchScore = 0.0;
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        public JaccardSimilarity() : this(new TokenizerWhitespace())
        {
        }

        public JaccardSimilarity(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 0.00014000000373926014;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                Collection<string> collection = this.tokenUtility.CreateMergedSet(this.tokenizer.Tokenize(firstWord), this.tokenizer.Tokenize(secondWord));
                if (collection.Count > 0)
                    return (((double)this.tokenUtility.CommonSetTerms()) / ((double)collection.Count));
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
