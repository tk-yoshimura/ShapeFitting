using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Numerics;

namespace ShapeFittingTest {
    [TestClass]
    public class AlgebraD2Tests {
        [TestMethod]
        public void EigenValuesTest() {
            {
                // mat = 1 3
                //       3 2
                AlgebraD2.SymmMatrix mat1 = new(1, 2, 3);

                (double l1, double l2) = AlgebraD2.EigenValues(mat1);

                Assert.AreEqual(-1.54138, l1, 1e-5);
                Assert.AreEqual( 4.54138, l2, 1e-5);
            }

            {
                // mat = -1 -3
                //       -3  2
                AlgebraD2.SymmMatrix mat2 = new(-1, 2, -3);

                (double l1, double l2) = AlgebraD2.EigenValues(mat2);

                Assert.AreEqual(-2.8541, l1, 1e-5);
                Assert.AreEqual( 3.8541, l2, 1e-5);
            }
        }
    }
}
