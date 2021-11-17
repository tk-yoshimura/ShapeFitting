using System.Collections.Generic;

namespace ShapeFitting {
    public class TukeyMAD : IWeightComputable {
        readonly double c;

        public TukeyMAD(double c = 3.160) {
            this.c = c;
        }

        public IEnumerable<double> Weight(IEnumerable<double> errs) {
            double mad = errs.MedianAbsoluteDeviation();

            return WeightFunc.Tukey(errs, c * mad);
        }
    }

    public class TukeyAAD : IWeightComputable {
        readonly double c;

        public TukeyAAD(double c = 3.738) {
            this.c = c;
        }

        public IEnumerable<double> Weight(IEnumerable<double> errs) {
            double aad = errs.AverageAbsoluteDeviation();

            return WeightFunc.Tukey(errs, c * aad);
        }
    }

    public class HuberMAD : IWeightComputable {
        readonly double k;

        public HuberMAD(double k = 0.907) {
            this.k = k;
        }

        public IEnumerable<double> Weight(IEnumerable<double> errs) {
            double mad = errs.MedianAbsoluteDeviation();

            return WeightFunc.Huber(errs, k * mad);
        }
    }

    public class HuberAAD : IWeightComputable {
        readonly double k;

        public HuberAAD(double k = 1.073) {
            this.k = k;
        }

        public IEnumerable<double> Weight(IEnumerable<double> errs) {
            double aad = errs.AverageAbsoluteDeviation();

            return WeightFunc.Huber(errs, k * aad);
        }
    }

    internal class Lasso : IWeightComputable {
        public Lasso() { }

        public IEnumerable<double> Weight(IEnumerable<double> errs) {
            return WeightFunc.Lasso(errs);
        }
    }
}
