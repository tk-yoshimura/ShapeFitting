using System.Collections.Generic;

namespace ShapeFitting {
    public static class MAEFitting {
        public static Line FitLine(IReadOnlyList<Vector> vs, int iters = 16, double tolerance = 1e-8) {
            return RobustFitting.FitLine(vs, new Lasso(), iters, tolerance);
        }

        public static Circle FitCircle(IReadOnlyList<Vector> vs, int iters = 16, double tolerance = 1e-8) {
            return RobustFitting.FitCircle(vs, new Lasso(), iters, tolerance);
        }

        public static Ellipse FitEllipse(IReadOnlyList<Vector> vs, int iters = 16, double tolerance = 1e-8) {
            return RobustFitting.FitEllipse(vs, new Lasso(), iters, tolerance);
        }
    }
}
