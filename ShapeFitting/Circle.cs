using System;

namespace ShapeFitting {

    /// <summary>circle</summary>
    /// <remarks>(x - cx)^2 + (y - cy)^2 = r^2</remarks>
    public struct Circle {
        public Vector Center { set; get; }
        public double Radius { set; get; }

        public Circle(Vector center, double radius) {
            this.Center = center;
            this.Radius = radius;
        }

        public Circle(double cx, double cy, double r) {
            this.Center = new Vector(cx, cy);
            this.Radius = r;
        }

        public bool IsValid => Center.IsValid && double.IsFinite(Radius) && Radius >= 0;

        public static Circle NaN => new(Vector.NaN, double.NaN);

        public static Circle FromPoints(Vector a, Vector b, Vector c) {
            Vector ab = a - b, bc = b - c, ca = c - a;

            double a_sqnorm = ab.SquareNorm, b_sqnorm = bc.SquareNorm, c_sqnorm = ca.SquareNorm;
            double a_norm = Math.Sqrt(a_sqnorm), b_norm = Math.Sqrt(b_sqnorm), c_norm = Math.Sqrt(c_sqnorm);

            double ra = a_sqnorm * (b_sqnorm + c_sqnorm - a_sqnorm);
            double rb = b_sqnorm * (c_sqnorm + a_sqnorm - b_sqnorm);
            double rc = c_sqnorm * (a_sqnorm + b_sqnorm - c_sqnorm);

            Vector center = (ra * c + rb * a + rc * b) / (ra + rb + rc);
            double radius = (a_norm * b_norm * c_norm) /
                Math.Sqrt(
                    (a_norm + b_norm + c_norm) * (-a_norm + b_norm + c_norm)
                    * (a_norm - b_norm + c_norm) * (a_norm + b_norm - c_norm)
                );

            return new Circle(center, radius);
        }

        public override bool Equals(object obj) {
            return obj is Circle circle && (circle == this);
        }

        public static bool operator ==(Circle a, Circle b) {
            return a.Center == b.Center && a.Radius == b.Radius;
        }

        public static bool operator !=(Circle a, Circle b) {
            return !(a == b);
        }

        public static implicit operator Circle((double cx, double cy, double r) circle) {
            return new Circle(circle.cx, circle.cy, circle.r);
        }

        public static implicit operator Circle((Vector c, double r) circle) {
            return new Circle(circle.c, circle.r);
        }

        public void Deconstruct(out double cx, out double cy, out double r) => (cx, cy, r) = (Center.X, Center.Y, Radius);

        public void Deconstruct(out Vector c, out double r) => (c, r) = (Center, Radius);

        public override int GetHashCode() {
            return Center.GetHashCode() ^ Radius.GetHashCode();
        }

        public override string ToString() {
            if (!IsValid) {
                return nameof(NaN);
            }

            string str = $"(x-{Center.X})^2+(y-{Center.Y})^2={Radius}^2";
            str = str.Replace("--", "+");

            return str;
        }
    }
}
