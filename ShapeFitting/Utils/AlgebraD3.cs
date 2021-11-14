using System;
using System.Numerics;

namespace ShapeFitting {
    internal static class AlgebraD3 {
        public class SymmMatrix {
            private readonly double m1, m2, m3, m4, m5, m6;

            public SymmMatrix(
                double m1, double m2, double m3,
                double m4, double m5,
                double m6) {

                this.m1 = m1;
                this.m2 = m2;
                this.m3 = m3;
                this.m4 = m4;
                this.m5 = m5;
                this.m6 = m6;
            }

            public void Deconstruct(
                out double m1, out double m2, out double m3,
                out double m4, out double m5,
                out double m6) =>

                (m1, m2, m3, m4, m5, m6)
                = (this.m1, this.m2, this.m3, this.m4, this.m5, this.m6);
        };

        public class Matrix {
            private readonly double m11, m12, m13, m21, m22, m23, m31, m32, m33;

            public Matrix(
                double m11, double m12, double m13,
                double m21, double m22, double m23,
                double m31, double m32, double m33) {

                this.m11 = m11;
                this.m12 = m12;
                this.m13 = m13;
                this.m21 = m21;
                this.m22 = m22;
                this.m23 = m23;
                this.m31 = m31;
                this.m32 = m32;
                this.m33 = m33;
            }

            public void Deconstruct(
                out double m11, out double m12, out double m13,
                out double m21, out double m22, out double m23,
                out double m31, out double m32, out double m33) =>

                (m11, m12, m13, m21, m22, m23, m31, m32, m33)
                = (this.m11, this.m12, this.m13, this.m21, this.m22, this.m23, this.m31, this.m32, this.m33);
        };

        public class Vector {
            private readonly double e1, e2, e3;

            public Vector(double e1, double e2, double e3) {
                this.e1 = e1;
                this.e2 = e2;
                this.e3 = e3;
            }

            public void Deconstruct(out double e1, out double e2, out double e3) =>
                (e1, e2, e3) = (this.e1, this.e2, this.e3);
        };

        public static double Det(SymmMatrix mat) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat;

            double det = m1 * m5 * m5 + m3 * m4 * m4 + m2 * m6 * m6 - m1 * m2 * m3 - 2 * m4 * m5 * m6;

            return det;
        }

        public static SymmMatrix Invert(SymmMatrix mat) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat;

            double d = 1 / Det(mat);

            double n1 = (m5 * m5 - m2 * m3) * d;
            double n2 = (m6 * m6 - m3 * m1) * d;
            double n3 = (m4 * m4 - m1 * m2) * d;
            double n4 = (m3 * m4 - m5 * m6) * d;
            double n5 = (m1 * m5 - m6 * m4) * d;
            double n6 = (m2 * m6 - m4 * m5) * d;

            return new SymmMatrix(n1, n2, n3, n4, n5, n6);
        }

        public static SymmMatrix Add(SymmMatrix mat1, SymmMatrix mat2) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat1;
            (double n1, double n2, double n3, double n4, double n5, double n6) = mat2;

            return new SymmMatrix(m1 + n1, m2 + n2, m3 + n3, m4 + n4, m5 + n5, m6 + n6);
        }

        public static SymmMatrix Sub(SymmMatrix mat1, SymmMatrix mat2) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat1;
            (double n1, double n2, double n3, double n4, double n5, double n6) = mat2;

            return new SymmMatrix(m1 - n1, m2 - n2, m3 - n3, m4 - n4, m5 - n5, m6 - n6);
        }

        public static SymmMatrix Mul(SymmMatrix mat1, SymmMatrix mat2) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat1;
            (double n1, double n2, double n3, double n4, double n5, double n6) = mat2;

            double mn4 = m4 * n4, mn5 = m5 * n5, mn6 = m6 * n6;

            double r1 = m1 * n1 + mn4 + mn6;
            double r2 = m2 * n2 + mn4 + mn5;
            double r3 = m3 * n3 + mn5 + mn6;
            double r4 = m1 * n4 + m4 * n2 + m6 * n5;
            double r5 = m2 * n5 + m4 * n6 + m5 * n3;
            double r6 = m1 * n6 + m4 * n5 + m6 * n3;

            return new SymmMatrix(r1, r2, r3, r4, r5, r6);
        }

        public static Matrix Mul(SymmMatrix mat1, Matrix mat2) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat1;
            (double n11, double n12, double n13,
             double n21, double n22, double n23,
             double n31, double n32, double n33) = mat2;

            double r11 = m1 * n11 + m4 * n21 + m6 * n31;
            double r12 = m1 * n12 + m4 * n22 + m6 * n32;
            double r13 = m1 * n13 + m4 * n23 + m6 * n33;
            double r21 = m2 * n21 + m4 * n11 + m5 * n31;
            double r22 = m2 * n22 + m4 * n12 + m5 * n32;
            double r23 = m2 * n23 + m4 * n13 + m5 * n33;
            double r31 = m3 * n31 + m5 * n21 + m6 * n11;
            double r32 = m3 * n32 + m5 * n22 + m6 * n12;
            double r33 = m3 * n33 + m5 * n23 + m6 * n13;

            return new Matrix(r11, r12, r13, r21, r22, r23, r31, r32, r33);
        }

        public static Matrix Mul(Matrix mat1, SymmMatrix mat2) {
            (double n11, double n12, double n13,
             double n21, double n22, double n23,
             double n31, double n32, double n33) = mat1;
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat2;

            double r11 = m1 * n11 + m4 * n12 + m6 * n13;
            double r21 = m1 * n21 + m4 * n22 + m6 * n23;
            double r31 = m1 * n31 + m4 * n32 + m6 * n33;
            double r12 = m2 * n12 + m4 * n11 + m5 * n13;
            double r22 = m2 * n22 + m4 * n21 + m5 * n23;
            double r32 = m2 * n32 + m4 * n31 + m5 * n33;
            double r13 = m3 * n13 + m5 * n12 + m6 * n11;
            double r23 = m3 * n23 + m5 * n22 + m6 * n21;
            double r33 = m3 * n33 + m5 * n32 + m6 * n31;

            return new Matrix(r11, r12, r13, r21, r22, r23, r31, r32, r33);
        }

        public static SymmMatrix SqueezeMul(Matrix mat1, SymmMatrix mat2) {
            (double n11, double n12, double n13,
             double n21, double n22, double n23,
             double n31, double n32, double n33) = mat1;
            (double r11, double r12, double r13,
             double r21, double r22, double r23,
             double r31, double r32, double r33) = Mul(mat1, mat2);

            double r1 = n11 * r11 + n12 * r12 + n13 * r13;
            double r2 = n21 * r21 + n22 * r22 + n23 * r23;
            double r3 = n31 * r31 + n32 * r32 + n33 * r33;
            double r4 = n11 * r21 + n12 * r22 + n13 * r23;
            double r5 = n21 * r31 + n22 * r32 + n23 * r33;
            double r6 = n11 * r31 + n12 * r32 + n13 * r33;

            return new SymmMatrix(r1, r2, r3, r4, r5, r6);
        }

        public static Vector Mul(Matrix mat, Vector v) {
            (double n11, double n12, double n13,
             double n21, double n22, double n23,
             double n31, double n32, double n33) = mat;
            (double e1, double e2, double e3) = v;

            double u1 = n11 * e1 + n12 * e2 + n13 * e3;
            double u2 = n21 * e1 + n22 * e2 + n23 * e3;
            double u3 = n31 * e1 + n32 * e2 + n33 * e3;

            return new Vector(u1, u2, u3);
        }

        public static Vector Mul(SymmMatrix mat, Vector v) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat;
            (double e1, double e2, double e3) = v;

            double u1 = m1 * e1 + m4 * e2 + m6 * e3;
            double u2 = m4 * e1 + m2 * e2 + m5 * e3;
            double u3 = m6 * e1 + m5 * e2 + m3 * e3;

            return new Vector(u1, u2, u3);
        }

        public static ((double val, Vector vec) l1, (double val, Vector vec) l2, (double val, Vector vec) l3) EigenValues(SymmMatrix mat, double eps = 1e-10) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat;

            if (Math.Abs(m4) < eps && Math.Abs(m5) < eps && Math.Abs(m6) < eps) {
                return Order.AbsSort((m1, new Vector(1, 0, 0)), (m2, new Vector(0, 1, 0)), (m3, new Vector(0, 0, 1)));
            }

            (Complex x1, Complex x2, Complex x3) = RootFinding.Cubic(
                -m1 - m2 - m3,
                m1 * m2 + m2 * m3 + m3 * m1 - m4 * m4 - m5 * m5 - m6 * m6,
                m1 * m5 * m5 + m2 * m6 * m6 + m3 * m4 * m4 - m1 * m2 * m3 - 2 * m4 * m5 * m6
            );

            static Vector normalize(Vector v) {
                (double x, double y, double z) = v;

                double s = x > 0 ? 1 : x < 0 ? -1
                         : y > 0 ? 1 : y < 0 ? -1
                         : z >= 0 ? 1 : -1;
                double n = s * Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(z));

                return new Vector(x / n, y / n, z / n);
            }

            Vector eigenvector(double l) {
                double rx, ry, rz;
                double n1 = l - m1, n2 = l - m2, n3 = l - m3, n4 = -m4, n5 = -m5, n6 = -m6;

                if (Math.Abs(n1) < eps && Math.Abs(n4) < eps && Math.Abs(n6) < eps) {
                    return new Vector(1, 0, 0);
                }
                if (Math.Abs(n2) < eps && Math.Abs(n4) < eps && Math.Abs(n5) < eps) {
                    return new Vector(0, 1, 0);
                }
                if (Math.Abs(n3) < eps && Math.Abs(n5) < eps && Math.Abs(n6) < eps) {
                    return new Vector(0, 0, 1);
                }

                double[] ex = { n1, n4, n6 }, ey = { n4, n2, n5 }, ez = { n6, n5, n3 };

                (int iz0, int iz1, int iz2) = Order.AbsArgSort(n6, n5, n3);

                if (Math.Abs(ez[iz1]) > eps) {
                    rx = -ey[iz0] * ez[iz1] + ey[iz1] * ez[iz0];
                    ry = +ex[iz0] * ez[iz1] - ex[iz1] * ez[iz0];
                }
                else {
                    rx = -ey[iz0];
                    ry = +ex[iz0];
                }

                rz = -(ex[iz2] * rx + ey[iz2] * ry) / ez[iz2];

                return double.IsInfinity(rz)
                    ? new Vector(0, 0, 1)
                    : normalize(new Vector(rx, ry, rz));
            }

            // M = transpose(M) => lambda in R (det lambda I - M = 0)
            (double l1, double l2, double l3) = Order.AbsSort(x1.Real, x2.Real, x3.Real);

            return ((l1, eigenvector(l1)), (l2, eigenvector(l2)), (l3, eigenvector(l3)));
        }
    }
}
