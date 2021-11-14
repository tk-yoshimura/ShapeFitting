using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Numerics;

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

            Assert.AreEqual(1,  l1, 1e-10);
            Assert.AreEqual(-1, l2, 1e-10);
            Assert.AreEqual(2,  l3, 1e-10);

            (double e11, double e12, double e13) = v1;
            (double e21, double e22, double e23) = v2;
            (double e31, double e32, double e33) = v3;

            Assert.AreEqual(1, e11, 1e-10);
            Assert.AreEqual(0, e12, 1e-10);
            Assert.AreEqual(1, e13, 1e-10);

            Assert.AreEqual(+1, e21, 1e-10);
            Assert.AreEqual(0,  e22, 1e-10);
            Assert.AreEqual(-1, e23, 1e-10);

            Assert.AreEqual(0, e31, 1e-10);
            Assert.AreEqual(1, e32, 1e-10);
            Assert.AreEqual(0, e33, 1e-10);
        }
    }
}
