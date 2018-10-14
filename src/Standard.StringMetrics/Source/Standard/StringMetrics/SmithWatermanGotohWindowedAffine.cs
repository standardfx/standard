using System;
using Standard;

namespace Standard.StringMetrics
{
    /// <summary>
    /// Implements the Smith-Waterman-Gotoh algorithm with a windowed affine gap providing a similarity measure between two string.
    /// </summary>
    public class SmithWatermanGotohWindowedAffine : AbstractStringMetric
    {
        private AbstractSubstitutionCost dCostFunction;
        private const double defaultMismatchScore = 0.0;
        private const double defaultPerfectScore = 1.0;
        private const int defaultWindowSize = 100;
        private double estimatedTimingConstant;
        private AbstractAffineGapCost gGapFunction;
        private int windowSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class using default parameters.
        /// </summary>
        public SmithWatermanGotohWindowedAffine() 
            : this(new AffineGapRange5To0Multiplier1(), new SubCostRange5ToMinus3(), 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class with the specified gap cost function.
        /// </summary>
        /// <param name="gapCostFunction">The gap cost function to use.</param>
        public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction) 
            : this(gapCostFunction, new SubCostRange5ToMinus3(), 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class with the specified substitution cost function.
        /// </summary>
        /// <param name="costFunction">The substitution cost function to use.</param>
        public SmithWatermanGotohWindowedAffine(AbstractSubstitutionCost costFunction) 
            : this(new AffineGapRange5To0Multiplier1(), costFunction, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class with the specified affinity gap window size.
        /// </summary>
        /// <param name="affineGapWindowSize">Specifies the affinity gap window size.</param>
        public SmithWatermanGotohWindowedAffine(int affineGapWindowSize) 
            : this(new AffineGapRange5To0Multiplier1(), new SubCostRange5ToMinus3(), affineGapWindowSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class with the specified parameters.
        /// </summary>
        /// <param name="gapCostFunction">The gap cost function to use.</param>
        /// <param name="costFunction">The substitution cost function to use.</param>
        public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction, AbstractSubstitutionCost costFunction) 
            : this(gapCostFunction, costFunction, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class with the specified parameters.
        /// </summary>
        /// <param name="gapCostFunction">The gap cost function to use.</param>
        /// <param name="affineGapWindowSize">Specifies the affinity gap window size.</param>
        public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction, int affineGapWindowSize) 
            : this(gapCostFunction, new SubCostRange5ToMinus3(), affineGapWindowSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class with the specified parameters.
        /// </summary>
        /// <param name="affineGapWindowSize">The affinity gap window size.</param>
        /// <param name="costFunction">The substitution cost function to use.</param>
        public SmithWatermanGotohWindowedAffine(AbstractSubstitutionCost costFunction, int affineGapWindowSize) 
            : this(new AffineGapRange5To0Multiplier1(), costFunction, affineGapWindowSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotohWindowedAffine"/> class with the specified parameters.
        /// </summary>
        /// <param name="gapCostFunction">The gap cost function to use.</param>
        /// <param name="costFunction">The substitution cost function to use.</param>
        /// <param name="affineGapWindowSize">Specifies the affinity gap window size.</param>
        public SmithWatermanGotohWindowedAffine(AbstractAffineGapCost gapCostFunction, AbstractSubstitutionCost costFunction, int affineGapWindowSize)
        {
            this.estimatedTimingConstant = 4.5000000682193786E-05;
            this.gGapFunction = gapCostFunction;
            this.dCostFunction = costFunction;
            this.windowSize = affineGapWindowSize;
        }

        /// <see cref="AbstractStringMetric.GetSimilarity(string, string)"/>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            if ((firstWord == null) || (secondWord == null))
                return 0.0;

            double unnormalizedSimilarity = this.GetUnnormalizedSimilarity(firstWord, secondWord);
            double num2 = Math.Min(firstWord.Length, secondWord.Length);
            if (this.dCostFunction.MaxCost > -this.gGapFunction.MaxCost)
                num2 *= this.dCostFunction.MaxCost;
            else
                num2 *= -this.gGapFunction.MaxCost;

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
                return ((((length * num2) * this.windowSize) + ((length * num2) * this.windowSize)) * this.estimatedTimingConstant);
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
                return (double) length;

            double[][] numArray = new double[length][];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = new double[num2];
            }
            double num4 = 0.0;
            for (int j = 0; j < length; j++)
            {
                double num6 = this.dCostFunction.GetCost(firstWord, j, secondWord, 0);
                if (j == 0)
                    numArray[0][0] = Math.Max(0.0, num6);
                else
                {
                    double num7 = 0.0;
                    int num8 = j - this.windowSize;
                    if (num8 < 1)
                        num8 = 1;

                    for (int n = num8; n < j; n++)
                    {
                        num7 = Math.Max(num7, numArray[j - n][0] - this.gGapFunction.GetCost(firstWord, j - n, j));
                    }
                    numArray[j][0] = MathUtility.Max(0.0, num7, num6);
                }
                if (numArray[j][0] > num4)
                    num4 = numArray[j][0];
            }
            for (int k = 0; k < num2; k++)
            {
                double num11 = this.dCostFunction.GetCost(firstWord, 0, secondWord, k);
                if (k == 0)
                    numArray[0][0] = Math.Max(0.0, num11);
                else
                {
                    double num12 = 0.0;
                    int num13 = k - this.windowSize;
                    if (num13 < 1)
                        num13 = 1;
                    for (int num14 = num13; num14 < k; num14++)
                    {
                        num12 = Math.Max(num12, numArray[0][k - num14] - this.gGapFunction.GetCost(secondWord, k - num14, k));
                    }
                    numArray[0][k] = MathUtility.Max(0.0, num12, num11);
                }
                if (numArray[0][k] > num4)
                    num4 = numArray[0][k];
            }
            for (int m = 1; m < length; m++)
            {
                for (int num16 = 1; num16 < num2; num16++)
                {
                    double num17 = this.dCostFunction.GetCost(firstWord, m, secondWord, num16);
                    double num18 = 0.0;
                    double num19 = 0.0;
                    int num20 = m - this.windowSize;
                    if (num20 < 1)
                        num20 = 1;
                    for (int num21 = num20; num21 < m; num21++)
                    {
                        num18 = Math.Max(num18, numArray[m - num21][num16] - this.gGapFunction.GetCost(firstWord, m - num21, m));
                    }
                    num20 = num16 - this.windowSize;
                    if (num20 < 1)
                        num20 = 1;
                    for (int num22 = num20; num22 < num16; num22++)
                    {
                        num19 = Math.Max(num19, numArray[m][num16 - num22] - this.gGapFunction.GetCost(secondWord, num16 - num22, num16));
                    }
                    numArray[m][num16] = MathUtility.Max(0.0, num18, num19, numArray[m - 1][num16 - 1] + num17);
                    if (numArray[m][num16] > num4)
                        num4 = numArray[m][num16];
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
            set { this.dCostFunction = value; }
        }

        /// <summary>
        /// Gets or sets the affinity gap cost function.
        /// </summary>
        public AbstractAffineGapCost GGapFunction
        {
            get { return this.gGapFunction; }
            set { this.gGapFunction = value; }
        }
    }
}
