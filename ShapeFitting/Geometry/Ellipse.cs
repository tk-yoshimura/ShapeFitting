using System;
using System.Collections.Generic;

namespace ShapeFitting {

    /// <summary>circle</summary>
    /// <remarks>a x^2 + b x y + c y^2 + d x + e y + f = 0</remarks>
    public struct Ellipse {
        public Vector Center { set; get; }
        public (double major, double minor) Axis { set; get; }
        public double Angle { set; get; }

        public Ellipse(Vector center, (double major, double minor) axis, double angle) {
            this.Center = center;
            this.Axis = axis;
            this.Angle = angle;
        }

        public bool IsValid => Center.IsValid
            && double.IsFinite(Axis.major) && double.IsFinite(Axis.minor)
            && double.IsFinite(Angle);

        public static Ellipse Invalid => new(Vector.Invalid, (double.NaN, double.NaN), double.NaN);

        /// <summary>from a x^2 + b x y + c y^2 + d x + e y + f = 0</summary>
        public static Ellipse FromImplicit(double a, double b, double c, double d, double e, double f) {
            if (4 * a * c - b * b <= 0) {
                return Invalid;
            }

            double angle = (b == 0 && a == c) ? 0 : Math.Atan2(b, a - c) / 2;

            double cs = Math.Cos(angle), sn = Math.Sin(angle);
            double sqcs = cs * cs, sqsn = sn * sn, cssn = cs * sn;

            (a, c, d, e) =
                (a * sqcs + b * cssn + c * sqsn, a * sqsn - b * cssn + c * sqcs,
                 e * sn + d * cs, e * cs - d * sn);
            angle += (a > c) ? Math.PI / 2 : 0;

            double x = -d / (2 * a), y = -e / (2 * c);

            double gamma = d * d / (4 * a) + e * e / (4 * c) - f;

            double major_axis = Math.Sqrt(gamma / c);
            double minor_axis = Math.Sqrt(gamma / a);

            double cx = x * cs - y * sn;
            double cy = x * sn + y * cs;

            return new Ellipse(new Vector(cx, cy), (major_axis, minor_axis), angle);
        }

        public Vector Point(double theta) {
            double cs = Math.Cos(Angle), sn = Math.Sin(Angle);
            double a = Math.Cos(theta) * Axis.major, b = Math.Sin(theta) * Axis.minor;

            return Center + new Vector(cs * a - sn * b, sn * a + cs * b);
        }

        public Vector[] Points(IReadOnlyList<double> thetas) {
            Vector[] vs = new Vector[thetas.Count];

            double cs = Math.Cos(Angle), sn = Math.Sin(Angle);

            for (int i = 0; i < thetas.Count; i++) {
                double theta = thetas[i];

                double a = Math.Cos(theta) * Axis.major, b = Math.Sin(theta) * Axis.minor;

                vs[i] = Center + new Vector(cs * a - sn * b, sn * a + cs * b);
            }

            return vs;
        }

        public override bool Equals(object obj) {
            return obj is Ellipse ellipse && (ellipse == this);
        }

        public static bool operator ==(Ellipse a, Ellipse b) {
            return a.Center == b.Center && a.Axis == b.Axis && a.Angle == b.Angle;
        }

        public static bool operator !=(Ellipse a, Ellipse b) {
            return !(a == b);
        }

        public static implicit operator Ellipse((double cx, double cy, double rx, double ry, double angle) ellipse) {
            return new Ellipse(new Vector(ellipse.cx, ellipse.cy), (ellipse.rx, ellipse.ry), ellipse.angle);
        }

        public static implicit operator Ellipse((Vector center, (double major, double minor) axis, double angle) ellipse) {
            return new Ellipse(ellipse.center, ellipse.axis, ellipse.angle);
        }

        public static implicit operator (double cx, double cy, double rx, double ry, double angle)(Ellipse ellipse) {
            return (ellipse.Center.X, ellipse.Center.Y, ellipse.Axis.major, ellipse.Axis.minor, ellipse.Angle);
        }

        public static implicit operator (Vector center, (double major, double minor) axis, double angle)(Ellipse ellipse) {
            return (ellipse.Center, ellipse.Axis, ellipse.Angle);
        }

        public void Deconstruct(out double cx, out double cy, out double rx, out double ry, out double angle)
            => (cx, cy, rx, ry, angle) = (Center.X, Center.Y, Axis.major, Axis.minor, Angle);

        public void Deconstruct(out Vector center, out (double major, double minor) axis, out double angle)
            => (center, axis, angle) = (Center, Axis, Angle);

        public static double[] Distance(IReadOnlyList<Vector> vs, double a, double b, double c, double d, double e, double f) {
            double[] dists = new double[vs.Count];

            double bias = f - (a * e * e - b * d * e + c * d * d) / (4 * a * c - b * b), sq_bias = Math.Sqrt(-bias);

            for (int i = 0; i < vs.Count; i++) {
                (double x, double y) = vs[i];

                double dist = Math.Abs(Math.Sqrt(Math.Abs((a * x + d) * x + (c * y + e) * y + b * x * y + f - bias)) - sq_bias);
                dists[i] = dist;
            }

            return dists;
        }

        public override int GetHashCode() {
            return Center.GetHashCode() ^ Axis.GetHashCode() ^ Angle.GetHashCode();
        }

        public override string ToString() {
            if (!IsValid) {
                return nameof(Invalid);
            }

            return $"({Center}),{Axis},{Angle}";
        }
    }
}
