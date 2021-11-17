using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFittingTest {
    [TestClass]
    public class EllipseTest {
        [TestMethod]
        public void CreateTest() {
            Ellipse ellipse = new(new Vector(1, 2), (4, 3), 5);

            Assert.AreEqual(new Vector(1, 2), ellipse.Center);
            Assert.AreEqual(4d, ellipse.Axis.major);
            Assert.AreEqual(3d, ellipse.Axis.minor);
            Assert.AreEqual(5d, ellipse.Angle);
        }

        [TestMethod]
        public void EqualTest() {
            Ellipse ellipse1 = new(new Vector(1, 2), (4, 3), 5);
            Ellipse ellipse2 = new((1, 2), (4, 3), 5);
            Ellipse ellipse3 = new((1, 2), (4, 4), 5);

            Assert.AreEqual(ellipse1, ellipse2);
            Assert.AreNotEqual(ellipse1, ellipse3);

            Assert.IsFalse(ellipse1.Equals(null));

            Assert.IsTrue(ellipse1 == ellipse2);
            Assert.IsTrue(ellipse1 != ellipse3);
        }

        [TestMethod]
        public void ImplicitTest1() {
            const double a = 5, b = 3, c = 2, d = 7, e = 11, f = -13;

            Ellipse ellipse = Ellipse.FromImplicit(a, b, c, d, e, f);

            Assert.AreEqual(+0.1612903226, ellipse.Center.X, 1e-10);
            Assert.AreEqual(-2.8709677419, ellipse.Center.Y, 1e-10);
            Assert.AreEqual(4.52471777797, ellipse.Axis.major, 1e-10);
            Assert.AreEqual(2.24080472704, ellipse.Axis.minor, 1e-10);
            Assert.AreEqual(1.96349540849, ellipse.Angle, 1e-10);

            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.3m; theta += 0.1m) {
                thetas.Add((double)theta);

                (double x, double y) = ellipse.Point((double)theta);

                double dist = a * x * x + b * x * y + c * y * y + d * x + e * y + f;

                Assert.AreEqual(0, dist, 1e-10);
            }

            foreach ((double x, double y) in ellipse.Points(thetas)) {
                double dist = a * x * x + b * x * y + c * y * y + d * x + e * y + f;

                Assert.AreEqual(0, dist, 1e-10);
            }
        }

        [TestMethod]
        public void ImplicitTest2() {
            const double a = 5, b = 0, c = 5, d = 7, e = 11, f = -13;

            Ellipse ellipse = Ellipse.FromImplicit(a, b, c, d, e, f);

            Assert.AreEqual(-0.7, ellipse.Center.X, 1e-10);
            Assert.AreEqual(-1.1, ellipse.Center.Y, 1e-10);
            Assert.AreEqual(2.07364413533, ellipse.Axis.major, 1e-10);
            Assert.AreEqual(2.07364413533, ellipse.Axis.minor, 1e-10);
            Assert.AreEqual(0.0, ellipse.Angle, 1e-10);

            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.3m; theta += 0.1m) {
                thetas.Add((double)theta);

                (double x, double y) = ellipse.Point((double)theta);

                double dist = a * x * x + b * x * y + c * y * y + d * x + e * y + f;

                Assert.AreEqual(0, dist, 1e-10);
            }

            foreach ((double x, double y) in ellipse.Points(thetas)) {
                double dist = a * x * x + b * x * y + c * y * y + d * x + e * y + f;

                Assert.AreEqual(0, dist, 1e-10);
            }
        }

        [TestMethod]
        public void ValidTest() {
            Ellipse ellipse1 = new((1, 2), (4, 3), 5), ellipse2 = Ellipse.NaN;

            Assert.AreNotEqual(ellipse1, ellipse2);
            Assert.IsTrue(ellipse1.IsValid);
            Assert.IsFalse(ellipse2.IsValid);
        }

        [TestMethod]
        public void MSEFitEllipseTest() {
            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.3m; theta += 0.1m) {
                thetas.Add((double)theta);
            }

            foreach (Vector center in new Vector[] { (-1, -1), (0, -1), (+1, -1), (-1, 0), (0, 0), (+1, 0), (-1, +1), (0, +1), (+1, +1) }) {
                foreach ((double major, double minor) axis in new (double, double)[] { (2, 1), (3, 2), (5, 4), (9, 8) }) {
                    foreach (double angle in new double[] { -1.2, -0.6, 0, +0.6, +1.2 }) {

                        Ellipse Ellipse = new(center, axis, angle);

                        IEnumerable<Vector> vs = Ellipse.Points(thetas);

                        Ellipse Ellipse_fit = MSEFitting.FitEllipse(vs);

                        Assert.AreEqual(center.X, Ellipse_fit.Center.X, 1e-5);
                        Assert.AreEqual(center.Y, Ellipse_fit.Center.Y, 1e-5);
                        Assert.AreEqual(axis.major, Ellipse_fit.Axis.major, 1e-5);
                        Assert.AreEqual(axis.minor, Ellipse_fit.Axis.minor, 1e-5);

                        Assert.AreEqual(0f, Math.Sin(Ellipse_fit.Angle - angle), 1e-5);
                    }
                }
            }
        }

        [TestMethod]
        public void WeightedFitEllipseTest() {
            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.3m; theta += 0.1m) {
                thetas.Add((double)theta);
            }

            Random random = new Random(1234);

            double[] ws = (new double[thetas.Count]).Select((_) => random.NextDouble() / 2 + 0.5).ToArray();

            foreach (Vector center in new Vector[] { (-1, -1), (0, -1), (+1, -1), (-1, 0), (0, 0), (+1, 0), (-1, +1), (0, +1), (+1, +1) }) {
                foreach ((double major, double minor) axis in new (double, double)[] { (2, 1), (3, 2), (5, 4), (9, 8) }) {
                    foreach (double angle in new double[] { -1.2, -0.6, 0, +0.6, +1.2 }) {

                        Ellipse Ellipse = new(center, axis, angle);

                        IEnumerable<Vector> vs = Ellipse.Points(thetas);

                        Ellipse Ellipse_fit = WeightedFitting.FitEllipse(vs, ws);

                        Assert.AreEqual(center.X, Ellipse_fit.Center.X, 1e-5);
                        Assert.AreEqual(center.Y, Ellipse_fit.Center.Y, 1e-5);
                        Assert.AreEqual(axis.major, Ellipse_fit.Axis.major, 1e-5);
                        Assert.AreEqual(axis.minor, Ellipse_fit.Axis.minor, 1e-5);

                        Assert.AreEqual(0f, Math.Sin(Ellipse_fit.Angle - angle), 1e-5);

                    }
                }
            }
        }
    }
}
