using System.Collections.Generic;

namespace ShapeFitting {
    public interface IWeightComputable {
        public (IEnumerable<double> ws, double scale) Weight(IEnumerable<double> errs);
    }
}
