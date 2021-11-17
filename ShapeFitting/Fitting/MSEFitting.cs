using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    public static class MSEFitting {
        public static Line FitLine(IEnumerable<Vector> vs) {
            int n = vs.Count();

            if (n < 2) {
                return Line.NaN;
            }

            (double sx, double sy,
             double sx2, double sxy, double sy2) = Summator.D2(vs);

            Line line = Solver.FitLine(
                n,
                sx, sy,
                sx2, sxy, sy2
            );

            return line;
        }

        public static Circle FitCircle(IEnumerable<Vector> vs) {
            int n = vs.Count();

            if (n < 3) {
                return Circle.NaN;
            }

            (double sx, double sy,
             double sx2, double sxy, double sy2,
             double sx3, double sx2y, double sxy2, double sy3) = Summator.D3(vs);

            (double a, double b, double c) = Solver.FitCircle(
                n,
                sx, sy,
                sx2, sxy, sy2,
                sx3, sx2y, sxy2, sy3
            );

            return Circle.FromImplicit(a, b, c);
        }

        public static Ellipse FitEllipse(IEnumerable<Vector> vs) {
            int n = vs.Count();

            if (n < 5) {
                return (Ellipse)FitCircle(vs);
            }

            (double sx, double sy,
             double sx2, double sxy, double sy2,
             double sx3, double sx2y, double sxy2, double sy3,
             double sx4, double sx3y, double sx2y2, double sxy3, double sy4) = Summator.D4(vs);

            (double a, double b, double c, double d, double e, double f) = Solver.FitEllipse(
                n,
                sx, sy,
                sx2, sxy, sy2,
                sx3, sx2y, sxy2, sy3,
                sx4, sx3y, sx2y2, sxy3, sy4
            );

            return Ellipse.FromImplicit(a, b, c, d, e, f);
        }
    }
}
