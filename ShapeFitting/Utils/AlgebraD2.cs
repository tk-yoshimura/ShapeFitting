using System;
using System.Numerics;

namespace ShapeFitting {
    internal static class AlgebraD2 {
        public class SymmMatrix {
            private readonly double m1, m2, m3;

            public SymmMatrix(double m1, double m2, double m3) {

                this.m1 = m1;
                this.m2 = m2;
                this.m3 = m3;
            }

            public void Deconstruct(out double m1, out double m2, out double m3) =>
                (m1, m2, m3) = (this.m1, this.m2, this.m3);
        };

        public class Matrix {
            private readonly double m11, m12, m21, m22;

            public Matrix(double m11, double m12, double m21, double m22) {
                this.m11 = m11;
                this.m12 = m12;
                this.m21 = m21;
                this.m22 = m22;
            }

            public void Deconstruct(out double m11, out double m12, out double m21, out double m22) =>
                (m11, m12, m21, m22) = (this.m11, this.m12, this.m21, this.m22);
        };

        public class Vector {
            private readonly double e1, e2;

            public Vector(double e1, double e2) {
                this.e1 = e1;
                this.e2 = e2;
            }

            public void Deconstruct(out double e1, out double e2) =>
                (e1, e2) = (this.e1, this.e2);
        };

        public static double Det(SymmMatrix mat) {
            (double m1, double m2, double m3) = mat;

            double det = m1 * m2 - m3 * m3;

            return det;
        }

        public static SymmMatrix Invert(SymmMatrix mat) {
            (double m1, double m2, double m3) = mat;

            double d = 1 / Det(mat);

            double n1 = m2 * d;
            double n2 = m1 * d;
            double n3 = -m3 * d;

            return new SymmMatrix(n1, n2, n3);
        }

        public static SymmMatrix Add(SymmMatrix mat1, SymmMatrix mat2) {
            (double m1, double m2, double m3) = mat1;
            (double n1, double n2, double n3) = mat2;

            return new SymmMatrix(m1 + n1, m2 + n2, m3 + n3);
        }

        public static SymmMatrix Sub(SymmMatrix mat1, SymmMatrix mat2) {
            (double m1, double m2, double m3) = mat1;
            (double n1, double n2, double n3) = mat2;

            return new SymmMatrix(m1 - n1, m2 - n2, m3 - n3);
        }

        public static SymmMatrix Mul(SymmMatrix mat1, SymmMatrix mat2) {
            (double m1, double m2, double m3) = mat1;
            (double n1, double n2, double n3) = mat2;

            double r1 = m1 * n1 + m3 * n3;
            double r2 = m3 * n3 + m2 * n2;
            double r3 = m1 * n3 + m3 * n2;

            return new SymmMatrix(r1, r2, r3);
        }

        public static Matrix Mul(SymmMatrix mat1, Matrix mat2) {
            (double m1, double m2, double m3) = mat1;
            (double n11, double n12,
             double n21, double n22) = mat2;

            double r11 = m1 * n11 + m3 * n21;
            double r12 = m1 * n12 + m3 * n22;
            double r21 = m2 * n21 + m3 * n11;
            double r22 = m2 * n22 + m3 * n12;

            return new Matrix(r11, r12, r21, r22);
        }

        public static Matrix Mul(Matrix mat1, SymmMatrix mat2) {
            (double n11, double n12,
             double n21, double n22) = mat1;
            (double m1, double m2, double m3) = mat2;

            double r11 = m1 * n11 + m3 * n12;
            double r21 = m1 * n21 + m3 * n22;
            double r12 = m2 * n12 + m3 * n11;
            double r22 = m2 * n22 + m3 * n21;

            return new Matrix(r11, r12, r21, r22);
        }

        public static SymmMatrix SqueezeMul(Matrix mat1, SymmMatrix mat2) {
            (double n11, double n12,
             double n21, double n22) = mat1;
            (double r11, double r12,
             double r21, double r22) = Mul(mat1, mat2);

            double r1 = n11 * r11 + n12 * r12;
            double r2 = n21 * r21 + n22 * r22;
            double r3 = n11 * r21 + n12 * r22;

            return new SymmMatrix(r1, r2, r3);
        }

        public static Vector Mul(Matrix mat, Vector v) {
            (double n11, double n12,
             double n21, double n22) = mat;
            (double e1, double e2) = v;

            double u1 = n11 * e1 + n12 * e2;
            double u2 = n21 * e1 + n22 * e2;

            return new Vector(u1, u2);
        }

        public static Vector Mul(SymmMatrix mat, Vector v) {
            (double m1, double m2, double m3) = mat;
            (double e1, double e2) = v;

            double u1 = m1 * e1 + m3 * e2;
            double u2 = m3 * e1 + m2 * e2;

            return new Vector(u1, u2);
        }

        public static ((double val, Vector vec) l1, (double val, Vector vec) l2) EigenValues(SymmMatrix mat, double eps = 1e-10) {
            (double m1, double m2, double m3) = mat;

            if (Math.Abs(m3) < eps) {
                return Order.AbsSort((m1, new Vector(1, 0)), (m2, new Vector(0, 1)));
            }

            (Complex x1, Complex x2) = RootFinding.Quadratic(
                -m1 - m2,
                m1 * m2 - m3 * m3
            );

            static Vector normalize(Vector v) {
                (double x, double y) = v;

                double s = x > 0 ? 1 : x < 0 ? -1
                         : y >= 0 ? 1 : -1;
                double n = s * Math.Max(Math.Abs(x), Math.Abs(y));

                return new Vector(x / n, y / n);
            }

            Vector eigenvector(double l) {
                double n1 = l - m1, n2 = l - m2, n3 = -m3;

                if (Math.Abs(n1) < eps && Math.Abs(n3) < eps) {
                    return new Vector(1, 0);
                }
                if (Math.Abs(n2) < eps && Math.Abs(n3) < eps) {
                    return new Vector(0, 1);
                }

                double rx = n3 - n2;
                double ry = n3 - n1;

                return normalize(new Vector(rx, ry));
            }

            // M = transpose(M) => lambda in R (det lambda I - M = 0)
            (double l1, double l2) = Order.AbsSort(x1.Real, x2.Real);

            return ((l1, eigenvector(l1)), (l2, eigenvector(l2)));
        }
    }
}
