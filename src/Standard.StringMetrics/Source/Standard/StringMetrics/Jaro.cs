using System;
using System.Text;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Jaro algorithm provides a similarity measure between two strings allowing character transpositions to a degree.
    /// </summary>
    public sealed class Jaro : AbstractStringMetric
    {
        private const double defaultMismatchScore = 0.0;
        private double estimatedTimingConstant = 4.1200000850949436E-05;

        private static StringBuilder GetCommonCharacters(string firstWord, string secondWord, int distanceSep)
        {
            if ((firstWord == null) || (secondWord == null))
                return null;

            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder(secondWord);
            for (int i = 0; i < firstWord.Length; i++)
            {
                char ch = firstWord[i];
                bool flag = false;
                for (int j = Math.Max(0, i - distanceSep); !flag && (j < Math.Min(i + distanceSep, secondWord.Length)); j++)
                {
                    if (builder2[j] == ch)
                    {
                        flag = true;
                        builder.Append(ch);
                        builder2[j] = '#';
                    }
                }
            }
            return builder;
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            int distanceSep = (Math.Min(firstWord.Length, secondWord.Length) / 2) + 1;
            StringBuilder builder = GetCommonCharacters(firstWord, secondWord, distanceSep);
            int length = builder.Length;
            if (length == 0)
                return 0.0;

            StringBuilder builder2 = GetCommonCharacters(secondWord, firstWord, distanceSep);
            if (length != builder2.Length)
                return 0.0;

            int num3 = 0;
            for (int i = 0; i < length; i++)
            {
                if (builder[i] != builder2[i])
                    num3++;
            }
            num3 /= 2;
            return (((((double)length) / (3.0 * firstWord.Length)) + (((double) length) / (3.0 * secondWord.Length))) + (((double)(length - num3)) / (3.0 * length)));
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
                return ((length * num2) * this.estimatedTimingConstant);
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
