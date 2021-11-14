using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;

namespace ShapeFittingTest {
    [TestClass]
    public class AlgebraD3Tests {
        [TestMethod]
        public void EigenValuesTest1() {
            // mat = 1 4 6
            //       4 2 5
            //       6 5 3
            AlgebraD3.SymmMatrix mat = new(1, 2, 3, 4, 5, 6);

            ((double l1, AlgebraD3.Vector v1), (double l2, AlgebraD3.Vector v2), (double l3, AlgebraD3.Vector v3))
                = AlgebraD3.EigenValues(mat);

            Assert.AreEqual(-2.03379, l1, 1e-5);
            Assert.AreEqual(-4.09460, l2, 1e-5);
            Assert.AreEqual(12.1284, l3, 1e-4);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(1.11706, e11 / e13, 1e-5);
            Assert.AreEqual(-2.34723, e12 / e13, 1e-5);

            Assert.AreEqual(-1.10087, e21 / e23, 1e-5);
            Assert.AreEqual(-0.0978772, e22 / e23, 1e-7);

            Assert.AreEqual(0.835157, e31 / e33, 1e-5);
            Assert.AreEqual(0.823490, e32 / e33, 1e-5);
        }

        [TestMethod]
        public void EigenValuesTest2() {
            // mat = -1 4 -6
            //        4 2  5
            //       -6 5 -3
            AlgebraD3.SymmMatrix mat = new(-1, 2, -3, 4, 5, -6);

            ((double l1, AlgebraD3.Vector v1), (double l2, AlgebraD3.Vector v2), (double l3, AlgebraD3.Vector v3))
                = AlgebraD3.EigenValues(mat);

            Assert.AreEqual(4.05820, l1, 1e-5);
            Assert.AreEqual(5.13117, l2, 1e-5);
            Assert.AreEqual(-11.1894, l3, 1e-4);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(-1.36885, e11 / e13, 1e-5);
            Assert.AreEqual(-0.230982, e12 / e13, 1e-6);

            Assert.AreEqual(0.379318, e21 / e23, 1e-6);
            Assert.AreEqual(2.08142, e22 / e23, 1e-5);

            Assert.AreEqual(0.83736, e31 / e33, 1e-5);
            Assert.AreEqual(-0.633043, e32 / e33, 1e-6);
        }

        [TestMethod]
        public void EigenValuesTest3() {
            // mat = 0 2 0
            //       2 1 0
            //       0 0 3
            AlgebraD3.SymmMatrix mat = new(0, 1, 3, 2, 0, 0);

            ((double l1, AlgebraD3.Vector v1), (double l2, AlgebraD3.Vector v2), (double l3, AlgebraD3.Vector v3))
                = AlgebraD3.EigenValues(mat);

            Assert.AreEqual(-1.56155, l1, 1e-5);
            Assert.AreEqual(+2.56155, l2, 1e-5);
            Assert.AreEqual(3, l3, 1e-10);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(-1.28078, e11 / e12, 1e-5);
            Assert.AreEqual(0, e13 / e22, 1e-10);

            Assert.AreEqual(0.780776, e21 / e22, 1e-5);
            Assert.AreEqual(0, e23 / e22, 1e-10);

            Assert.AreEqual(0, e31 / e33, 1e-10);
            Assert.AreEqual(0, e32 / e33, 1e-10);
        }

        [TestMethod]
        public void EigenValuesTest4() {
            // mat = 1 0 0
            //       0 2 0
            //       0 0 3
            AlgebraD3.SymmMatrix mat = new(1, 2, 3, 0, 0, 0);

            ((double l1, AlgebraD3.Vector v1), (double l2, AlgebraD3.Vector v2), (double l3, AlgebraD3.Vector v3))
                = AlgebraD3.EigenValues(mat);

            Assert.AreEqual(1, l1, 1e-10);
            Assert.AreEqual(2, l2, 1e-10);
            Assert.AreEqual(3, l3, 1e-10);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(0, e12 / e11, 1e-10);
            Assert.AreEqual(0, e13 / e11, 1e-10);

            Assert.AreEqual(0, e21 / e22, 1e-10);
            Assert.AreEqual(0, e23 / e22, 1e-10);

            Assert.AreEqual(0, e31 / e33, 1e-10);
            Assert.AreEqual(0, e32 / e33, 1e-10);
        }

        [TestMethod]
        public void EigenValuesTest5() {
            // mat = 1 4 6
            //       4 2 5
            //       6 5 3 x1e-10(eps)
            AlgebraD3.SymmMatrix mat = new(1e-10, 2e-10, 3e-10, 4e-10, 5e-10, 6e-10);

            ((double l1, AlgebraD3.Vector v1), (double l2, AlgebraD3.Vector v2), (double l3, AlgebraD3.Vector v3))
                = AlgebraD3.EigenValues(mat);

            Assert.AreEqual(-2.03379e-10, l1, 1e-15);
            Assert.AreEqual(-4.09460e-10, l2, 1e-15);
            Assert.AreEqual(12.1284e-10, l3, 1e-14);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(1.11706, e11 / e13, 1e-5);
            Assert.AreEqual(-2.34723, e12 / e13, 1e-5);

            Assert.AreEqual(-1.10087, e21 / e23, 1e-5);
            Assert.AreEqual(-0.0978772, e22 / e23, 1e-7);

            Assert.AreEqual(0.835157, e31 / e33, 1e-5);
            Assert.AreEqual(0.823490, e32 / e33, 1e-5);
        }

        [TestMethod]
        public void EigenValuesTest6() {
            // mat = 0 0 1
            //       0 2 0
            //       1 0 0
            AlgebraD3.SymmMatrix mat = new(0, 2, 0, 0, 0, 1);

            ((double l1, AlgebraD3.Vector v1), (double l2, AlgebraD3.Vector v2), (double l3, AlgebraD3.Vector v3))
                = AlgebraD3.EigenValues(mat);

            Assert.AreEqual(1, l1, 1e-10);
            Assert.AreEqual(-1, l2, 1e-10);
            Assert.AreEqual(2, l3, 1e-10);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(1, e11, 1e-10);
            Assert.AreEqual(0, e12, 1e-10);
            Assert.AreEqual(1, e13, 1e-10);

            Assert.AreEqual(+1, e21, 1e-10);
            Assert.AreEqual(0, e22, 1e-10);
            Assert.AreEqual(-1, e23, 1e-10);

            Assert.AreEqual(0, e31, 1e-10);
            Assert.AreEqual(1, e32, 1e-10);
            Assert.AreEqual(0, e33, 1e-10);
        }

        [TestMethod]
        public void EigenValuesTest7() {
            // mat = 1 0 0
            //       0 2 0
            //       0 0 1
            AlgebraD3.SymmMatrix mat = new(1, 2, 1, 0, 0, 0);

            ((double l1, AlgebraD3.Vector v1), (double l2, AlgebraD3.Vector v2), (double l3, AlgebraD3.Vector v3))
                = AlgebraD3.EigenValues(mat);

            Assert.AreEqual(1, l1, 1e-10);
            Assert.AreEqual(1, l2, 1e-10);
            Assert.AreEqual(2, l3, 1e-10);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(1, e11, 1e-10);
            Assert.AreEqual(0, e12, 1e-10);
            Assert.AreEqual(0, e13, 1e-10);

            Assert.AreEqual(0, e21, 1e-10);
            Assert.AreEqual(0, e22, 1e-10);
            Assert.AreEqual(1, e23, 1e-10);

            Assert.AreEqual(0, e31, 1e-10);
            Assert.AreEqual(1, e32, 1e-10);
            Assert.AreEqual(0, e33, 1e-10);
        }

        [TestMethod]
        public void SymmMatrixInverseMulTest() {
            // mat = -1 4 -6
            //        4 2  7
            //       -6 7 -3
            AlgebraD3.SymmMatrix mat = new(-1, 2, -3, 4, 7, -6);

            AlgebraD3.SymmMatrix mat_inv = AlgebraD3.Invert(mat);
            AlgebraD3.SymmMatrix mat_id1 = AlgebraD3.Mul(mat, mat_inv);
            AlgebraD3.SymmMatrix mat_id2 = AlgebraD3.Mul(mat_inv, mat);

            (double m1, double m2, double m3, double m4, double m5, double m6) = mat_id1;
            (double n1, double n2, double n3, double n4, double n5, double n6) = mat_id2;

            Assert.AreEqual(1d, AlgebraD3.Det(mat) * AlgebraD3.Det(mat_inv), 1e-10);

            Assert.AreEqual(1d, m1, 1e-10);
            Assert.AreEqual(1d, m2, 1e-10);
            Assert.AreEqual(1d, m3, 1e-10);
            Assert.AreEqual(0d, m4, 1e-10);
            Assert.AreEqual(0d, m5, 1e-10);
            Assert.AreEqual(0d, m6, 1e-10);

            Assert.AreEqual(1d, n1, 1e-10);
            Assert.AreEqual(1d, n2, 1e-10);
            Assert.AreEqual(1d, n3, 1e-10);
            Assert.AreEqual(0d, n4, 1e-10);
            Assert.AreEqual(0d, n5, 1e-10);
            Assert.AreEqual(0d, n6, 1e-10);
        }

        [TestMethod]
        public void MatrixMulTest() {
            AlgebraD3.SymmMatrix mat1 = new(-1, 2, -3, 4, 7, -6);
            AlgebraD3.Matrix mat2 = new(1, -2, 3,
                                         6, -5, -4,
                                        -7, 9, -8);

            AlgebraD3.Matrix mat12 = AlgebraD3.Mul(mat1, mat2);
            AlgebraD3.Matrix mat21 = AlgebraD3.Mul(mat2, mat1);

            (double m11, double m12, double m13,
             double m21, double m22, double m23,
             double m31, double m32, double m33) = mat12;
            (double n11, double n12, double n13,
             double n21, double n22, double n23,
             double n31, double n32, double n33) = mat21;

            Assert.AreEqual(65, m11, 1e-10);
            Assert.AreEqual(-72, m12, 1e-10);
            Assert.AreEqual(29, m13, 1e-10);
            Assert.AreEqual(-33, m21, 1e-10);
            Assert.AreEqual(45, m22, 1e-10);
            Assert.AreEqual(-52, m23, 1e-10);
            Assert.AreEqual(57, m31, 1e-10);
            Assert.AreEqual(-50, m32, 1e-10);
            Assert.AreEqual(-22, m33, 1e-10);

            Assert.AreEqual(-27, n11, 1e-10);
            Assert.AreEqual(21, n12, 1e-10);
            Assert.AreEqual(-29, n13, 1e-10);
            Assert.AreEqual(-2, n21, 1e-10);
            Assert.AreEqual(-14, n22, 1e-10);
            Assert.AreEqual(-59, n23, 1e-10);
            Assert.AreEqual(91, n31, 1e-10);
            Assert.AreEqual(-66, n32, 1e-10);
            Assert.AreEqual(129, n33, 1e-10);
        }

        [TestMethod]
        public void MatrixSqueezeMulTest() {
            AlgebraD3.SymmMatrix mat1 = new(-1, 2, -3, 4, 7, -6);
            AlgebraD3.Matrix mat2 = new(1, -2, 3,
                                         6, -5, -4,
                                        -7, 9, -8);

            AlgebraD3.SymmMatrix mat212 = AlgebraD3.SqueezeMul(mat2, mat1);

            (double m1, double m2, double m3, double m4, double m5, double m6) = mat212;

            Assert.AreEqual(-156, m1, 1e-10);
            Assert.AreEqual(294, m2, 1e-10);
            Assert.AreEqual(-2263, m3, 1e-10);
            Assert.AreEqual(-151, m4, 1e-10);
            Assert.AreEqual(360, m5, 1e-10);
            Assert.AreEqual(610, m6, 1e-10);
        }

        [TestMethod]
        public void VectorMulTest() {
            AlgebraD3.SymmMatrix mat1 = new(-1, 2, -3, 4, 7, -6);
            AlgebraD3.Matrix mat2 = new(1, -2, 3,
                                         6, -5, -4,
                                        -7, 9, -8);

            AlgebraD3.Vector vec = new(11, 13, -17);

            AlgebraD3.Vector vec1 = AlgebraD3.Mul(mat1, vec);
            AlgebraD3.Vector vec2 = AlgebraD3.Mul(mat2, vec);

            (double v1, double v2, double v3) = vec1;
            (double u1, double u2, double u3) = vec2;

            Assert.AreEqual(143, v1, 1e-10);
            Assert.AreEqual(-49, v2, 1e-10);
            Assert.AreEqual(76, v3, 1e-10);

            Assert.AreEqual(-66, u1, 1e-10);
            Assert.AreEqual(69, u2, 1e-10);
            Assert.AreEqual(176, u3, 1e-10);
        }

        [TestMethod]
        public void SymmMatrixAddSubTest() {
            AlgebraD3.SymmMatrix mat1 = new(-1, 2, -13, 4, 19, -16);
            AlgebraD3.SymmMatrix mat2 = new(14, -11, 12, 7, -5, -3);

            (double m1, double m2, double m3, double m4, double m5, double m6) = AlgebraD3.Add(mat1, mat2);
            (double n1, double n2, double n3, double n4, double n5, double n6) = AlgebraD3.Sub(mat1, mat2);

            Assert.AreEqual(13, m1, 1e-10);
            Assert.AreEqual(-9, m2, 1e-10);
            Assert.AreEqual(-1, m3, 1e-10);
            Assert.AreEqual(11, m4, 1e-10);
            Assert.AreEqual(14, m5, 1e-10);
            Assert.AreEqual(-19, m6, 1e-10);

            Assert.AreEqual(-15, n1, 1e-10);
            Assert.AreEqual(13, n2, 1e-10);
            Assert.AreEqual(-25, n3, 1e-10);
            Assert.AreEqual(-3, n4, 1e-10);
            Assert.AreEqual(24, n5, 1e-10);
            Assert.AreEqual(-13, n6, 1e-10);
        }
    }
}
