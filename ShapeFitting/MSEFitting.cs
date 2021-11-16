using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    public static class MSEFitting {
        public static Line FitLine(IEnumerable<Vector> vs) {
            int n = vs.Count();

            if (n < 2) {
                return Line.NaN;
            }
            if (n == 2) {
                return Line.FromPoints(vs.First(), vs.Last());
            }

            double sx = 0, sy = 0, sx2 = 0, sxy = 0, sy2 = 0;

            foreach ((double x, double y) in vs) {
                sx += x;
                sy += y;
                sx2 += x * x;
                sxy += x * y;
                sy2 += y * y;
            }

            Line line = Solver.FitLine(n, sx, sy, sx2, sxy, sy2);

            return line;
        }
    }
}
