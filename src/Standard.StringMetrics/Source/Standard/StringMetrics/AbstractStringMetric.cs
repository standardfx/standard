using System;

namespace Standard.StringMetrics
{
    /// <summary>
    /// An abstract implementation of <see cref="IStringMetric"/>. All algorithm implementations inherit from this class.
    /// </summary>
    public abstract class AbstractStringMetric : IStringMetric
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractStringMetric"/> class.
        /// </summary>
        protected AbstractStringMetric()
        {
        }

        /// <summary>
        /// Runs the <see cref="GetSimilarity(string, string)"/> function against each member of an array.
        /// </summary>
        /// <param name="setRenamed">An array of strings to test for similarity against <paramref name="comparator"/>.</param>
        /// <param name="comparator">A string that will be compared against each item in <paramref name="setRenamed"/>.</param>
        /// <returns>
        /// An array of numbers indicating the <see cref="GetSimilarity(string, string)"/> result of each item in <paramref name="setRenamed"/> 
        /// against <paramref name="comparator"/>.
        /// </returns>
        public double[] BatchCompareSet(string[] setRenamed, string comparator)
        {
            if ((setRenamed == null) || (comparator == null))
                return null;

            double[] numArray = new double[setRenamed.Length];
            for (int i = 0; i < setRenamed.Length; i++)
            {
                numArray[i] = this.GetSimilarity(setRenamed[i], comparator);
            }
            return numArray;
        }

        /// <summary>
        /// Runs the <see cref="GetSimilarity(string, string)"/> function against each member of two arrays.
        /// </summary>
        /// <param name="firstSet">The first array of strings.</param>
        /// <param name="secondSet">The second array of strings.</param>
        /// <returns>
        /// An array of numbers indicating the <see cref="GetSimilarity(string, string)"/> results 
        /// of the items in <paramref name="firstSet"/> and <paramref name="secondSet"/> in each index position.
        /// </returns>
        public double[] BatchCompareSets(string[] firstSet, string[] secondSet)
        {
            double[] numArray;
            if ((firstSet == null) || (secondSet == null))
                return null;

            if (firstSet.Length <= secondSet.Length)
                numArray = new double[firstSet.Length];
            else
                numArray = new double[secondSet.Length];

            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = this.GetSimilarity(firstSet[i], secondSet[i]);
            }
            return numArray;
        }

        /// <see cref="IStringMetric.GetSimilarity(string, string)"/>
        public abstract double GetSimilarity(string firstWord, string secondWord);

        /// <see cref="IStringMetric.GetSimilarityExplained(string, string)"/>
        public abstract string GetSimilarityExplained(string firstWord, string secondWord);

        /// <see cref="IStringMetric.GetSimilarityTimingActual(string, string)"/>
        public long GetSimilarityTimingActual(string firstWord, string secondWord)
        {
            long num = (DateTime.Now.Ticks - 0x89f7ff5f7b58000L) / 0x2710L;
            this.GetSimilarity(firstWord, secondWord);
            long num2 = (DateTime.Now.Ticks - 0x89f7ff5f7b58000L) / 0x2710L;
            return (num2 - num);
        }

        /// <see cref="IStringMetric.GetSimilarityTimingEstimated(string, string)"/>
        public abstract double GetSimilarityTimingEstimated(string firstWord, string secondWord);

        /// <see cref="IStringMetric.GetUnnormalizedSimilarity(string, string)"/>
        public abstract double GetUnnormalizedSimilarity(string firstWord, string secondWord);
    }
}
