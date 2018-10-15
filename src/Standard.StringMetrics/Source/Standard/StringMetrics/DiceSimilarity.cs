using System;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Dice Similarity algorithm provides a similarity measure between two strings using the vector space of present terms.
    /// </summary>
    internal sealed class DiceSimilarity : AbstractStringMetric
    {
        private double estimatedTimingConstant;
        private ITokenizer tokenizer;
        private TokenizerUtility<string> tokenUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiceSimilarity"/> class.
        /// </summary>
        public DiceSimilarity() 
            : this(new TokenizerWhitespace())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiceSimilarity"/> class, using the tokenizer specified.
        /// </summary>
        /// <param name="tokenizerToUse">The tokenizer to use for parsing the input.</param>
        public DiceSimilarity(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 3.4457139008736704E-07;
            this.tokenizer = tokenizerToUse;
            this.tokenUtility = new TokenizerUtility<string>();
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
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
                return ((length + num2) * ((length + num2) * this.estimatedTimingConstant));
            }
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
