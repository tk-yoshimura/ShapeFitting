using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    internal static class Order {
        public static (double, double) AbsSort(double x0, double x1) {
            return Math.Abs(x0) <= Math.Abs(x1) ? (x0, x1) : (x1, x0);
        }

        public static (double, double, double) AbsSort(double x0, double x1, double x2) {
            return Math.Abs(x0) <= Math.Abs(x1)
                ? (Math.Abs(x1) <= Math.Abs(x2) ? (x0, x1, x2)
                    : Math.Abs(x0) <= Math.Abs(x2) ? (x0, x2, x1) : (x2, x0, x1))
                : (Math.Abs(x0) <= Math.Abs(x2) ? (x1, x0, x2)
                    : Math.Abs(x1) <= Math.Abs(x2) ? (x1, x2, x0) : (x2, x1, x0));
        }

        public static IEnumerable<double> AbsSort(IEnumerable<double> xs) {
            return xs.OrderBy((x) => Math.Abs(x));
        }

        public static ((double, T), (double, T)) AbsSort<T>((double v, T) x0, (double v, T) x1) {
            return Math.Abs(x0.v) <= Math.Abs(x1.v) ? (x0, x1) : (x1, x0);
        }

        public static ((double, T), (double, T), (double, T)) AbsSort<T>((double v, T) x0, (double v, T) x1, (double v, T) x2) {
            return Math.Abs(x0.v) <= Math.Abs(x1.v)
                ? (Math.Abs(x1.v) <= Math.Abs(x2.v) ? (x0, x1, x2)
                    : Math.Abs(x0.v) <= Math.Abs(x2.v) ? (x0, x2, x1) : (x2, x0, x1))
                : (Math.Abs(x0.v) <= Math.Abs(x2.v) ? (x1, x0, x2)
                    : Math.Abs(x1.v) <= Math.Abs(x2.v) ? (x1, x2, x0) : (x2, x1, x0));
        }

        public static IEnumerable<(double, T)> AbsSort<T>(IEnumerable<(double v, T)> xs) {
            return xs.OrderBy((x) => Math.Abs(x.v));
        }

        public static (int, int) AbsArgSort(double x0, double x1) {
            return Math.Abs(x0) <= Math.Abs(x1) ? (0, 1) : (1, 0);
        }

        public static (int, int, int) AbsArgSort(double x0, double x1, double x2) {
            return Math.Abs(x0) <= Math.Abs(x1)
                ? (Math.Abs(x1) <= Math.Abs(x2) ? (0, 1, 2)
                    : Math.Abs(x0) <= Math.Abs(x2) ? (0, 2, 1) : (2, 0, 1))
                : (Math.Abs(x0) <= Math.Abs(x2) ? (1, 0, 2)
                    : Math.Abs(x1) <= Math.Abs(x2) ? (1, 2, 0) : (2, 1, 0));
        }
    }
}
