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

            double sw = 0, swx = 0, swy = 0, swxy = 0, swxx = 0, swyy = 0;

            foreach (((double x, double y), double w) in vs.Zip(weights)) {
                sw += w;
                swx += w * x;
                swy += w * y;
                swxy += w * x * y;
                swxx += w * x * x;
                swyy += w * y * y;
            }

            double u = swxx - swyy - (swx * swx - swy * swy) / sw;
            double v = 2 * (swxy - swx * swy / sw);

            double theta = -Math.Atan2(v, u) / 2;
            double phi = -(Math.Sin(theta) * swx + Math.Cos(theta) * swy) / sw;

            return new Line(theta, phi);
        }
    }
}
