using System;

namespace Standard
{
    public static partial class MathUtility
    {
        /// <summary>
        /// An implementation of [http://en.wikipedia.org/wiki/Gaussian_function#Two-dimensional_Gaussian_function](Gauss 2D function).
        /// </summary>
        /// <param name="amplitude">Curve amplitude.</param>
        /// <param name="x">Position X-coordinate.</param>
        /// <param name="y">Position Y-coordinate.</param>
        /// <param name="centerX">X-coordinate of the center.</param>
        /// <param name="centerY">Y-coordinate of the center.</param>
        /// <param name="sigmaX">Curve sigma X.</param>
        /// <param name="sigmaY">Curve sigma y.</param>
        /// <returns>
        /// Result of the Gauss function
        /// </returns>
        public static float Gauss(float amplitude, float x, float y, float centerX, float centerY, float sigmaX, float sigmaY)
        {
            return (float)Gauss((double)amplitude, x, y, centerX, centerY, sigmaX, sigmaY);
        }

        /// <summary>
        /// An implementation of [http://en.wikipedia.org/wiki/Gaussian_function#Two-dimensional_Gaussian_function](Gauss 2D function).
        /// </summary>
        /// <param name="amplitude">Curve amplitude.</param>
        /// <param name="x">Position X-coordinate.</param>
        /// <param name="y">Position Y-coordinate.</param>
        /// <param name="centerX">X-coordinate of the center.</param>
        /// <param name="centerY">Y-coordinate of the center.</param>
        /// <param name="sigmaX">Curve sigma X.</param>
        /// <param name="sigmaY">Curve sigma y.</param>
        /// <returns>
        /// Result of the Gauss function
        /// </returns>
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
