using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFittingTest {
    [TestClass]
    public class WeightFuncTests {
        [TestMethod]
        public void TukeyTest() {
            List<double> xs = new();
            for (decimal x = 0; x <= 10; x += 0.1m) {
                xs.Add((double)x);
            }

            List<double> ys = WeightFunc.Tukey(xs, 2).ToList();

            Assert.AreEqual(1d, ys[0]);
            Assert.AreEqual(225d / 256, ys[5]);
            Assert.AreEqual(9d / 16, ys[10]);
            Assert.AreEqual(49d / 256, ys[15]);
            Assert.AreEqual(1e-8d, ys[20], 1e-16);
            Assert.AreEqual(1e-8d, ys[25], 1e-16);
        }

        [TestMethod]
        public void HuberTest() {
            List<double> xs = new();
            for (decimal x = 0; x <= 10; x += 0.1m) {
                xs.Add((double)x);
            }

            List<double> ys = WeightFunc.Huber(xs, 2).ToList();

            Assert.AreEqual(1d, ys[0]);
            Assert.AreEqual(1d, ys[5]);
            Assert.AreEqual(1d, ys[10]);
            Assert.AreEqual(1d, ys[15]);
            Assert.AreEqual(1d, ys[20]);
            Assert.AreEqual(4d / 5, ys[25], 1e-10);
            Assert.AreEqual(2d / 3, ys[30], 1e-10);
            Assert.AreEqual(4d / 7, ys[35], 1e-10);
            Assert.AreEqual(1d / 2, ys[40], 1e-10);
        }

        [TestMethod]
        public void LassoTest() {
            List<double> xs = new();
            for (decimal x = 0; x <= 10; x += 0.1m) {
                xs.Add((double)x);
            }

            List<double> ys = WeightFunc.Lasso(xs).ToList();

            Assert.AreEqual(1d, ys[0]);
            Assert.AreEqual(1d / 50, ys[5]);
            Assert.AreEqual(1d / 100, ys[10]);
            Assert.AreEqual(1d / 150, ys[15]);
            Assert.AreEqual(1d / 200, ys[20]);
            Assert.AreEqual(1d / 250, ys[25], 1e-10);
            Assert.AreEqual(1d / 300, ys[30], 1e-10);
            Assert.AreEqual(1d / 350, ys[35], 1e-10);
            Assert.AreEqual(1d / 400, ys[40], 1e-10);
            Assert.AreEqual(1d / 1000, ys[100], 1e-10);
        }
    }
}
