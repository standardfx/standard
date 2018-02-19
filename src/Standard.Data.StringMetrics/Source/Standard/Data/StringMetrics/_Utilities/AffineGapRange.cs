namespace Standard.Data.StringMetrics
{
    public sealed class AffineGapRange1To0Multiplier1Over3 : AbstractAffineGapCost
    {
        private const int charExactMatchScore = 1;
        private const int charMismatchMatchScore = 0;

        public override double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap)
        {
            if (stringIndexStartGap >= stringIndexEndGap)
                return 0.0;

            return (double) (1f + (((stringIndexEndGap - 1) - stringIndexStartGap) * 0.3333333f));
        }

        public override double MaxCost
        {
            get { return 1.0; }
        }

        public override double MinCost
        {
            get { return 0.0; }
        }
    }

    public sealed class AffineGapRange5To0Multiplier1 : AbstractAffineGapCost
    {
        private const int charExactMatchScore = 5;
        private const int charMismatchMatchScore = 0;

        public override double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap)
        {
            if (stringIndexStartGap >= stringIndexEndGap)
                return 0.0;

            return (double)(5 + ((stringIndexEndGap - 1) - stringIndexStartGap));
        }

        public override double MaxCost
        {
            get { return 5.0; }
        }

        public override double MinCost
        {
            get { return 0.0; }
        }
    }
}
