using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;

namespace ShapeFittingTest {
    [TestClass]
    public class AlgebraD2Tests {
        [TestMethod]
        public void EigenValuesTest1() {
            // mat = 1 3
            //       3 2
            AlgebraD2.SymmMatrix mat = new(1, 2, 3);

            ((double l1, AlgebraD2.Vector v1), (double l2, AlgebraD2.Vector v2)) = AlgebraD2.EigenValues(mat);

            Assert.AreEqual(-1.54138, l1, 1e-5);
            Assert.AreEqual(4.54138, l2, 1e-5);

            (double e11, double e12) = v1;
            (double e21, double e22) = v2;

            Assert.AreEqual(-1.18046, e11 / e12, 1e-5);
            Assert.AreEqual(0.847127, e21 / e22, 1e-5);
        }

        [TestMethod]
        public void EigenValuesTest2() {
            // mat = -1 -3
            //       -3  2
            AlgebraD2.SymmMatrix mat = new(-1, 2, -3);

            ((double l1, AlgebraD2.Vector v1), (double l2, AlgebraD2.Vector v2)) = AlgebraD2.EigenValues(mat);

            Assert.AreEqual(-2.8541, l1, 1e-5);
            Assert.AreEqual(3.8541, l2, 1e-5);

            (double e11, double e12) = v1;
            (double e21, double e22) = v2;

            Assert.AreEqual(1.618034, e11 / e12, 1e-5);
            Assert.AreEqual(-0.618034, e21 / e22, 1e-5);
        }

        [TestMethod]
        public void EigenValuesTest3() {
            // mat = 0 2
            //       2 1
            AlgebraD2.SymmMatrix mat = new(0, 1, 2);

            ((double l1, AlgebraD2.Vector v1), (double l2, AlgebraD2.Vector v2))
                = AlgebraD2.EigenValues(mat);

            Assert.AreEqual(-1.56155, l1, 1e-5);
            Assert.AreEqual(+2.56155, l2, 1e-5);

            (double e11, double e12) = v1;
            (double e21, double e22) = v2;

            Assert.AreEqual(-1.28078, e11 / e12, 1e-5);
            Assert.AreEqual(0.780776, e21 / e22, 1e-5);
        }

        [TestMethod]
        public void EigenValuesTest4() {
            // mat = 2 0
            //       0 1
            AlgebraD2.SymmMatrix mat = new(2, 1, 0);

            ((double l1, AlgebraD2.Vector v1), (double l2, AlgebraD2.Vector v2))
                = AlgebraD2.EigenValues(mat);

            Assert.AreEqual(1, l1, 1e-10);
            Assert.AreEqual(2, l2, 1e-10);

            (double e11, double e12) = v1;
            (double e21, double e22) = v2;

            Assert.AreEqual(0, e11, 1e-10);
            Assert.AreEqual(1, e12, 1e-10);

            Assert.AreEqual(1, e21, 1e-10);
            Assert.AreEqual(0, e22, 1e-10);
        }

        [TestMethod]
        public void EigenValuesTest5() {
            // mat = 1 3
            //       3 2 x1e-10(eps)
            AlgebraD2.SymmMatrix mat = new(1e-10, 2e-10, 3e-10);

            ((double l1, AlgebraD2.Vector v1), (double l2, AlgebraD2.Vector v2))
                = AlgebraD2.EigenValues(mat);

            Assert.AreEqual(-1.54138e-10, l1, 1e-5);
            Assert.AreEqual(4.54138e-10, l2, 1e-5);

            (double e11, double e12) = v1;
            (double e21, double e22) = v2;

            Assert.AreEqual(-1.18046, e11 / e12, 1e-5);
            Assert.AreEqual(0.847127, e21 / e22, 1e-5);
        }

        [TestMethod]
        public void EigenValuesTest6() {
            // mat = 0 0
            //       0 2
            AlgebraD2.SymmMatrix mat = new(0, 2, 0);

            ((double l1, AlgebraD2.Vector v1), (double l2, AlgebraD2.Vector v2))
                = AlgebraD2.EigenValues(mat);

            Assert.AreEqual(0, l1, 1e-10);
            Assert.AreEqual(2, l2, 1e-10);

            (double e11, double e12) = v1;
            (double e21, double e22) = v2;

            Assert.AreEqual(1, e11, 1e-10);
            Assert.AreEqual(0, e12, 1e-10);

            Assert.AreEqual(0, e21, 1e-10);
            Assert.AreEqual(1, e22, 1e-10);
        }

        [TestMethod]
        public void EigenValuesTest7() {
            // mat = 1 0
            //       0 1
            AlgebraD2.SymmMatrix mat = new(1, 1, 0);

            ((double l1, AlgebraD2.Vector v1), (double l2, AlgebraD2.Vector v2))
                = AlgebraD2.EigenValues(mat);

            Assert.AreEqual(1, l1, 1e-10);
            Assert.AreEqual(1, l2, 1e-10);

            (double e11, double e12) = v1;
            (double e21, double e22) = v2;

            Assert.AreEqual(1, e11, 1e-10);
            Assert.AreEqual(0, e12, 1e-10);

            Assert.AreEqual(0, e21, 1e-10);
            Assert.AreEqual(1, e22, 1e-10);
        }

        [TestMethod]
        public void SymmMatrixInverseMulTest() {
            // mat = -1 4
            //        4 2
            AlgebraD2.SymmMatrix mat = new(-1, 2, 4);

            AlgebraD2.SymmMatrix mat_inv = AlgebraD2.Invert(mat);
            AlgebraD2.Matrix mat_id1 = AlgebraD2.Mul(mat, mat_inv);
            AlgebraD2.Matrix mat_id2 = AlgebraD2.Mul(mat_inv, mat);

            (double m11, double m12,
             double m21, double m22) = mat_id1;
            (double n11, double n12,
             double n21, double n22) = mat_id2;

            Assert.AreEqual(1d, AlgebraD2.Det(mat) * AlgebraD2.Det(mat_inv), 1e-10);

            Assert.AreEqual(1d, m11, 1e-10);
            Assert.AreEqual(1d, m22, 1e-10);
            Assert.AreEqual(0d, m12, 1e-10);
            Assert.AreEqual(0d, m21, 1e-10);

            Assert.AreEqual(1d, n11, 1e-10);
            Assert.AreEqual(1d, n22, 1e-10);
            Assert.AreEqual(0d, n12, 1e-10);
            Assert.AreEqual(0d, n21, 1e-10);
        }

        [TestMethod]
        public void MatrixMulTest() {
            AlgebraD2.SymmMatrix mat1 = new(-1, 2, -3);
            AlgebraD2.Matrix mat2 = new(-5, -4,
                                         9, -8);
            AlgebraD2.Matrix mat3 = new(1, -2,
                                        -3, 7);

            AlgebraD2.Matrix mat12 = AlgebraD2.Mul(mat1, mat2);
            AlgebraD2.Matrix mat21 = AlgebraD2.Mul(mat2, mat1);
            AlgebraD2.Matrix mat23 = AlgebraD2.Mul(mat2, mat3);

            (double m11, double m12,
             double m21, double m22) = mat12;
            (double n11, double n12,
             double n21, double n22) = mat21;
            (double r11, double r12,
             double r21, double r22) = mat23;

            Assert.AreEqual(-22, m11, 1e-10);
            Assert.AreEqual(28, m12, 1e-10);
            Assert.AreEqual(33, m21, 1e-10);
            Assert.AreEqual(-4, m22, 1e-10);

            Assert.AreEqual(17, n11, 1e-10);
            Assert.AreEqual(7, n12, 1e-10);
            Assert.AreEqual(15, n21, 1e-10);
            Assert.AreEqual(-43, n22, 1e-10);

            Assert.AreEqual(7, r11, 1e-10);
            Assert.AreEqual(-18, r12, 1e-10);
            Assert.AreEqual(33, r21, 1e-10);
            Assert.AreEqual(-74, r22, 1e-10);
        }

        [TestMethod]
        public void TransposeTest() {
            AlgebraD2.Matrix mat = new(-5, -4,
                                       9, -8);

            AlgebraD2.Matrix mat_t = AlgebraD2.Transpose(mat);

            (double r11, double r12,
             double r21, double r22) = mat_t;

            Assert.AreEqual(-5, r11, 1e-10);
            Assert.AreEqual(9, r12, 1e-10);
            Assert.AreEqual(-4, r21, 1e-10);
            Assert.AreEqual(-8, r22, 1e-10);
        }

        [TestMethod]
        public void VectorMulTest() {
            AlgebraD2.SymmMatrix mat1 = new(-1, 2, -3);
            AlgebraD2.Matrix mat2 = new(-5, -4,
                                         9, -8);

            AlgebraD2.Vector vec = new(11, 13);

            AlgebraD2.Vector vec1 = AlgebraD2.Mul(mat1, vec);
            AlgebraD2.Vector vec2 = AlgebraD2.Mul(mat2, vec);

            (double v1, double v2) = vec1;
            (double u1, double u2) = vec2;

            Assert.AreEqual(-50, v1, 1e-10);
            Assert.AreEqual(-7, v2, 1e-10);

            Assert.AreEqual(-107, u1, 1e-10);
            Assert.AreEqual(-5, u2, 1e-10);
        }

        [TestMethod]
        public void SymmMatrixAddSubTest() {
            AlgebraD2.SymmMatrix mat1 = new(-1, 2, -13);
            AlgebraD2.SymmMatrix mat2 = new(14, -11, 12);

            (double m1, double m2, double m3) = AlgebraD2.Add(mat1, mat2);
            (double n1, double n2, double n3) = AlgebraD2.Sub(mat1, mat2);

            Assert.AreEqual(13, m1, 1e-10);
            Assert.AreEqual(-9, m2, 1e-10);
            Assert.AreEqual(-1, m3, 1e-10);

            Assert.AreEqual(-15, n1, 1e-10);
            Assert.AreEqual(13, n2, 1e-10);
            Assert.AreEqual(-25, n3, 1e-10);
        }
    }
}
