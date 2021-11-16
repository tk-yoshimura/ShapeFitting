using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    public static class WeightedFitting {
        public static Line FitLine(IEnumerable<Vector> vs, IEnumerable<double> weights) {
            if (vs.Count() != weights.Count()) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            int n = vs.Count();

            if (n < 2) {
                return Line.NaN;
            }
            if (n == 2) {
                return Line.FromPoints(vs.First(), vs.Last());
            }

            double sw = 0, swx = 0, swy = 0, swx2 = 0, swxy = 0, swy2 = 0;

            foreach (((double x, double y), double w) in vs.Zip(weights)) {
                sw += w;
                swx += w * x;
                swy += w * y;
                swx2 += w * x * x;
                swxy += w * x * y;
                swy2 += w * y * y;
            }

            Line line = Solver.FitLine(sw, swx, swy, swx2, swxy, swy2);

            return line;
        }
    }
}
