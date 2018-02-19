using System;
using System.Collections.ObjectModel;

namespace Standard.Data.StringMetrics
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

        public MongeElkan() : this(new TokenizerWhitespace())
        {
        }

        public MongeElkan(AbstractStringMetric metricToUse)
        {
            this.estimatedTimingConstant = 0.034400001168251038;
            this.tokenizer = new TokenizerWhitespace();
            this.internalStringMetric = metricToUse;
        }

        public MongeElkan(ITokenizer tokenizerToUse)
        {
            this.estimatedTimingConstant = 0.034400001168251038;
            this.tokenizer = tokenizerToUse;
            this.internalStringMetric = new SmithWatermanGotoh();
        }

        public MongeElkan(ITokenizer tokenizerToUse, AbstractStringMetric metricToUse)
        {
            this.estimatedTimingConstant = 0.034400001168251038;
            this.tokenizer = tokenizerToUse;
            this.internalStringMetric = metricToUse;
        }

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
            return this.GetSimilarity(firstWord, secondWord);
        }
    }
}
