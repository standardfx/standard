using System;

namespace Standard
{
    partial class MathUtility
    {
        //<#
        //  .INHERIT Lerp(byte, byte, float)
        //#>
        public static double Lerp(double from, double to, double amount)
        {
            return (1 - amount) * from + amount * to;
        }

        //<#
        //  .INHERIT Lerp(byte, byte, float)
        //#>
        public static float Lerp(float from, float to, float amount)
        {
            return (1 - amount) * from + amount * to;
        }

        //<#
        //  .SYNOPSIS
        //      Interpolates between two values using a linear function by a given amount.
        //
        //  .PARAM from
        //      Value to interpolate from.
        //
        //  .PARAM to
        //      Value to interpolate to.
        //
        //  .PARAM amount
        //      Interpolation amount.
        //
        //  .REMARKS
        //      See [http://www.encyclopediaofmath.org/index.php/Linear_interpolation](here) and 
        //      [http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future](here) 
        //      for an in-depth description of the interpolation algorithm.
        //#>
        public static byte Lerp(byte from, byte to, float amount)
        {
            return (byte)Lerp((float)from, (float)to, amount);
        }

        //<#
        //  .SYNOPSIS
        //      Performs smooth (cubic Hermite) interpolation between 0 and 1.
        //
        //  .PARAM amount
        //      Value between 0 and 1 indicating interpolation amount.
        //
        //  .REMARKS
        //      See [https://en.wikipedia.org/wiki/Smoothstep](wikipedia article) for an in-depth description of the smooth step algorithm.
        //#>
        public static float SmoothStep(float amount)
        {
            return SmoothStep(amount, false);
        }

        //<#
        //  .SYNOPSIS
        //      Performs smooth (cubic Hermite) interpolation between 0 and 1.
        //
        //  .PARAM amount
        //      Value between 0 and 1 indicating interpolation amount.
        //
        //  .PARAM highRes
        //      Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.
        //
        //  .REMARKS
        //      See [https://en.wikipedia.org/wiki/Smoothstep](wikipedia article) for an in-depth description of the smooth step algorithm.
        //#>
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