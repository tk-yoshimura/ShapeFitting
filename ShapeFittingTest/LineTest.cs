using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFittingTest {
    [TestClass]
    public class LineTest {
        [TestMethod]
        public void CreateTest() {
            Line line = new(1, 2, 3);

            Assert.AreEqual(1d, line.A);
            Assert.AreEqual(2d, line.B);
            Assert.AreEqual(3d, line.C);
        }

        [TestMethod]
        public void ThetaTest() {
            Line line1 = new(2, 3), line2 = new(-1, 5);

            Assert.AreEqual(2d, line1.Theta, 1e-10);
            Assert.AreEqual(3d, line1.Phi);
            Assert.AreEqual(-1d, line2.Theta, 1e-10);
            Assert.AreEqual(5d, line2.Phi);
        }

        [TestMethod]
        public void EqualTest() {
            Line line1 = new(1, 2, 3), line2 = new(1, 2, 3), line3 = new(1, 3, 2);

            Assert.AreEqual(line1, line2);
            Assert.AreNotEqual(line1, line3);

            Assert.IsFalse(line1.Equals(null));

            Assert.IsTrue(line1 == line2);
            Assert.IsTrue(line1 != line3);
        }

        [TestMethod]
        public void ValidTest() {
            Line line1 = new(1, 2, 3), line2 = Line.NaN;

            Assert.AreNotEqual(line1, line2);
            Assert.IsTrue(line1.IsValid);
            Assert.IsFalse(line2.IsValid);
        }

        [TestMethod]
        public void FromFxTest() {
            Line line = Line.FromFx(0.2, 2);

            Assert.AreEqual(+1.6, line.Fx(-2), 1e-15);
            Assert.AreEqual(+1.8, line.Fx(-1), 1e-15);
            Assert.AreEqual(+2.0, line.Fx(0), 1e-15);
            Assert.AreEqual(+2.2, line.Fx(+1), 1e-15);
            Assert.AreEqual(+2.4, line.Fx(+2), 1e-15);

            Assert.AreEqual(-2, line.Fy(+1.6), 1e-15);
            Assert.AreEqual(-1, line.Fy(+1.8), 1e-15);
            Assert.AreEqual(0, line.Fy(+2.0), 1e-15);
            Assert.AreEqual(+1, line.Fy(+2.2), 1e-15);
            Assert.AreEqual(+2, line.Fy(+2.4), 1e-15);
        }

        [TestMethod]
        public void FromFyTest() {
            Line line = Line.FromFy(0.2, 2);

            Assert.AreEqual(-2, line.Fx(+1.6), 1e-15);
            Assert.AreEqual(-1, line.Fx(+1.8), 1e-15);
            Assert.AreEqual(0, line.Fx(+2.0), 1e-15);
            Assert.AreEqual(+1, line.Fx(+2.2), 1e-15);
            Assert.AreEqual(+2, line.Fx(+2.4), 1e-15);

            Assert.AreEqual(+1.6, line.Fy(-2), 1e-15);
            Assert.AreEqual(+1.8, line.Fy(-1), 1e-15);
            Assert.AreEqual(+2.0, line.Fy(0), 1e-15);
            Assert.AreEqual(+2.2, line.Fy(+1), 1e-15);
            Assert.AreEqual(+2.4, line.Fy(+2), 1e-15);
        }

        [TestMethod]
        public void FromPointsTest() {
            Line line1 = Line.FromPoints(new Vector(1, 2), new Vector(4, 3));
            Line line2 = Line.FromPoints(new Vector(1, 2), new Vector(3, 4));
            Line line3 = Line.FromPoints(new Vector(1, 2), new Vector(3, 2));
            Line line4 = Line.FromPoints(new Vector(1, 2), new Vector(1, 3));
            Line line5 = Line.FromPoints(new Vector(1, 2), new Vector(1, 2));
            Line line6 = Line.FromPoints(new Vector(1, 2), new Vector(2, 4));

            Assert.AreEqual(2, line1.Fx(1), 1e-15);
            Assert.AreEqual(3, line1.Fx(4), 1e-15);
            Assert.AreEqual(1, line1.Fy(2), 1e-15);
            Assert.AreEqual(4, line1.Fy(3), 1e-15);
            Assert.IsTrue(Math.Abs(line1.A) <= 1);
            Assert.IsTrue(Math.Abs(line1.B) <= 1);

            Assert.AreEqual(2, line2.Fx(1), 1e-15);
            Assert.AreEqual(4, line2.Fx(3), 1e-15);
            Assert.AreEqual(1, line2.Fy(2), 1e-15);
            Assert.AreEqual(3, line2.Fy(4), 1e-15);
            Assert.IsTrue(Math.Abs(line2.A) <= 1);
            Assert.IsTrue(Math.Abs(line2.B) <= 1);

            Assert.AreEqual(2, line3.Fx(1), 1e-15);
            Assert.AreEqual(2, line3.Fx(3), 1e-15);
            Assert.IsTrue(double.IsNaN(line3.Fy(2)));
            Assert.IsTrue(line3.IsValid);
            Assert.IsTrue(Math.Abs(line3.A) <= 1);
            Assert.IsTrue(Math.Abs(line3.B) <= 1);

            Assert.IsTrue(double.IsNaN(line4.Fx(1)));
            Assert.AreEqual(1, line4.Fy(2), 1e-15);
            Assert.AreEqual(1, line4.Fy(3), 1e-15);
            Assert.IsTrue(line4.IsValid);
            Assert.IsTrue(Math.Abs(line4.A) <= 1);
            Assert.IsTrue(Math.Abs(line4.B) <= 1);

            Assert.IsTrue(double.IsNaN(line5.Fx(1)));
            Assert.IsTrue(double.IsNaN(line5.Fy(2)));
            Assert.IsFalse(line5.IsValid);

            Assert.AreEqual(new Line(1, -0.5, 0), line6);
        }

        [TestMethod]
        public void MSEFitLineTest() {
            const int n = 1024;

            Random random = new Random(1234);

            double[] xs = (new double[n]).Select((_) => random.NextDouble() * 10 - 5).ToArray();

            foreach (double theta in new double[] { -1.4, -1.0, -0.5, 0, 0.5, 1, 1.4 }) {
                foreach (double phi in new double[] { -2, -1, 0, 1, 2 }) {
                    Line line = new(theta, phi);

                    double[] ys = line.Fx(xs);
                    IEnumerable<Vector> vs = Vector.Concat(xs, ys);

                    Line line_fit = MSEFitting.FitLine(vs);

                    Assert.AreEqual(theta, line_fit.Theta, 1e-5);
                    Assert.AreEqual(phi, line_fit.Phi, 1e-5);
                }
            }
        }

        [TestMethod]
        public void WeightedFitLineTest() {
            const int n = 1024;

            Random random = new Random(1234);

            double[] xs = (new double[n]).Select((_) => random.NextDouble() * 10 - 5).ToArray();
            double[] ws = (new double[n]).Select((_) => random.NextDouble() / 2 + 0.5).ToArray();

            foreach (double theta in new double[] { -1.4, -1.0, -0.5, 0, 0.5, 1, 1.4 }) {
                foreach (double phi in new double[] { -2, -1, 0, 1, 2 }) {
                    Line line = new(theta, phi);

                    double[] ys = line.Fx(xs);
                    IEnumerable<Vector> vs = Vector.Concat(xs, ys);

                    Line line_fit = WeightedFitting.FitLine(vs, ws);

                    Assert.AreEqual(theta, line_fit.Theta, 1e-5);
                    Assert.AreEqual(phi, line_fit.Phi, 1e-5);
                }
            }
        }
    }
}
