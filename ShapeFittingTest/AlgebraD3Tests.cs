using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Numerics;

namespace ShapeFittingTest {
    [TestClass]
    public class AlgebraD3Tests {
        [TestMethod]
        public void EigenValuesTest() {
            {
                // mat = 1 4 6
                //       4 2 5
                //       6 5 3
                AlgebraD3.SymmMatrix mat1 = new(1, 2, 3, 4, 5, 6);

                (double l1, double l2, double l3) = AlgebraD3.EigenValues(mat1);

                Assert.AreEqual(-4.09460, l1, 1e-5);
                Assert.AreEqual(-2.03379, l2, 1e-5);
                Assert.AreEqual(12.1284, l3, 1e-4);
            }

            {
                // mat = -1 4 -6
                //        4 2  5
                //       -6 5 -3
                AlgebraD3.SymmMatrix mat2 = new(-1, 2, -3, 4, 5, -6);

                (double l1, double l2, double l3) = AlgebraD3.EigenValues(mat2);

                Assert.AreEqual(-11.1894, l1, 1e-4);
                Assert.AreEqual( 4.05820, l2, 1e-5);
                Assert.AreEqual( 5.13117, l3, 1e-5);
            }
        }
    }
}
