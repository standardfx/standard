namespace Standard.Data.StringMetrics
{
    public abstract class AbstractSubstitutionCost : ISubstitutionCost
    {
        protected AbstractSubstitutionCost()
        {
        }

        public abstract double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex);

        public abstract double MaxCost { get; }

        public abstract double MinCost { get; }
    }
}
