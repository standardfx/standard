namespace Standard.StringMetrics
{
    /// <summary>
    /// Factory class for creating new stric metric instances.
    /// </summary>
	public static class StringMetricFactory
	{
        /// <summary>
        /// Returns a new instance of a supported string metrics calculator.
        /// </summary>
        /// <param name="algorithm">A supported string similarity algorithm.</param>
        /// <returns>A string metric measurement class.</returns>
		public static IStringMetric FromAlgorithm(SimMetricAlgorithm algorithm)
		{
            switch (algorithm)
            {
                case SimMetricAlgorithm.BlockDistance:
                    return new BlockDistance();
                case SimMetricAlgorithm.ChapmanLengthDeviation:
                    return new ChapmanLengthDeviation();
                case SimMetricAlgorithm.CosineSimilarity:
                    return new CosineSimilarity();
                case SimMetricAlgorithm.DiceSimilarity:
                    return new DiceSimilarity();
                case SimMetricAlgorithm.EuclideanDistance:
                    return new EuclideanDistance();
                case SimMetricAlgorithm.JaccardSimilarity:
                    return new JaccardSimilarity();
                case SimMetricAlgorithm.Jaro:
                    return new Jaro();
                case SimMetricAlgorithm.JaroWinkler:
                    return new JaroWinkler();
                case SimMetricAlgorithm.MatchingCoefficient:
                    return new MatchingCoefficient();
                case SimMetricAlgorithm.MongeElkan:
                    return new MongeElkan();
                case SimMetricAlgorithm.NeedlemanWunch:
                    return new NeedlemanWunch();
                case SimMetricAlgorithm.OverlapCoefficient:
                    return new OverlapCoefficient();
                case SimMetricAlgorithm.QGramsDistance:
                    return new QGramsDistance();
                case SimMetricAlgorithm.SmithWaterman:
                    return new SmithWaterman();
                case SimMetricAlgorithm.SmithWatermanGotoh:
                    return new SmithWatermanGotoh();
                case SimMetricAlgorithm.SmithWatermanGotohWindowedAffine:
                    return new SmithWatermanGotohWindowedAffine();
                case SimMetricAlgorithm.ChapmanMeanLength:
                    return new ChapmanMeanLength();
                default:
                    return new Levenstein();
            }
        }
	}
}
