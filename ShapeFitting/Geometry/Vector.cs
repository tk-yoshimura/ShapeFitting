using System;
using System.Collections.Generic;

namespace ShapeFitting {

    /// <summary>vector</summary>
    public struct Vector {
        public double X { set; get; }
        public double Y { set; get; }

        public Vector(double x, double y) {
            this.X = x;
            this.Y = y;
        }

        public bool IsValid => double.IsFinite(X) && double.IsFinite(Y);

        public static Vector Invalid => new(double.NaN, double.NaN);

        public static Vector operator +(Vector a, Vector b) {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b) {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector v, double r) {
            return new Vector(v.X * r, v.Y * r);
        }

        public static Vector operator *(double r, Vector v) {
            return v * r;
        }

        public static Vector operator /(Vector v, double r) {
            return v * (1d / r);
        }

        public double SquareNorm => X * X + Y * Y;
        public double Norm => Math.Sqrt(SquareNorm);

        public override bool Equals(object obj) {
            return obj is Vector vector && (vector == this);
        }

        public static bool operator ==(Vector a, Vector b) {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector a, Vector b) {
            return !(a == b);
        }

        public static implicit operator Vector((double x, double y) v) {
            return new Vector(v.x, v.y);
        }
        public void Deconstruct(out double x, out double y) => (x, y) = (X, Y);

        public static Vector[] Concat(IReadOnlyList<double> xs, IReadOnlyList<double> ys) {
            if (xs.Count != ys.Count) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            int n = xs.Count;

            Vector[] vs = new Vector[n];

            for (int i = 0; i < n; i++) {
                vs[i] = new Vector(xs[i], ys[i]);
            }

            return vs;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString() {
            return IsValid ? $"{X},{Y}" : nameof(Invalid);
        }

    }
}
