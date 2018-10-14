namespace Standard.StringMetrics
{
    /// <summary>
    /// Abstract implement for the <see cref="ISubstitutionCost"/> interface. All substitution cost implementations inherit from this class.
    /// </summary>
    public abstract class AbstractSubstitutionCost : ISubstitutionCost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSubstitutionCost"/> class.
        /// </summary>
        protected AbstractSubstitutionCost()
        {
        }

        /// <see cref="ISubstitutionCost.GetCost(string, int, string, int)"/>
        public abstract double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex);

        /// <see cref="ISubstitutionCost.MaxCost"/>
        public abstract double MaxCost { get; }

        /// <see cref="ISubstitutionCost.MinCost"/>
        public abstract double MinCost { get; }
    }
}
