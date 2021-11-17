using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    public static class WeightFunc {
        public static IEnumerable<double> Tukey(IEnumerable<double> errs, double c) {
            if (!(c >= 0)) {
                throw new ArgumentOutOfRangeException(nameof(c));
            }

            c += double.Epsilon;

            foreach (double err in errs) {
#if DEBUG
                if (!(err >= 0)) {
                    throw new ArgumentOutOfRangeException(nameof(errs));
                }
#endif

                if (err > c) {
                    yield return 0;
                    continue;
                }

                double n = err / c, m = 1 - n * n, w = m * m;
                yield return w;
            }
        }

        public static IEnumerable<double> Huber(IEnumerable<double> errs, double k) {
            if (!(k >= 0)) {
                throw new ArgumentOutOfRangeException(nameof(k));
            }

            k += double.Epsilon;

            foreach (double err in errs) {
#if DEBUG
                if (!(err >= 0)) {
                    throw new ArgumentOutOfRangeException(nameof(errs));
                }
#endif

                if (err <= k) {
                    yield return 1;
                    continue;
                }

                double w = k / err;

                yield return w;
            }
        }

        public static IEnumerable<double> Lasso(IEnumerable<double> errs) {
            double k = Math.Max(errs.Min(), errs.Max() * 1e-3);

            return Huber(errs, k);
        }
    }
}
