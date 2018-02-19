using System.Collections.Generic;
using Standard.Data.StringMetrics;

namespace Standard
{
    /// <summary>
    /// Extension methods for comparing string similarity.
    /// </summary>
    public static class StringMetricsExtension
    {
        //<#
        //  .SYNOPSIS
        //      Compares two strings for similarity. The returned result indicates the level of similarity between the two string.
        //
        //  .EXAMPLE
        //      ```C#
        //      string word = "fooler";
        //      List<string> list = new List<string>() { "fowler", "fish", "crawler" };
        //  
        //      List<string> newList = new List<string>();
        //      foreach (string l in list)
        //      {
        //          double num = l.NearEquals(word, algorithm);
        //          double thr = 1 - num;
        //          if (thr <= threshold)
        //              newList.Add(l);
        //      }
        //      Console.WriteLine(newList);
        //      ```
        //
        //      DESCRIPTION
        //      -----------
        //      Compares each member in a list of string against the specified word for similarity. The returned result will all 
        //      have passed the specified similarity threshold.
        //#>
        public static double NearEquals(this string firstWord, string secondWord, SimMetricAlgorithm algorithm = SimMetricAlgorithm.Levenstein)
        {
            AbstractStringMetric sim;

            switch (algorithm)
            {
                case SimMetricAlgorithm.BlockDistance:
                    sim = new BlockDistance();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.ChapmanLengthDeviation:
                    sim = new ChapmanLengthDeviation();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.CosineSimilarity:
                    sim = new CosineSimilarity();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.DiceSimilarity:
                    sim = new DiceSimilarity();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.EuclideanDistance:
                    sim = new EuclideanDistance();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.JaccardSimilarity:
                    sim = new JaccardSimilarity();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.Jaro:
                    sim = new Jaro();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.JaroWinkler:
                    sim = new JaroWinkler();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.MatchingCoefficient:
                    sim = new MatchingCoefficient();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.MongeElkan:
                    sim = new MongeElkan();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.NeedlemanWunch:
                    sim = new NeedlemanWunch();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.OverlapCoefficient:
                    sim = new OverlapCoefficient();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.QGramsDistance:
                    sim = new QGramsDistance();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.SmithWaterman:
                    sim = new SmithWaterman();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.SmithWatermanGotoh:
                    sim = new SmithWatermanGotoh();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.SmithWatermanGotohWindowedAffine:
                    sim = new SmithWatermanGotohWindowedAffine();
                    return sim.GetSimilarity(firstWord, secondWord);
                case SimMetricAlgorithm.ChapmanMeanLength:
                    sim = new ChapmanMeanLength();
                    return sim.GetSimilarity(firstWord, secondWord);
                default:
                    sim = new Levenstein();
                    return sim.GetSimilarity(firstWord, secondWord);
            }
        }
    }
}
