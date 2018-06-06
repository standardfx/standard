using System;
using Standard;
using Standard.Core;

namespace Standard
{
    public static partial class NumberExtension
    {
        /// <summary>
        /// Ensures that the specified value does not exceed a given range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value permitted.</param>
        /// <param name="max">The maximum value permitted.</param>
        /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        /// <returns>The result of clamping a value between the minimum and maximum values specified.</returns>
        public static float Clamp(this float value, float min, float max)
        {
            if (min > max)
                throw new ArgumentException(string.Format(RS.Err_MinGtMax, min, max), nameof(min));

            return value < min ? min 
                : value > max ? max 
                : value;
        }

        /// <summary>
        /// Ensures that the specified value does not exceed a given range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        /// <returns>The result of clamping a value between the minimum and maximum values specified.</returns>
        public static int Clamp(this int value, int min, int max)
        {
            if (min > max)
                throw new ArgumentException(string.Format(RS.Err_MinGtMax, min, max), nameof(min));

            return value < min ? min 
                : value > max ? max 
                : value;
        }

        /// <summary>
        /// Calculates the modulo of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulo">The modulo.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static float Mod(this float value, float modulo)
        {
            if (modulo == 0.0f)
                return value;

            return value % modulo;
        }

        /// <summary>
        /// Calculates the modulo 2*PI of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static float Mod2PI(this float value)
        {
            return Mod(value, MathUtility.TwoPI);
        }

        /// <summary>
        /// Wraps the specified value into a range "[min, max]".
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        /// <returns>Result of the wrapping.</returns>
        /// <remarks>
        /// <para>Any number that is not between the given range 'wraps' over to the other side and starts decrementing/incrementing. The wrapped value cannot exceed the given range.</para>
        /// <code><![CDATA[
        /// Console.WriteLine("Wrapping between -3 to 3:\n");
        /// Console.WriteLine("Value | Int32 | Single");
        /// Console.WriteLine("----------------------");
        /// for (int i = -7; i < 7; i++)
        /// {
        ///     Console.WriteLine("{0,5} | {1,5} | {2,5}", i, i.Wrap(-3, 3), ((float)i).Wrap(-3, 3));
        /// }
        ///
        /// // Wrapping between -3 to 3:
        /// // 
        /// // Value | Int32 | Single
        /// // ----------------------
        /// //    -7 |     0 |     -1
        /// //    -6 |     1 |      0
        /// //    -5 |     2 |      1
        /// //    -4 |     3 |      2
        /// //    -3 |    -3 |     -3
        /// //    -2 |    -2 |     -2
        /// //    -1 |    -1 |     -1
        /// //     0 |     0 |      0
        /// //     1 |     1 |      1
        /// //     2 |     2 |      2
        /// //     3 |     3 |     -3
        /// //     4 |    -3 |     -2
        /// //     5 |    -2 |     -1
        /// //     6 |    -1 |      0
        /// ]]></code>
        /// </remarks>
        public static int Wrap(this int value, int min, int max)
        {
            if (min > max)
                throw new ArgumentException(string.Format(RS.Err_MinGtMax, min, max), nameof(min));

            // Code from http://stackoverflow.com/a/707426/1356325
            int rangeSize = max - min + 1;

            if (value < min)
                value += rangeSize * ((min - value) / rangeSize + 1);

            return min + (value - min) % rangeSize;
        }

        /// <summary>
        /// Wraps the specified value into a range "[min, max[".
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <exception cref="ArgumentException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        /// <returns>Result of the wrapping.</returns>
        /// <remarks>
        /// As opposed to wrapping integers, a wrapped floating point number that is equal to its upper range will be "wrapped" to its lower range value. 
        /// </remarks>
        public static float Wrap(this float value, float min, float max)
        {
            if (NearEquals(min, max)) 
                return min;

            double mind = min;
            double maxd = max;
            double valued = value;

            if (mind > maxd)
                throw new ArgumentException(string.Format(RS.Err_MinGtMax, min, max), nameof(min));

            double rangeSize = maxd - mind;
            return (float)(mind + (valued - mind) - (rangeSize * Math.Floor((valued - mind) / rangeSize)));
        }
    }
}