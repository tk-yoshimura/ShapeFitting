using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    public class TukeyMAD : IWeightComputable {
        readonly double c, min_scale;

        public TukeyMAD(double c = 3.160, double min_scale = 0) {
            if (!(c > 0)) {
                throw new ArgumentOutOfRangeException(nameof(c));
            }
            if (!(min_scale >= 0)) {
                throw new ArgumentOutOfRangeException(nameof(min_scale));
            }

            this.c = c;
            this.min_scale = min_scale;
        }

        public (IEnumerable<double> ws, double scale) Weight(IEnumerable<double> errs) {
            double mad = errs.MedianAbsoluteDeviation();
            double scale = Math.Max(min_scale, c * mad);

            return (WeightFunc.Tukey(errs, scale), scale);
        }
    }

    public class TukeyAAD : IWeightComputable {
        readonly double c, min_scale;

        public TukeyAAD(double c = 3.738, double min_scale = 0) {
            if (!(c > 0)) {
                throw new ArgumentOutOfRangeException(nameof(c));
            }
            if (!(min_scale >= 0)) {
                throw new ArgumentOutOfRangeException(nameof(min_scale));
            }

            this.c = c;
            this.min_scale = min_scale;
        }

        public (IEnumerable<double> ws, double scale) Weight(IEnumerable<double> errs) {
            double aad = errs.AverageAbsoluteDeviation();
            double scale = Math.Max(min_scale, c * aad);

            return (WeightFunc.Tukey(errs, scale), scale);
        }
    }

    public class HuberMAD : IWeightComputable {
        readonly double k, min_scale;

        public HuberMAD(double k = 0.907, double min_scale = 0) {
            if (!(k > 0)) {
                throw new ArgumentOutOfRangeException(nameof(k));
            }
            if (!(min_scale >= 0)) {
                throw new ArgumentOutOfRangeException(nameof(min_scale));
            }

            this.k = k;
            this.min_scale = min_scale;
        }

        public (IEnumerable<double> ws, double scale) Weight(IEnumerable<double> errs) {
            double mad = errs.MedianAbsoluteDeviation();
            double scale = Math.Max(min_scale, k * mad);

            return (WeightFunc.Huber(errs, scale), scale);
        }
    }

    public class HuberAAD : IWeightComputable {
        readonly double k, min_scale;

        public HuberAAD(double k = 1.073, double min_scale = 0) {
            if (!(k > 0)) {
                throw new ArgumentOutOfRangeException(nameof(k));
            }
            if (!(min_scale >= 0)) {
                throw new ArgumentOutOfRangeException(nameof(min_scale));
            }

            this.k = k;
            this.min_scale = min_scale;
        }

        public (IEnumerable<double> ws, double scale) Weight(IEnumerable<double> errs) {
            double aad = errs.AverageAbsoluteDeviation();
            double scale = Math.Max(min_scale, k * aad);

            return (WeightFunc.Huber(errs, scale), scale);
        }
    }

    internal class Lasso : IWeightComputable {
        public Lasso() { }

        public (IEnumerable<double> ws, double scale) Weight(IEnumerable<double> errs) {
            double k = Math.Max(errs.Min(), errs.Max() * 1e-3);

            return (WeightFunc.Huber(errs, k), double.NaN);
        }
    }
}
