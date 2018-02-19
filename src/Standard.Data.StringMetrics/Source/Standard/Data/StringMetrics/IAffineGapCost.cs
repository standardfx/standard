namespace Standard.Data.StringMetrics
{
    public interface IAffineGapCost
    {
        double GetCost(string textToGap, int stringIndexStartGap, int stringIndexEndGap);

        double MaxCost { get; }

        double MinCost { get; }
    }
}
