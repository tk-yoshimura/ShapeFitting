using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;

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
        public void ValidTest() {
            Circle circle1 = new(1, 2, 3), circle2 = Circle.NaN;

            Assert.AreNotEqual(circle1, circle2);
            Assert.IsTrue(circle1.IsValid);
            Assert.IsFalse(circle2.IsValid);
        }
    }
}
