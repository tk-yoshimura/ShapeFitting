using System.Collections.Generic;

namespace ShapeFitting {
    public static class MAEFitting {
        public static Line FitLine(IReadOnlyList<Vector> vs, int iters = 16, double toi = 1e-8) {
            return RobustFitting.FitLine(vs, new Lasso(), iters, toi);
        }

        public static Circle FitCircle(IReadOnlyList<Vector> vs, int iters = 16, double toi = 1e-8) {
            return RobustFitting.FitCircle(vs, new Lasso(), iters, toi);
        }

        public static Ellipse FitEllipse(IReadOnlyList<Vector> vs, int iters = 16, double toi = 1e-8) {
            return RobustFitting.FitEllipse(vs, new Lasso(), iters, toi);
        }
    }
}
