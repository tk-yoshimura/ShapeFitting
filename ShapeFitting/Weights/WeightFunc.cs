using System;
using System.Collections.Generic;

namespace ShapeFitting {
    public static class WeightFunc {
        public static double[] Tukey(IReadOnlyList<double> errs, double c) {
            c += double.Epsilon;

            double[] ws = new double[errs.Count];

            for (int i = 0; i < errs.Count; i++) {
                double err = errs[i];
#if DEBUG
                if (!(err >= 0)) {
                    throw new ArgumentOutOfRangeException(nameof(errs));
                }
#endif

                double n = err / c, m = Math.Max(0, 1 - n * n), w = m * m;
                ws[i] = w;
            }

            return ws;
        }

        public static double[] Huber(IReadOnlyList<double> errs, double k) {
            k += double.Epsilon;

            double[] ws = new double[errs.Count];

            for (int i = 0; i < errs.Count; i++) {
                double err = errs[i];
#if DEBUG
                if (!(err >= 0)) {
                    throw new ArgumentOutOfRangeException(nameof(errs));
                }
#endif

                double w = Math.Min(1, k / err);
                ws[i] = w;
            }

            return ws;
        }
    }
}
