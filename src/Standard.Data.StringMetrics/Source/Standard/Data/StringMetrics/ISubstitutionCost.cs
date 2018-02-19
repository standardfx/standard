namespace Standard.Data.StringMetrics
{
    public interface ISubstitutionCost
    {
        double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex);

        double MaxCost { get; }

        double MinCost { get; }
    }
}