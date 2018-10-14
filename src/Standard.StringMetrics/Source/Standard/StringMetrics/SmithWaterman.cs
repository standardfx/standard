using System;
using Standard;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Smith-Waterman algorithm provides a similarity measure between two string.
    /// </summary>
    public sealed class SmithWaterman : AbstractStringMetric
    {
        private AbstractSubstitutionCost dCostFunction;
        private const double defaultGapCost = 0.5;
        private const double defaultMismatchScore = 0.0;
        private const double defaultPerfectMatchScore = 1.0;
        private const double estimatedTimingConstant = 0.0001610000035725534;
        private double gapCost;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWaterman"/> class.
        /// </summary>
        public SmithWaterman() 
            : this(0.5, new SubCostRange1ToMinus2())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWaterman"/> class, using the cost function specified.
        /// </summary>
        /// <param name="costFunction">The cost function to use.</param>
        public SmithWaterman(AbstractSubstitutionCost costFunction) 
            : this(0.5, costFunction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWaterman"/> class with the specified gap cost.
        /// </summary>
        /// <param name="costG">The gap cost.</param>
        public SmithWaterman(double costG) 
            : this(costG, new SubCostRange1ToMinus2())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWaterman"/> class with the specified gap cost and cost function
        /// </summary>
        /// <param name="costG">The gap cost.</param>
        /// <param name="costFunction">The cost function to use.</param>
        public SmithWaterman(double costG, AbstractSubstitutionCost costFunction)
        {
            this.gapCost = costG;
            this.dCostFunction = costFunction;
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            double unnormalizedSimilarity = this.GetUnnormalizedSimilarity(firstWord, secondWord);
            double num2 = Math.Min(firstWord.Length, secondWord.Length);
            if (this.dCostFunction.MaxCost > -this.gapCost)
                num2 *= this.dCostFunction.MaxCost;
            else
                num2 *= -this.gapCost;

            if (num2 == 0.0)
                return 1.0;

            return (unnormalizedSimilarity / num2);
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
                return ((((length * num2) + length) + num2) * 0.0001610000035725534);
            }
            return 0.0;
        }

        /// <see cref="AbstractStringMetric.GetUnnormalizedSimilarity(string, string)"/>
        public override double GetUnnormalizedSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            int length = firstWord.Length;
            int num2 = secondWord.Length;
            if (length == 0)
                return (double) num2;

            if (num2 == 0)
                return (double)length;

            double[][] numArray = new double[length][];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = new double[num2];
            }
            double num4 = 0.0;
            for (int j = 0; j < length; j++)
            {
                double thirdNumber = this.dCostFunction.GetCost(firstWord, j, secondWord, 0);
                if (j == 0)
                    numArray[0][0] = MathUtility.Max(0.0, -this.gapCost, thirdNumber);
                else
                    numArray[j][0] = MathUtility.Max(0.0, numArray[j - 1][0] - this.gapCost, thirdNumber);

                if (numArray[j][0] > num4)
                    num4 = numArray[j][0];
            }
            for (int k = 0; k < num2; k++)
            {
                double num8 = this.dCostFunction.GetCost(firstWord, 0, secondWord, k);
                if (k == 0)
                    numArray[0][0] = MathUtility.Max(0.0, -this.gapCost, num8);
                else
                    numArray[0][k] = MathUtility.Max(0.0, numArray[0][k - 1] - this.gapCost, num8);

                if (numArray[0][k] > num4)
                    num4 = numArray[0][k];
            }
            for (int m = 1; m < length; m++)
            {
                for (int n = 1; n < num2; n++)
                {
                    double num11 = this.dCostFunction.GetCost(firstWord, m, secondWord, n);
                    numArray[m][n] = MathUtility.Max(0.0, numArray[m - 1][n] - this.gapCost, numArray[m][n - 1] - this.gapCost, numArray[m - 1][n - 1] + num11);
                    if (numArray[m][n] > num4)
                        num4 = numArray[m][n];
                }
            }
            return num4;
        }

        /// <summary>
        /// Gets or sets the substitution cost function.
        /// </summary>
        public AbstractSubstitutionCost DCostFunction
        {
            get { return this.dCostFunction; }
            set { this.DCostFunction = value; }
        }

        /// <summary>
        /// Gets or sets the gap cost.
        /// </summary>
        public double GapCost
        {
            get { return this.gapCost; }
            set { this.gapCost = value; }
        }
    }
}
