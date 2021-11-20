using System;
using System.Collections.Generic;

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

        /// <summary>from x^2 + y^2 + ax + by + c = 0</summary>
        public static Circle FromImplicit(double a, double b, double c) {
            double cx = -a / 2;
            double cy = -b / 2;
            double r = Math.Sqrt(cx * cx + cy * cy - c);

            return new Circle(cx, cy, r);
        }

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

        public Vector Point(double theta) {
            return Center + new Vector(Math.Cos(theta) * Radius, Math.Sin(theta) * Radius);
        }

        public Vector[] Points(IReadOnlyList<double> thetas) {
            Vector[] vs = new Vector[thetas.Count];

            for (int i = 0; i < thetas.Count; i++) {
                double theta = thetas[i];
                vs[i] = Center + new Vector(Math.Cos(theta) * Radius, Math.Sin(theta) * Radius);
            }

            return vs;
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

        public static implicit operator (double cx, double cy, double r)(Circle circle) {
            return (circle.Center.X, circle.Center.Y, circle.Radius);
        }

        public static implicit operator (Vector c, double r)(Circle circle) {
            return (circle.Center, circle.Radius);
        }

        public static explicit operator Ellipse(Circle circle) {
            return new Ellipse(circle.Center, (circle.Radius, circle.Radius), 0);
        }

        public void Deconstruct(out double cx, out double cy, out double r) => (cx, cy, r) = (Center.X, Center.Y, Radius);

        public void Deconstruct(out Vector c, out double r) => (c, r) = (Center, Radius);

        public static double[] Distance(IReadOnlyList<Vector> vs, double a, double b, double c) {
            double[] dists = new double[vs.Count];

            double bias = c - (a * a + b * b) / 4, sq_bias = Math.Sqrt(-bias);

            for (int i = 0; i < vs.Count; i++) {
                (double x, double y) = vs[i];

                double dist = Math.Abs(Math.Sqrt(Math.Abs((x + a) * x + (y + b) * y + c - bias)) - sq_bias);
                dists[i] = dist;
            }

            return dists;
        }

        public override int GetHashCode() {
            return Center.GetHashCode() ^ Radius.GetHashCode();
        }

        public override string ToString() {
            if (!IsValid) {
                return nameof(NaN);
            }

            return $"({Center}),{Radius}";
        }
    }
}
