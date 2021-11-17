using System.Collections.Generic;

namespace ShapeFitting {
    public interface IWeightComputable {
        public IEnumerable<double> Weight(IEnumerable<double> errs);
    }
}
