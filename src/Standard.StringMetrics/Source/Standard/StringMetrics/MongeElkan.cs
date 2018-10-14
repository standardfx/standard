using System;
using System.Collections.ObjectModel;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Monge Elkan algorithm provides a matching style similarity measure between two strings.
    /// </summary>
    public class MongeElkan : AbstractStringMetric
    {
        private const double defaultMismatchScore = 0.0;
        private double estimatedTimingConstant;
        private AbstractStringMetric internalStringMetric;
        internal ITokenizer tokenizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongeElkan"/> class using default parameters.
        /// </summary>
        public MongeElkan() 
            : this(new TokenizerWhitespace())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongeElkan"/> class, using the string metric algorithm specified.
        /// </summary>
        /// <param name="metricToUse">The meta string metric to use.</param>
        /// <remarks>
        /// The default tokenizer is the <see cref="TokenizerWhitespace"/>.
        /// </remarks>
        public MongeElkan(AbstractStringMetric metricToUse)
        {
            this.estimatedTimingConstant = 0.034400001168251038;
            this.tokenizer = new TokenizerWhitespace();
            this.internalStringMetric = metricToUse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongeElkan"/> class, using the tokenizer specified.
        /// </summary>
        /// <param name="tokenizerToUse">The tokenizer to use.</param>
        /// <remarks>
        /// The default string metric algorithm used is <see cref="SmithWatermanGotoh"/>.
        /// </remarks>
        public MongeElkan(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 0.034400001168251038;
            this.tokenizer = tokenizerToUse;
            this.internalStringMetric = new SmithWatermanGotoh();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongeElkan"/> class, using the parameters specified.
        /// </summary>
        /// <param name="tokenizerToUse">The tokenizer to use.</param>
        /// <param name="metricToUse">The meta string metric to use.</param>
        public MongeElkan(ITokenizer tokenizerToUse, AbstractStringMetric metricToUse)
        {
            this.estimatedTimingConstant = 0.034400001168251038;
            this.tokenizer = tokenizerToUse;
            this.internalStringMetric = metricToUse;
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            Collection<string> collection = this.tokenizer.Tokenize(firstWord);
            Collection<string> collection2 = this.tokenizer.Tokenize(secondWord);
            double num = 0.0;
            for (int i = 0; i < collection.Count; i++)
            {
                string str = collection[i];
                double num3 = 0.0;
                for (int j = 0; j < collection2.Count; j++)
                {
                    string str2 = collection2[j];
                    double similarity = this.internalStringMetric.GetSimilarity(str, str2);
                    if (similarity > num3)
                        num3 = similarity;
                }
                num += num3;
            }
            return (num / ((double)collection.Count));
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
        /// This method does the same thing as <see cref="GetSimilarity(string, string)"/>.
        /// </remarks>
        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            return this.GetSimilarity(firstWord, secondWord);
        }
    }
}
