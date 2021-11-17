using System;
using System.Numerics;

namespace ShapeFitting {
    internal static class RootFinding {
        /// <summary>solve x^2 + a1 x + a0 = 0</summary>
        public static (Complex x1, Complex x2) Quadratic(double a1, double a0) {
            Complex delta = Complex.Sqrt(a1 * a1 - 4 * a0);

            Complex x1 = -(a1 + delta) / 2;
            Complex x2 = -(a1 - delta) / 2;

            return (x1, x2);
        }

        /// <summary>solve x, x^3 + a2 x^2 + a1 x + a0 = 0</summary>
        public static (Complex x1, Complex x2, Complex x3) Cubic(double a2, double a1, double a0) {
            static Complex cbrt(Complex c) =>
                Complex.FromPolarCoordinates(Math.Cbrt(c.Magnitude), c.Phase / 3.0);
            double sqrt3 = Math.Sqrt(3), cbrt2 = Math.Cbrt(2);
            Complex omega1 = new(1d, -sqrt3), omega2 = new(1d, sqrt3);

            double p = 3 * a1 - a2 * a2;
            double q = -2 * a2 * a2 * a2 + 9 * a1 * a2 - 27 * a0;
            double c = -a2 / 3;
            double d1 = 1 / (6 * cbrt2), d2 = p / (3 * cbrt2 * cbrt2);

            Complex n = cbrt(q + Complex.Sqrt(4 * p * p * p + q * q));

            Complex x1 = c + n / (3 * cbrt2) - (cbrt2 * p / 3) / n;
            Complex x2 = c - omega2 * n * d1 + omega1 / n * d2;
            Complex x3 = c - omega1 * n * d1 + omega2 / n * d2;

            return (x1, x2, x3);
        }
    }
}
