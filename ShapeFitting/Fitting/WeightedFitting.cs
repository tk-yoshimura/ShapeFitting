using System.Collections.Generic;

namespace ShapeFitting {
    public static class WeightedFitting {
        public static Line FitLine(IReadOnlyList<Vector> vs, IReadOnlyList<double> weights) {
            int n = vs.Count;

            if (n < 2) {
                return Line.NaN;
            }

            (double sw,
             double swx, double swy,
             double swx2, double swxy, double swy2) = Summator.D2(vs, weights);

            Line line = Solver.FitLine(
                sw,
                swx, swy,
                swx2, swxy, swy2
            );

            return line;
        }

        public static Circle FitCircle(IReadOnlyList<Vector> vs, IReadOnlyList<double> weights) {
            int n = vs.Count;

            if (n < 3) {
                return Circle.NaN;
            }

            (double sw,
             double swx, double swy,
             double swx2, double swxy, double swy2,
             double swx3, double swx2y, double swxy2, double swy3) = Summator.D3(vs, weights);

            (double a, double b, double c) = Solver.FitCircle(
                sw,
                swx, swy,
                swx2, swxy, swy2,
                swx3, swx2y, swxy2, swy3
            );

            return Circle.FromImplicit(a, b, c);
        }

        public static Ellipse FitEllipse(IReadOnlyList<Vector> vs, IReadOnlyList<double> weights) {
            int n = vs.Count;

            if (n < 5) {
                return (Ellipse)FitCircle(vs, weights);
            }

            (double sw,
             double swx, double swy,
             double swx2, double swxy, double swy2,
             double swx3, double swx2y, double swxy2, double swy3,
             double swx4, double swx3y, double swx2y2, double swxy3, double swy4) = Summator.D4(vs, weights);

            (double a, double b, double c, double d, double e, double f) = Solver.FitEllipse(
                sw,
                swx, swy,
                swx2, swxy, swy2,
                swx3, swx2y, swxy2, swy3,
                swx4, swx3y, swx2y2, swxy3, swy4
            );

            return Ellipse.FromImplicit(a, b, c, d, e, f);
        }
    }
}
