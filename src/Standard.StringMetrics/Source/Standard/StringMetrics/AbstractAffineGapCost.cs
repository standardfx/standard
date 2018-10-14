namespace Standard.StringMetrics
{
    /// <summary>
    /// An abstract implementation of <see cref="IAffineGapCost"/>. All affinity cost objects inherit from this class.
    /// </summary>
    public abstract class AbstractAffineGapCost : IAffineGapCost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAffineGapCost"/> class.
        /// </summary>
        protected AbstractAffineGapCost()
        {
        }

        /// <see cref="IAffineGapCost.GetCost(string, int, int)"/>
        public abstract double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap);

        /// <see cref="IAffineGapCost.MaxCost"/>
        public abstract double MaxCost { get; }

        /// <see cref="IAffineGapCost.MinCost"/>
        public abstract double MinCost { get; }
    }
}
