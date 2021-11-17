using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFittingTest {
    [TestClass]
    public class CircleTest {
        [TestMethod]
        public void CreateTest() {
            Circle circle1 = new(new Vector(1, 2), 3);
            Circle circle2 = new(1, 2, 3);

            Assert.AreEqual(new Vector(1, 2), circle1.Center);
            Assert.AreEqual(3d, circle1.Radius);

            Assert.AreEqual(new Vector(1, 2), circle2.Center);
            Assert.AreEqual(3d, circle2.Radius);
        }

        [TestMethod]
        public void EqualTest() {
            Circle circle1 = new(1, 2, 3), circle2 = new(1, 2, 3), circle3 = new(4, 3, 2);

            Assert.AreEqual(circle1, circle2);
            Assert.AreNotEqual(circle1, circle3);

            Assert.IsFalse(circle1.Equals(null));

            Assert.IsTrue(circle1 == circle2);
            Assert.IsTrue(circle1 != circle3);
        }

        [TestMethod]
        public void ImplicitTest() {
            const double a = 5, b = 3, c = 2;

            Circle circle = Circle.FromImplicit(a, b, c);

            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.3m; theta += 0.1m) {
                thetas.Add((double)theta);

                (double x, double y) = circle.Point((double)theta);

                double dist = x * x + y * y + a * x + b * y + c;

                Assert.AreEqual(0, dist, 1e-10);
            }

            foreach ((double x, double y) in circle.Points(thetas)) {
                double dist = x * x + y * y + a * x + b * y + c;

                Assert.AreEqual(0, dist, 1e-10);
            }
        }

        [TestMethod]
        public void ValidTest() {
            Circle circle1 = new(1, 2, 3), circle2 = Circle.NaN;

            Assert.AreNotEqual(circle1, circle2);
            Assert.IsTrue(circle1.IsValid);
            Assert.IsFalse(circle2.IsValid);
        }

        [TestMethod]
        public void MSEFitCircleTest() {
            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.3m; theta += 0.1m) {
                thetas.Add((double)theta);
            }

            foreach (Vector center in new Vector[] { (-1, -1), (0, -1), (+1, -1), (-1, 0), (0, 0), (+1, 0), (-1, +1), (0, +1), (+1, +1) }) {
                foreach (double radius in new double[] { 1, 2, 4, 8 }) {
                    Circle circle = new(center, radius);

                    IEnumerable<Vector> vs = circle.Points(thetas);

                    Circle circle_fit = MSEFitting.FitCircle(vs);

                    Assert.AreEqual(center.X, circle_fit.Center.X, 1e-5);
                    Assert.AreEqual(center.Y, circle_fit.Center.Y, 1e-5);
                    Assert.AreEqual(radius, circle_fit.Radius, 1e-5);
                }
            }
        }

        [TestMethod]
        public void WeightedFitCircleTest() {
            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.3m; theta += 0.1m) {
                thetas.Add((double)theta);
            }

            Random random = new Random(1234);

            double[] ws = (new double[thetas.Count]).Select((_) => random.NextDouble() / 2 + 0.5).ToArray();

            foreach (Vector center in new Vector[] { (-1, -1), (0, -1), (+1, -1), (-1, 0), (0, 0), (+1, 0), (-1, +1), (0, +1), (+1, +1) }) {
                foreach (double radius in new double[] { 1, 2, 4, 8 }) {
                    Circle circle = new(center, radius);

                    IEnumerable<Vector> vs = circle.Points(thetas);

                    Circle circle_fit = WeightedFitting.FitCircle(vs, ws);

                    Assert.AreEqual(center.X, circle_fit.Center.X, 1e-5);
                    Assert.AreEqual(center.Y, circle_fit.Center.Y, 1e-5);
                    Assert.AreEqual(radius, circle_fit.Radius, 1e-5);
                }
            }
        }
    }
}
