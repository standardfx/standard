using System;
using Standard.Core;

namespace Standard
{
    partial class NumberExtension
    {
        /// <summary>
        /// Checks whether two numbers are almost equals, taking into account the magnitude of floating point 
        /// numbers (unlike <see cref="WithinEpsilon(float, float, float)"/> method).
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <param name="maxUlp">The maximum number of floating point values between <paramref name="a"/> and <paramref name="b"/> to be considered nearly equal.</param>
        /// <returns>
        /// `true` if <paramref name="a"/> is almost equal to <paramref name="b"/>; otherwise, `false`.
        /// </returns>
        /// <remarks>
        /// The code is using the technique described by Bruce Dawson in 
        /// [http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/](Comparing Floating point numbers 2012 edition). 
        /// </remarks>
        public unsafe static bool NearEquals(this float a, float b, int maxUlp = 4)
        {
            // Choose of maxUlp = 4
            // according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h

            // Check if the numbers are really close -- needed
            // when comparing numbers near zero.
            if (NearZero(a - b))
                return true;

            // Original from Bruce Dawson: http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            // Different signs means they do not match.
            // #todo improve based on http://home.apache.org/~luc/commons-math-3.6-RC2-site/jacoco/org.apache.commons.math3.util/Precision.java.html
            if ((aInt < 0) != (bInt < 0))
                return false;

            // Find the difference in ULPs.
            int ulp = Math.Abs(aInt - bInt);
            return (ulp <= maxUlp);
        }

        /// <see cref="NearEquals(float, float, int)" />
        public unsafe static bool NearEquals(this double a, double b, int maxUlp = 4)
        {
            if (NearZero(a - b))
                return true;

            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            if ((aInt < 0) != (bInt < 0))
                return false;

            // Find the difference in ULPs.
            int ulp = Math.Abs(aInt - bInt);
            return (ulp <= maxUlp);
        }

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns>
        /// `true` if the specified value is close to zero (0.0f); otherwise, `false`.
        /// </returns>
        public static bool NearZero(this float a)
        {
            return Math.Abs(a) < MathUtility.ZeroTolerance;
        }

        /// <see cref="NearZero(float)" />
        public static bool NearZero(this double a)
        {
            return Math.Abs(a) < (double)(MathUtility.ZeroTolerance);
        }

        /// <summary>
        /// Determines whether the specified value is close to one (1.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns>
        /// `true` if the specified value is close to 1; otherwise, `false`.
        /// </returns>
        public static bool NearOne(this float a)
        {
            return NearZero(a - 1.0f);
        }

        /// <see cref="NearOne(float)" />
        public static bool NearOne(this double a)
        {
            return NearZero(a - 1.0d);
        }

        /// <summary>
        /// Checks whether the difference between two numbers are almost equals within an epsilon.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <param name="epsilon">The epsilon value. Note that a negative value will always return `false`.</param>
        /// <returns>
        /// `true` if <paramref name="a"/> is almost equal to <paramref name="b"/> within the <paramref name="epsilon"/>; 
        /// otherwise, `false`.
        /// </returns>
        public static bool WithinEpsilon(this float a, float b, float epsilon)
        {
            // silvia:
            // Return True only if abs(a-b) <= epsilon
            // NOTE: epsilon must >=0
            float delta = a - b;
            return ((-epsilon <= delta) && (delta <= epsilon));
        }

        /// <see cref="WithinEpsilon(float, float, float)" />
        public static bool WithinEpsilon(this double a, double b, double epsilon)
        {
            double delta = a - b;
            return ((-epsilon <= delta) && (delta <= epsilon));
        }
    }
}