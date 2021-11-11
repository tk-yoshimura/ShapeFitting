using System;
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

            double sx = 0, sy = 0, sxy = 0, sxx = 0, syy = 0;
            
            foreach((double x, double y) in vs) {
                sx += x;
                sy += y;
                sxy += x * y;
                sxx += x * x;
                syy += y * y;
            }

            double u = sxx - syy - (sx * sx - sy * sy) / n;
            double v = 2 * (sxy - sx * sy / n);

            double theta = -Math.Atan2(v, u) / 2;
            double phi = -(Math.Sin(theta) * sx + Math.Cos(theta) * sy) / n;

            return new Line(theta, phi);
        } 
    }
}
