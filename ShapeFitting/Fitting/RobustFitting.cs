using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    public static class RobustFitting {
        public static Line FitLine(IEnumerable<Vector> vs, IWeightComputable weight_rule, int iters = 16, double toi = 1e-4) {
            if (iters <= 1) {
                throw new ArgumentOutOfRangeException(nameof(iters));
            }
            if (!(toi > 0)) {
                throw new ArgumentOutOfRangeException(nameof(toi));
            }

            int n = vs.Count();

            if (n < 2) {
                return Line.NaN;
            }

            (double sx, double sy,
             double sx2, double sxy, double sy2) = Summator.D2(vs);

            (double a, double b, double c) = (Line)Solver.FitLine(
                n,
                sx, sy,
                sx2, sxy, sy2
            );

            double scale = double.NaN;

#if DEBUG
            Trace.WriteLine($"{(a, b, c)}");
#endif

            for (int iter = 0; iter < iters; iter++) {
                List<double> errs = new();

                foreach ((double x, double y) in vs) {
                    double err = Math.Abs(a * x + b * y + c);
                    errs.Add(err);
                }

                (IEnumerable<double> weights, double new_scale) = weight_rule.Weight(errs);

                (double sw,
                 double swx, double swy,
                 double swx2, double swxy, double swy2) = Summator.D2(vs, weights);

                (double new_a, double new_b, double new_c) = (Line)Solver.FitLine(
                    sw,
                    swx, swy,
                    swx2, swxy, swy2
                );

                if (new_scale < scale && scale - new_scale < toi) {
#if DEBUG
                    Trace.WriteLine($"breakiter {iter}");
#endif
                    iter = iters;
                }
                else if (new_scale > scale || double.IsNaN(new_a) || double.IsNaN(new_b) || double.IsNaN(new_c)) {
#if DEBUG
                    Trace.WriteLine($"breakiter {iter}");
#endif
                    break;
                }

                (a, b, c) = (new_a, new_b, new_c);
                scale = new_scale;

#if DEBUG
                Trace.WriteLine($"{(a, b, c)}");
#endif
            }

            return new Line(a, b, c);
        }

        public static Circle FitCircle(IEnumerable<Vector> vs, IWeightComputable weight_rule, int iters = 16, double toi = 1e-4) {
            if (iters <= 1) {
                throw new ArgumentOutOfRangeException(nameof(iters));
            }
            if (!(toi > 0)) {
                throw new ArgumentOutOfRangeException(nameof(toi));
            }

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

            double scale = double.NaN;

#if DEBUG
            Trace.WriteLine($"{(a, b, c)}");
#endif

            for (int iter = 0; iter < iters; iter++) {
                List<double> errs = new();

                foreach ((double x, double y) in vs) {
                    double err = Math.Abs(x * x + y * y + a * x + b * y + c);
                    errs.Add(err);
                }

                (IEnumerable<double> weights, double new_scale) = weight_rule.Weight(errs);

                (double sw,
                 double swx, double swy,
                 double swx2, double swxy, double swy2,
                 double swx3, double swx2y, double swxy2, double swy3) = Summator.D3(vs, weights);

                (double new_a, double new_b, double new_c) = Solver.FitCircle(
                    sw,
                    swx, swy,
                    swx2, swxy, swy2,
                    swx3, swx2y, swxy2, swy3
                );

                if (new_scale < scale && scale - new_scale < toi) {
#if DEBUG
                    Trace.WriteLine($"breakiter {iter}");
#endif
                    iter = iters;
                }
                else if (new_scale > scale || double.IsNaN(new_a) || double.IsNaN(new_b) || double.IsNaN(new_c)) {
#if DEBUG
                    Trace.WriteLine($"breakiter {iter}");
#endif
                    break;
                }

                (a, b, c) = (new_a, new_b, new_c);
                scale = new_scale;

#if DEBUG
                Trace.WriteLine($"{(a, b, c)}");
#endif
            }

            return Circle.FromImplicit(a, b, c);
        }

        public static Ellipse FitEllipse(IEnumerable<Vector> vs, IWeightComputable weight_rule, int iters = 16, double toi = 1e-4) {
            if (iters <= 1) {
                throw new ArgumentOutOfRangeException(nameof(iters));
            }
            if (!(toi > 0)) {
                throw new ArgumentOutOfRangeException(nameof(toi));
            }

            int n = vs.Count();

            if (n < 5) {
                return (Ellipse)FitCircle(vs, weight_rule);
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

            double scale = double.NaN;

#if DEBUG
            Trace.WriteLine($"{(a, b, c, d, e, f)}");
#endif

            for (int iter = 0; iter < iters; iter++) {
                List<double> errs = new();

                foreach ((double x, double y) in vs) {
                    double err = Math.Abs(a * x * x + b * x * y + c * y * y + d * x + e * y + f);
                    errs.Add(err);
                }

                (IEnumerable<double> weights, double new_scale) = weight_rule.Weight(errs);

                (double sw,
                 double swx, double swy,
                 double swx2, double swxy, double swy2,
                 double swx3, double swx2y, double swxy2, double swy3,
                 double swx4, double swx3y, double swx2y2, double swxy3, double swy4) = Summator.D4(vs, weights);

                (double new_a, double new_b, double new_c, double new_d, double new_e, double new_f) = Solver.FitEllipse(
                    sw,
                    swx, swy,
                    swx2, swxy, swy2,
                    swx3, swx2y, swxy2, swy3,
                    swx4, swx3y, swx2y2, swxy3, swy4
                );

                if (new_scale < scale && scale - new_scale < toi) {
#if DEBUG
                    Trace.WriteLine($"breakiter {iter}");
#endif
                    iter = iters;
                }
                else if (new_scale > scale ||
                         double.IsNaN(new_a) || double.IsNaN(new_b) || double.IsNaN(new_c) ||
                         double.IsNaN(new_d) || double.IsNaN(new_e) || double.IsNaN(new_f)) {
#if DEBUG
                    Trace.WriteLine($"breakiter {iter}");
#endif

                    break;
                }

                (a, b, c, d, e, f) = (new_a, new_b, new_c, new_d, new_e, new_f);
                scale = new_scale;

#if DEBUG
                Trace.WriteLine($"{(a, b, c, d, e, f)}");
#endif
            }

            return Ellipse.FromImplicit(a, b, c, d, e, f);
        }
    }
}
