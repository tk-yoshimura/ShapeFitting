using System;

namespace ShapeFitting {
    internal static class Solver {
        public static (double theta, double phi)
            FitLine(double sw,
                    double swx, double swy,
                    double swx2, double swxy, double swy2) {

            double u = swx2 - swy2 - (swx * swx - swy * swy) / sw;
            double v = 2 * (swxy - swx * swy / sw);

            double theta = -Math.Atan2(v, u) / 2;
            double phi = -(Math.Sin(theta) * swx + Math.Cos(theta) * swy) / sw;

            return (theta, phi);
        }

        public static (double a, double b, double c)
            FitCircle(double sw,
                      double swx, double swy,
                      double swx2, double swxy, double swy2,
                      double swx3, double swx2y, double swxy2, double swy3) {

            AlgebraD3.SymmMatrix s = new(swx2, swy2, sw, swxy, swy, swx);
            AlgebraD3.Vector v = new(-swxy2 - swx3, -swx2y - swy3, -swx2 - swy2);

            (double a, double b, double c) = AlgebraD3.Mul(AlgebraD3.Invert(s), v);

            return (a, b, c);
        }

        public static (double a, double b, double c, double d, double e, double f)
            FitEllipse(double sw,
                       double swx, double swy,
                       double swx2, double swxy, double swy2,
                       double swx2y, double swxy2, double swx3, double swy3,
                       double swx4, double swx3y, double swx2y2, double swxy3, double swy4) {

            AlgebraD3.SymmMatrix s1 = new(swx4, swx2y2, swy4, swx3y, swxy3, swx2y2);
            AlgebraD3.SymmMatrix s3 = new(swx2, swy2, sw, swxy, swy, swx);
            AlgebraD3.Matrix s2 = new(swx3, swx2y, swx2, swx2y, swxy2, swxy, swxy2, swy3, swy2);

            AlgebraD3.Matrix t = AlgebraD3.Mul(AlgebraD3.Invert(s3), AlgebraD3.Transpose(s2));

            (double r1, double r4, double r6,
             _, double r2, double r5,
             _, _, double r3) = AlgebraD3.Mul(s2, t);

            (double p1, double p2, double p3, double p4, double p5, double p6) =
                AlgebraD3.Sub(s1, new AlgebraD3.SymmMatrix(r1, r2, r3, r4, r5, r6));

            AlgebraD3.Matrix m = new(p6 / 2, p5 / 2, p3 / 2, -p4, -p2, -p5, p1 / 2, p4 / 2, p6 / 2);


            throw new NotImplementedException();
        }
    }
}
