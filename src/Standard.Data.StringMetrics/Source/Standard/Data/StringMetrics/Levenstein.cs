using System;
using Standard;

namespace Standard.Data.StringMetrics
{
    /// <summary>
    /// Basic Levenstein algorithm provides a similarity measure between two strings.
    /// </summary>
    public sealed class Levenstein : AbstractStringMetric
    {
        private AbstractSubstitutionCost dCostFunction = new SubCostRange0To1();
        private const double defaultMismatchScore = 0.0;
        private const double defaultPerfectMatchScore = 1.0;
        private double estimatedTimingConstant = 0.00018000000272877514;

        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            double unnormalizedSimilarity = this.GetUnnormalizedSimilarity(firstWord, secondWord);
            double length = firstWord.Length;
            if (length < secondWord.Length)
                length = secondWord.Length;

            if (length == 0.0)
                return 1.0;

            return (1.0 - (unnormalizedSimilarity / length));
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
                return ((length * num2) * this.estimatedTimingConstant);
            }
            return 0.0;
        }

        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            int length = firstWord.Length;
            int index = secondWord.Length;
            if (length == 0)
                return (double) index;

            if (index == 0)
                return (double) length;

            double[][] numArray = new double[length + 1][];
            for (int i = 0; i < (length + 1); i++)
            {
                numArray[i] = new double[index + 1];
            }
            for (int j = 0; j <= length; j++)
            {
                numArray[j][0] = j;
            }
            for (int k = 0; k <= index; k++)
            {
                numArray[0][k] = k;
            }
            for (int m = 1; m <= length; m++)
            {
                for (int n = 1; n <= index; n++)
                {
                    double num8 = this.dCostFunction.GetCost(firstWord, m - 1, secondWord, n - 1);
                    numArray[m][n] = MathUtility.Min((double)(numArray[m - 1][n] + 1.0), (double)(numArray[m][n - 1] + 1.0), (double)(numArray[m - 1][n - 1] + num8));
                }
            }
            return numArray[length][index];
        }
    }
}
