using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;

namespace ShapeFittingTest {
    [TestClass]
    public class VectorTest {
        [TestMethod]
        public void CreateTest() {
            Vector vector = new(2, 3);

            Assert.AreEqual(2d, vector.X);
            Assert.AreEqual(3d, vector.Y);
        }

        [TestMethod]
        public void EqualTest() {
            Vector vector1 = new(2, 3), vector2 = new(2, 3), vector3 = new(3, 2);

            Assert.AreEqual(vector1, vector2);
            Assert.AreNotEqual(vector1, vector3);

            Assert.IsFalse(vector1.Equals(null));

            Assert.IsTrue(vector1 == vector2);
            Assert.IsTrue(vector1 != vector3);
        }

        [TestMethod]
        public void ValidTest() {
            Vector vector1 = new(2, 3), vector2 = Vector.Invalid;

            Assert.AreNotEqual(vector1, vector2);
            Assert.IsTrue(vector1.IsValid);
            Assert.IsFalse(vector2.IsValid);
        }
    }
}
