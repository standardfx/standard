using System;

namespace Standard
{
    public static partial class MathUtility
    {
        //<#
        //  .SYNOPSIS
        //      An implementation of [http://en.wikipedia.org/wiki/Gaussian_function#Two-dimensional_Gaussian_function](Gauss 2D function).
        //
        //  .PARAM amplitude
        //      Curve amplitude.
        //
        //  .PARAM x
        //      Position X-coordinate.
        //
        //  .PARAM y
        //      Position Y-coordinate.
        //
        //  .PARAM centerX
        //      X-coordinate of the center.
        //
        //  .PARAM centerY
        //      Y-coordinate of the center.
        //
        //  .PARAM sigmaX
        //      Curve sigma X.
        //
        //  .PARAM sigmaY
        //      Curve sigma Y.
        //
        // .OUTPUT
        //      Result of the Gauss function
        //#>
        public static float Gauss(float amplitude, float x, float y, float centerX, float centerY, float sigmaX, float sigmaY)
        {
            return (float)Gauss((double)amplitude, x, y, centerX, centerY, sigmaX, sigmaY);
        }

        //<#
        //  .INHERITS Gauss(float, float, float, float, float, float, float)
        //#>
        public static double Gauss(double amplitude, double x, double y, double centerX, double centerY, double sigmaX, double sigmaY)
        {
            double cx = x - centerX;
            double cy = y - centerY;

            double componentX = (cx * cx) / (2 * sigmaX * sigmaX);
            double componentY = (cy * cy) / (2 * sigmaY * sigmaY);

            return amplitude * Math.Exp(-(componentX + componentY));
        }       
    }
}
