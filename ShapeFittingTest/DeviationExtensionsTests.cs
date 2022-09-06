using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;

namespace ShapeFittingTest {
    [TestClass]
    public class DeviationExtensionsTests {
        [TestMethod]
        public void MedianTest() {
            Assert.IsTrue(double.IsNaN(System.Array.Empty<double>().Median()));
            Assert.AreEqual(1d, new double[] { 1 }.Median());
            Assert.AreEqual(1.5d, new double[] { 2, 1 }.Median());
            Assert.AreEqual(2d, new double[] { 2, 3, 1 }.Median());
            Assert.AreEqual(2.5d, new double[] { 2, 3, 1, 4 }.Median());
            Assert.AreEqual(3d, new double[] { 4, 1, 6, 2, 9, 2 }.Median());
            Assert.AreEqual(2d, new double[] { 1, 4, 1, 6, 2, 9, 2 }.Median());
        }

        [TestMethod]
        public void MedianAbsoluteDeviationTest() {
            Assert.AreEqual(1d, new double[] { 1, 4, 1, 6, 2, 9, 2 }.MedianAbsoluteDeviation().mad);
        }

        [TestMethod]
        public void AverageAbsoluteDeviationTest() {
            Assert.AreEqual(3.6d, new double[] { 2, 2, 3, 4, 14 }.AverageAbsoluteDeviation().aad, 1e-10);
        }
    }
}
