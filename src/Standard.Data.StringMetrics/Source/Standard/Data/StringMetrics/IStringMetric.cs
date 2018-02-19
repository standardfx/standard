namespace Standard.Data.StringMetrics
{
    public interface IStringMetric
    {
        double GetSimilarity(string firstWord, string secondWord);

        string GetSimilarityExplained(string firstWord, string secondWord);

        long GetSimilarityTimingActual(string firstWord, string secondWord);

        double GetSimilarityTimingEstimated(string firstWord, string secondWord);

        double GetUnnormalizedSimilarity(string firstWord, string secondWord);
    }
}
