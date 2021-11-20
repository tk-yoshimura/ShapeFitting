using System.Collections.Generic;

namespace ShapeFitting {
    public interface IWeightComputable {
        public (double[] ws, double scale) Weight(IReadOnlyList<double> errs);
    }
}
