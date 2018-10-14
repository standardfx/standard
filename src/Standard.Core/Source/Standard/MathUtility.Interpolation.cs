using System;

namespace Standard
{
    partial class MathUtility
    {
        /// <summary>
        /// @ref <see cref="Lerp(byte, byte, float)"/>
        /// </summary>
        public static double Lerp(double from, double to, double amount)
        {
            return (1 - amount) * from + amount * to;
        }

        /// <summary>
        /// @ref <see cref="Lerp(byte, byte, float)"/>
        /// </summary>
        public static float Lerp(float from, float to, float amount)
        {
            return (1 - amount) * from + amount * to;
        }

        /// <summary>
        /// Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <param name="from">Value to interpolate from.</param>
        /// <param name="to">Value to interpolate to.</param>
        /// <param name="amount">Interpolation amount.</param>
        /// <returns>
        /// The interpolated value.
        /// </returns>
        /// <remarks>
        /// See the following articles for an in-depth description of the interpolation algorithm:
        /// - [http://www.encyclopediaofmath.org/index.php/Linear_interpolation](encyclopediaofmath)
        /// - [http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future](background reading)
        /// </remarks>
        public static byte Lerp(byte from, byte to, float amount)
        {
            return (byte)Lerp((float)from, (float)to, amount);
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        /// <returns>
        /// Smooth (cubic Hermite) interpolation between 0 and 1.
        /// </returns>
        /// <remarks>
        /// See this [https://en.wikipedia.org/wiki/Smoothstep](wikipedia article) for an in-depth description of the smooth step algorithm.
        /// </remarks>
        public static float SmoothStep(float amount)
        {
            return SmoothStep(amount, false);
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        /// <param name="highRes">Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.</param>
        /// <returns>
        /// Smooth (cubic Hermite) interpolation between 0 and 1.
        /// </returns>
        /// <remarks>
        /// See this [https://en.wikipedia.org/wiki/Smoothstep](wikipedia article) for an in-depth description of the smooth step algorithm.
        /// </remarks>
        public static float SmoothStep(float amount, bool highRes)
        {
            if (!highRes)
            {
                return (amount <= 0) ? 0
                    : (amount >= 1) ? 1
                    : amount * amount * (3 - (2 * amount));
            }
            else
            {
                return (amount <= 0) ? 0
                    : (amount >= 1) ? 1
                    : amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
            }
        }
    }
}