namespace Standard.Data.StringMetrics
{
    /// <summary>
    /// Built-in string similarity algorithms.
    /// </summary>
    public enum SimMetricAlgorithm
    {
        /// <summary>
        /// Token based. Block distance algorithm uses vector space block distance to determine a similarity.
        /// </summary>
        BlockDistance,

        /// <summary>
        /// Length based. Chapman Length Deviation algorithm uses the length deviation of the word strings to determine if the strings are similar in size. This apporach is not intended to be used single handedly but rather alongside other approaches.
        /// </summary>
        ChapmanLengthDeviation,

        /// <summary>
        /// Length based. The Chapman Mean Length algorithm provides a similarity measure between two strings from size of the mean length of the vectors. This approach is supposed to be used to determine which metrics may be best to apply rather than giveing a valid response itself.
        /// </summary>
        ChapmanMeanLength,

        /// <summary>
        /// Token based. Cosine Similarity algorithm provides a similarity measure between two strings from the angular divergence within term based vector space.
        /// </summary>
        CosineSimilarity,

        /// <summary>
        /// Token based. Euclidean Distancey algorithm provides a similarity measure between two strings using the vector space of combined terms as the dimensions.
        /// </summary>
        EuclideanDistance,

        /// <summary>
        /// Token based. Jaccard Similarity algorithm provides a similarity measure between two strings.
        /// </summary>
        JaccardSimilarity,

        /// <summary>
        /// Token based. Dice Similarity algorithm provides a similarity measure between two strings using the vector space of present terms.
        /// </summary>
        DiceSimilarity,

        /// <summary>
        /// Jaro algorithm provides a similarity measure between two strings allowing character transpositions to a degree.
        /// </summary>       
        Jaro, 

        /// <summary>
        /// Jaro-Winkler algorithm provides a similarity measure between two strings allowing character transpositions to a degree, adjusting the weighting for common prefixes.
        /// </summary>
        JaroWinkler, 

        /// <summary>
        /// Token based. Matching Coefficient algorithm provides a similarity measure between two strings.
        /// </summary>
        MatchingCoefficient, 

        /// <summary>
        /// Token based. Monge Elkan algorithm provides a matching style similarity measure between two strings.
        /// </summary>
        MongeElkan,

        /// <summary>
        /// Token based. Overlap Coefficient algorithm provides a similarity measure between two string where it is determined to what degree a string is a subset of another.
        /// </summary>
        OverlapCoefficient,         

        /// <summary>
        /// Q Grams Distance algorithm provides a similarity measure between two strings using the qGram approach, check matching qGrams/possible matching qGrams.
        /// </summary>        
        QGramsDistance,

        /// <summary>
        /// Edit distance based. Basic Levenstein algorithm provides a similarity measure between two strings.
        /// </summary>
        Levenstein, 

        /// <summary>
        /// Edit distance based. Needleman-Wunch algorithm provides an edit distance based similarity measure between two strings.
        /// </summary>
        NeedlemanWunch, 

        /// <summary>
        /// Edit distance based. Smith-Waterman algorithm provides a similarity measure between two string.
        /// </summary>
        SmithWaterman, 

        /// <summary>
        /// Edit distance based. Smith-Waterman-Gotoh algorithm provides a similarity measure between two string.
        /// </summary>
        SmithWatermanGotoh, 

        /// <summary>
        /// Edit distance based. Implements the Smith-Waterman-Gotoh algorithm with a windowed affine gap providing a similarity measure between two string.
        /// </summary>
        SmithWatermanGotohWindowedAffine
    }
}
