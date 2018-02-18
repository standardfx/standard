using System;
using System.Collections.Generic;

namespace Standard
{
    public static class RandomUtility
    {
        /// <summary>
        /// Calculates the entropy of the specified input.
        /// </summary>
        /// <param name="sample">The data to evaluate.</param>
        /// <returns>Entropy score. Higher score means more randomness.</returns>
        public static double GetEntropy(byte[] sample)
        {
            Dictionary<byte, double> frequencyTable = new Dictionary<byte, double>();
            int totalCount = 0;

            foreach (byte b in sample)
            {
                double currentValueCount;
                if (frequencyTable.TryGetValue(b, out currentValueCount))
                    frequencyTable[b] = ++currentValueCount;
                else
                    frequencyTable.Add(b, 1);

                ++totalCount;
            }

            double entropy = 0;
            foreach (var item in frequencyTable)
            {
                double p = item.Value / totalCount;
                entropy += p * Math.Log(p, 2);
            }

            return -entropy;
        }
    }   
}
