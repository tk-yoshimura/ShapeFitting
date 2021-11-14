using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Numerics;

namespace ShapeFittingTest {
    [TestClass]
    public class RootFindingTests {
        [TestMethod]
        public void QuadraticRootTest1() {
            //(x - 2)(x - 3) = x^2 - 5 x + 6 = 0

            (Complex x1, Complex x2) = RootFinding.Quadratic(-5, 6);

            Console.WriteLine(x1);
            Console.WriteLine(x2);

            Assert.AreEqual(0d, x1.Imaginary);
            Assert.AreEqual(0d, x2.Imaginary);

            double[] rs = new double[] { x1.Real, x2.Real };
            Array.Sort(rs);

            Assert.AreEqual(2d, rs[0], 1e-10);
            Assert.AreEqual(3d, rs[1], 1e-10);
        }

        [TestMethod]
        public void QuadraticRootTest2() {
            //(x - ((5 + sqrt(7)i) / 2))(x - ((5 - sqrt(7)i) / 2)) = x^2 - 5 x + 8 = 0

            (Complex x1, Complex x2) = RootFinding.Quadratic(-5, 8);

            Console.WriteLine(x1);
            Console.WriteLine(x2);

            Assert.AreEqual(-Math.Sqrt(7) / 2, x1.Imaginary, 1e-10);
            Assert.AreEqual(Math.Sqrt(7) / 2, x2.Imaginary, 1e-10);

            Assert.AreEqual(2.5d, x1.Real, 1e-10);
            Assert.AreEqual(2.5d, x2.Real, 1e-10);
        }

        [TestMethod]
        public void CubicRootTest1() {
            //(x - 2)(x - 3)(x - 5) = x^3 - 10 x^2 + 31 x - 30 = 0

            (Complex x1, Complex x2, Complex x3) = RootFinding.Cubic(-10, 31, -30);

            Console.WriteLine(x1);
            Console.WriteLine(x2);
            Console.WriteLine(x3);

            Assert.AreEqual(0d, x1.Imaginary, 1e-10);
            Assert.AreEqual(0d, x2.Imaginary, 1e-10);
            Assert.AreEqual(0d, x3.Imaginary, 1e-10);

            double[] rs = new double[] { x1.Real, x2.Real, x3.Real };
            Array.Sort(rs);

            Assert.AreEqual(2d, rs[0], 1e-10);
            Assert.AreEqual(3d, rs[1], 1e-10);
            Assert.AreEqual(5d, rs[2], 1e-10);
        }

        [TestMethod]
        public void CubicRootTest2() {
            //(x - 2)(x - ((5 + sqrt(7)i) / 2))(x - ((5 - sqrt(7)i) / 2)) = x^3 - 7 x^2 + 18 x - 16 = 0

            (Complex x1, Complex x2, Complex x3) = RootFinding.Cubic(-7, 18, -16);

            Console.WriteLine(x1);
            Console.WriteLine(x2);
            Console.WriteLine(x3);

            Assert.AreEqual(0d, x1.Imaginary, 1e-10);
            Assert.AreEqual(-Math.Sqrt(7) / 2, x2.Imaginary, 1e-10);
            Assert.AreEqual(Math.Sqrt(7) / 2, x3.Imaginary, 1e-10);

            Assert.AreEqual(2d, x1.Real, 1e-10);
            Assert.AreEqual(2.5d, x2.Real, 1e-10);
            Assert.AreEqual(2.5d, x3.Real, 1e-10);
        }
    }
}
