namespace Standard.StringMetrics
{
    /// <summary>
    /// Smith-Waterman-Gotoh algorithm provides a similarity measure between two string.
    /// </summary>
    public sealed class SmithWatermanGotoh : SmithWatermanGotohWindowedAffine
    {
        private const int affineGapWindowSize = 0x7fffffff;
        private const double estimatedTimingConstant = 2.2000000171829015E-05;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotoh"/> class.
        /// </summary>
        public SmithWatermanGotoh() 
            : base(new AffineGapRange5To0Multiplier1(), new SubCostRange5ToMinus3(), 0x7fffffff)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotoh"/> class, using the affinity gap cost function specified.
        /// </summary>
        /// <param name="gapCostFunction">The affinity gap cost function to use.</param>
        public SmithWatermanGotoh(AbstractAffineGapCost gapCostFunction) 
            : base(gapCostFunction, new SubCostRange5ToMinus3(), 0x7fffffff)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotoh"/> class, using the substitution cost function specified.
        /// </summary>
        /// <param name="costFunction">The substitution cost function to use.</param>
        public SmithWatermanGotoh(AbstractSubstitutionCost costFunction) 
            : base(new AffineGapRange5To0Multiplier1(), costFunction, 0x7fffffff)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmithWatermanGotoh"/> class, using the affinity gap cost and substitution cost functions specified.
        /// </summary>
        /// <param name="gapCostFunction">The affinity gap cost function to use.</param>
        /// <param name="costFunction">The substitution cost function to use.</param>
        public SmithWatermanGotoh(AbstractAffineGapCost gapCostFunction, AbstractSubstitutionCost costFunction) 
            : base(gapCostFunction, costFunction, 0x7fffffff)
        {
        }

        /// <see cref="AbstractStringMetric.GetSimilarityTimingEstimated(string, string)"/>
        public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                double length = firstWord.Length;
                double num2 = secondWord.Length;
                return ((((length * num2) * length) + ((length * num2) * num2)) * 2.2000000171829015E-05);
            }
            return 0.0;
        }
    }
}
