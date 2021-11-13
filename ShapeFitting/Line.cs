using System;
using System.Collections.Generic;
using System.Linq;

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

        public static Line NaN => new(double.NaN, double.NaN, double.NaN);

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

        public void Deconstruct(out double a, out double b, out double c) => (a, b, c) = (A, B, C);

        public void Deconstruct(out double theta, out double phi) => (theta, phi) = (Theta, Phi);

        public static IEnumerable<Line> Concat(IEnumerable<double> a_list, IEnumerable<double> b_list, IEnumerable<double> c_list) {
            if (a_list.Count() != b_list.Count() || a_list.Count() != c_list.Count()) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            using var ea = a_list.GetEnumerator();
            using var eb = b_list.GetEnumerator();
            using var ec = c_list.GetEnumerator();

            while (ea.MoveNext() && eb.MoveNext() && ec.MoveNext()) {
                yield return new Line(ea.Current, eb.Current, ec.Current);
            }
        }

        public static IEnumerable<Line> Concat(IEnumerable<double> theta_list, IEnumerable<double> phi_list) {
            if (theta_list.Count() != phi_list.Count()) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            return theta_list.Zip(phi_list, (theta, phi) => new Line(theta, phi));
        }

        public override int GetHashCode() {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }

        public override string ToString() {
            static string tostr(double value, string suffix) {
                if (value == 0) {
                    return string.Empty;
                }
                if (Math.Abs(value) == 1 && suffix.Length > 0) {
                    return (value > 0) ? $"+{suffix}" : $"-{suffix}";
                }

                return (value > 0) ? $"+{value}{suffix}" : $"{value}{suffix}";
            }

            if (!IsValid) {
                return nameof(NaN);
            }

            string str = $"{tostr(A, "x")}{tostr(B, "y")}{tostr(C, string.Empty)}=0";

            return (str[0] == '+') ? str[1..] : str;
        }
    }
}
