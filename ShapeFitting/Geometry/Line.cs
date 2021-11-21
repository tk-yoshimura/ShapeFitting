using System;
using System.Collections.Generic;

namespace ShapeFitting {

    /// <summary>line</summary>
    /// <remarks>
    /// ax + by + c = 0
    /// sin(theta) x + cos(theta) y + phi = 0
    /// </remarks>
    public struct Line {
        public double A { set; get; }
        public double B { set; get; }
        public double C { set; get; }

        public double Theta => Math.Atan2(A, B);
        public double Phi => C;

        public Line(double a, double b, double c) {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public Line(double theta, double phi) {
            this.A = Math.Sin(theta);
            this.B = Math.Cos(theta);
            this.C = phi;
        }

        public bool IsValid => double.IsFinite(A) && double.IsFinite(B) && double.IsFinite(C);

        public static Line Invalid => new(double.NaN, double.NaN, double.NaN);

        /// <summary>y = f(x)</summary>
        public double Fx(double x) => -(A * x + C) / B;

        /// <summary>y = f(x)</summary>
        public double[] Fx(double[] xs) {
            double[] ys = new double[xs.Length];
            for (int i = 0; i < xs.Length; i++) {
                ys[i] = -(A * xs[i] + C) / B;
            }

            return ys;
        }

        /// <summary>x = f(y)</summary>
        public double Fy(double y) => -(B * y + C) / A;

        /// <summary>x = f(y)</summary>
        public double[] Fy(double[] ys) {
            double[] xs = new double[ys.Length];
            for (int i = 0; i < ys.Length; i++) {
                xs[i] = -(B * ys[i] + C) / A;
            }

            return xs;
        }

        /// <summary>from y = slope x + y_intercept</summary>
        public static Line FromFx(double slope, double y_intercept) {
            return new Line(-slope, 1, -y_intercept);
        }

        /// <summary>from x = slope y + x_intercept</summary>
        public static Line FromFy(double slope, double x_intercept) {
            return new Line(1, -slope, -x_intercept);
        }

        public static Line FromPoints(Vector a, Vector b) {
            double dx = a.X - b.X, dy = a.Y - b.Y;

            if (Math.Abs(dx) > Math.Abs(dy)) {
                double slope = dy / dx, y_intercept = a.Y - slope * a.X;
                return FromFx(slope, y_intercept);
            }
            else {
                double slope = dx / dy, x_intercept = a.X - slope * a.Y;
                return FromFy(slope, x_intercept);
            }
        }

        public override bool Equals(object obj) {
            return obj is Line line && (line == this);
        }

        public static bool operator ==(Line a, Line b) {
            return a.A == b.A && a.B == b.B && a.C == b.C;
        }

        public static bool operator !=(Line a, Line b) {
            return !(a == b);
        }

        public static implicit operator Line((double a, double b, double c) line) {
            return new Line(line.a, line.b, line.c);
        }

        public static implicit operator Line((double theta, double phi) line) {
            return new Line(line.theta, line.phi);
        }

        public static implicit operator (double a, double b, double c)(Line line) {
            return (line.A, line.B, line.C);
        }

        public static implicit operator (double theta, double phi)(Line line) {
            return (line.Theta, line.Phi);
        }

        public void Deconstruct(out double a, out double b, out double c) => (a, b, c) = (A, B, C);

        public void Deconstruct(out double theta, out double phi) => (theta, phi) = (Theta, Phi);

        public static Line[] Concat(IReadOnlyList<double> a_list, IReadOnlyList<double> b_list, IReadOnlyList<double> c_list) {
            if (a_list.Count != b_list.Count || a_list.Count != c_list.Count) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            int n = a_list.Count;

            Line[] lines = new Line[n];
            for (int i = 0; i < n; i++) {
                lines[i] = new Line(a_list[i], b_list[i], c_list[i]);
            }

            return lines;
        }

        public static Line[] Concat(IReadOnlyList<double> theta_list, IReadOnlyList<double> phi_list) {
            if (theta_list.Count != phi_list.Count) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            int n = theta_list.Count;

            Line[] lines = new Line[n];
            for (int i = 0; i < n; i++) {
                lines[i] = new Line(theta_list[i], phi_list[i]);
            }

            return lines;
        }

        public static double[] Distance(IReadOnlyList<Vector> vs, double a, double b, double c) {
            double[] dists = new double[vs.Count];

            double n = Math.Sqrt(a * a + b * b);
            (a, b, c) = (a / n, b / n, c / n);

            for (int i = 0; i < vs.Count; i++) {
                (double x, double y) = vs[i];

                double dist = Math.Abs(a * x + b * y + c);
                dists[i] = dist;
            }

            return dists;
        }

        public override int GetHashCode() {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }

        public override string ToString() {
            if (!IsValid) {
                return nameof(Invalid);
            }

            return $"({Theta},{Phi})";
        }
    }
}
