using System;
using System.Collections.Generic;
using System.Linq;
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

            public double MaxAbsValue {
                get {
                    static double max(double a, double b, double c) => Math.Max(Math.Max(a, b), c);

                    return max(Math.Max(Math.Abs(m1), Math.Abs(m2)),
                               Math.Max(Math.Abs(m3), Math.Abs(m4)),
                               Math.Max(Math.Abs(m5), Math.Abs(m6)));
                }
            }
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

            public double MaxAbsValue {
                get {
                    static double max(double a, double b, double c) => Math.Max(Math.Max(a, b), c);

                    return max(max(Math.Abs(m11), Math.Abs(m12), Math.Abs(m13)),
                               max(Math.Abs(m21), Math.Abs(m22), Math.Abs(m23)),
                               max(Math.Abs(m31), Math.Abs(m32), Math.Abs(m33)));
                }
            }
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

        public static Matrix Mul(SymmMatrix mat1, SymmMatrix mat2) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat1;
            (double n1, double n2, double n3, double n4, double n5, double n6) = mat2;

            double r11 = m1 * n1 + m4 * n4 + m6 * n6;
            double r12 = m1 * n4 + m4 * n2 + m6 * n5;
            double r13 = m1 * n6 + m4 * n5 + m6 * n3;
            double r21 = m2 * n4 + m5 * n6 + m4 * n1;
            double r22 = m2 * n2 + m5 * n5 + m4 * n4;
            double r23 = m2 * n5 + m5 * n3 + m4 * n6;
            double r31 = m3 * n6 + m5 * n4 + m6 * n1;
            double r32 = m3 * n5 + m5 * n2 + m6 * n4;
            double r33 = m3 * n3 + m5 * n5 + m6 * n6;

            return new Matrix(r11, r12, r13, r21, r22, r23, r31, r32, r33);
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

        public static Matrix Mul(Matrix mat1, Matrix mat2) {
            (double m11, double m12, double m13,
             double m21, double m22, double m23,
             double m31, double m32, double m33) = mat1;
            (double n11, double n12, double n13,
             double n21, double n22, double n23,
             double n31, double n32, double n33) = mat2;

            double r11 = m11 * n11 + m12 * n21 + m13 * n31;
            double r21 = m21 * n11 + m22 * n21 + m23 * n31;
            double r31 = m31 * n11 + m32 * n21 + m33 * n31;
            double r12 = m11 * n12 + m12 * n22 + m13 * n32;
            double r22 = m21 * n12 + m22 * n22 + m23 * n32;
            double r32 = m31 * n12 + m32 * n22 + m33 * n32;
            double r13 = m11 * n13 + m12 * n23 + m13 * n33;
            double r23 = m21 * n13 + m22 * n23 + m23 * n33;
            double r33 = m31 * n13 + m32 * n23 + m33 * n33;

            return new Matrix(r11, r12, r13, r21, r22, r23, r31, r32, r33);
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

        public static Matrix Transpose(Matrix mat) {
            (double m11, double m12, double m13,
             double m21, double m22, double m23,
             double m31, double m32, double m33) = mat;

            return new Matrix(m11, m21, m31, m12, m22, m32, m13, m23, m33);
        }

        public static ((double val, Vector vec) l1, (double val, Vector vec) l2, (double val, Vector vec) l3) EigenValues(SymmMatrix mat, double eps = 1e-8) {
            (double m1, double m2, double m3, double m4, double m5, double m6) = mat;

            double veps = eps * mat.MaxAbsValue;

            if (Math.Abs(m4) < veps && Math.Abs(m5) < veps && Math.Abs(m6) < veps) {
                return Order.AbsSort((m1, new Vector(1, 0, 0)), (m2, new Vector(0, 1, 0)), (m3, new Vector(0, 0, 1)));
            }

            (Complex x1, Complex x2, Complex x3) = RootFinding.Cubic(
                -m1 - m2 - m3,
                m1 * m2 + m2 * m3 + m3 * m1 - m4 * m4 - m5 * m5 - m6 * m6,
                m1 * m5 * m5 + m2 * m6 * m6 + m3 * m4 * m4 - m1 * m2 * m3 - 2 * m4 * m5 * m6
            );

            // M = transpose(M) => lambda in R (det lambda I - M = 0)
            (double l1, double l2, double l3) = Order.AbsSort(x1.Real, x2.Real, x3.Real);

            Matrix mat_asymm = new(m1, m4, m6, m4, m2, m5, m6, m5, m3);

            return ((l1, EigenVector(mat_asymm, l1, veps)),
                    (l2, EigenVector(mat_asymm, l2, veps)),
                    (l3, EigenVector(mat_asymm, l3, veps)));
        }

        public static IEnumerable<(double val, Vector vec)> EigenValues(Matrix mat, double eps = 1e-8) {
            (double m11, double m12, double m13,
             double m21, double m22, double m23,
             double m31, double m32, double m33) = mat;

            double veps = eps * mat.MaxAbsValue;

            if (Math.Abs(m12) < veps && Math.Abs(m13) < veps && Math.Abs(m21) < veps &&
                Math.Abs(m23) < veps && Math.Abs(m31) < veps && Math.Abs(m33) < veps) {

                return Order.AbsSort(new (double val, Vector vec)[]{
                    (m11, new Vector(1, 0, 0)), (m22, new Vector(0, 1, 0)), (m33, new Vector(0, 0, 1))
                });
            }

            (Complex x1, Complex x2, Complex x3) = RootFinding.Cubic(
                 -m11 - m22 - m33,
                 m11 * m22 + m22 * m33 + m33 * m11 - m12 * m21 - m23 * m32 - m31 * m13,
                 m11 * (m23 * m32 - m22 * m33) + m12 * (m21 * m33 - m23 * m31) + m13 * (m22 * m31 - m21 * m32)
            );

            double[] ls = Order.AbsSort(
                    new Complex[] { x1, x2, x3 }
                    .Where((c) => Math.Abs(c.Real) < eps || Math.Abs(c.Real) * eps >= Math.Abs(c.Imaginary))
                    .Select((c) => c.Real)
                ).ToArray();

            return ls.Select((l) => (l, EigenVector(mat, l, veps)));
        }

        private static Vector EigenVector(Matrix mat, double l, double eps) {
            static Vector normalize(Vector v) {
                (double x, double y, double z) = v;

                double s = x > 0 ? 1 : x < 0 ? -1
                         : y > 0 ? 1 : y < 0 ? -1
                         : z >= 0 ? 1 : -1;
                double n = s * Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(z));

                return new Vector(x / n, y / n, z / n);
            }

            (double m11, double m12, double m13,
             double m21, double m22, double m23,
             double m31, double m32, double m33) = mat;

            m11 -= l; m22 -= l; m33 -= l;
            
            double rx, ry, rz;
            double[][] e = { new double[]{ m11, m12, m13 }, 
                             new double[]{ m21, m22, m23 }, 
                             new double[]{ m31, m32, m33 } };

            (int i0, int i1, int i2) = Order.AbsArgSort(e[0][0], e[1][0], e[2][0]);
            e = new double[][] { e[i0], e[i1], e[i2] };

            if (Math.Abs(e[2][0]) > eps) {
                double r02 = e[0][0] / e[2][0], r12 = e[1][0] / e[2][0];

                e[0][0] = e[1][0] = 0;
                e[0][1] -= e[2][1] * r02;
                e[0][2] -= e[2][2] * r02;
                e[1][1] -= e[2][1] * r12;
                e[1][2] -= e[2][2] * r12;
            }

            if (Math.Abs(e[0][1]) > Math.Abs(e[1][1])) {
                e = new double[][] { e[1], e[0], e[2] };
            }

            if (Math.Abs(e[1][1]) > eps) {
                double r01 = e[0][1] / e[1][1], r21 = e[2][1] / e[1][1];

                e[0][1] = e[2][1] = 0;
                e[0][2] -= e[1][2] * r01;
                e[2][2] -= e[1][2] * r21;
            }

            if (Math.Abs(e[0][2]) > eps) {
                e[1][2] = e[2][2] = 0;
            }

#if DEBUG
            if (Math.Abs(e[0][0]) > eps || Math.Abs(e[0][1]) > eps || Math.Abs(e[1][0]) > eps) {
                throw new NotImplementedException();
            }
#endif

            if (Math.Abs(e[0][2]) <= eps) {
                double s11 = e[2][0], s12 = e[2][1], s13 = e[2][2], s22 = e[1][1], s23 = e[1][2];
                // s11 rx + s12 ry + s13 rz = 0
                //          s22 ry + s23 rz = 0

                if (Math.Abs(s11) > eps) {
                    (rx, ry, rz) = ((s12 * s23 - s13 * s22) / s11, -s23, s22);
                }
                else if (Math.Abs(s12) <= eps && Math.Abs(s13) <= eps) {
                    (rx, ry, rz) = (0, -s23, s22);
                }
                else {
                    (rx, ry, rz) = (1, 0, 0);
                }
            }
            else { 
                double s11 = e[2][0], s12 = e[2][1], s22 = e[1][1], s33 = e[0][2];
                // s11 rx + s12 ry        = 0
                //          s22 ry        = 0
                //                 s33 rz = 0

                if (Math.Abs(s22) <= eps) {
                    (rx, ry, rz) = (-s12, s11, 0);
                }
                else if(Math.Abs(s11) <= eps){
                    (rx, ry, rz) = (1, 0, 0);
                }
                else {
                    // det M - lambda I != 0
                    return new Vector(0, 0, 0);
                }
            }

            return normalize(new Vector(rx, ry, rz));
        }
    }
}
