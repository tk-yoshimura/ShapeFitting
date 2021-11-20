using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    internal static class Summator {

        public static (double sx, double sy,
                       double sx2, double sxy, double sy2)
            D2(IReadOnlyList<Vector> vs) {

            double sx = 0, sy = 0,
                   sx2 = 0, sxy = 0, sy2 = 0;

            foreach ((double x, double y) in vs) {
                sx += x;
                sy += y;

                sx2 += x * x;
                sxy += x * y;
                sy2 += y * y;
            }

            return (sx, sy,
                    sx2, sxy, sy2);
        }

        public static (double sw,
                       double swx, double swy,
                       double swx2, double swxy, double swy2)
            D2(IReadOnlyList<Vector> vs, IReadOnlyList<double> weights) {

            if (vs.Count != weights.Count) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            double sw = 0,
                   swx = 0, swy = 0,
                   swx2 = 0, swxy = 0, swy2 = 0;

            foreach (((double x, double y), double w) in vs.Zip(weights)) {
                sw += w;

                swx += w * x;
                swy += w * y;

                swx2 += w * x * x;
                swxy += w * x * y;
                swy2 += w * y * y;
            }

            return (sw,
                    swx, swy,
                    swx2, swxy, swy2);
        }

        public static (double sx, double sy,
                       double sx2, double sxy, double sy2,
                       double sx3, double sx2y, double sxy2, double sy3)
            D3(IReadOnlyList<Vector> vs) {

            double sx = 0, sy = 0,
                   sx2 = 0, sxy = 0, sy2 = 0,
                   sx3 = 0, sx2y = 0, sxy2 = 0, sy3 = 0;

            foreach ((double x, double y) in vs) {
                double x2 = x * x, x3 = x * x2;
                double y2 = y * y, y3 = y * y2;

                sx += x;
                sy += y;

                sx2 += x2;
                sxy += x * y;
                sy2 += y2;

                sx3 += x3;
                sx2y += x2 * y;
                sxy2 += x * y2;
                sy3 += y3;
            }

            return (sx, sy,
                    sx2, sxy, sy2,
                    sx3, sx2y, sxy2, sy3);
        }

        public static (double sw,
                       double swx, double swy,
                       double swx2, double swxy, double swy2,
                       double swx3, double swx2y, double swxy2, double swy3)
            D3(IReadOnlyList<Vector> vs, IReadOnlyList<double> weights) {

            if (vs.Count != weights.Count) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            double sw = 0,
                   swx = 0, swy = 0,
                   swx2 = 0, swxy = 0, swy2 = 0,
                   swx3 = 0, swx2y = 0, swxy2 = 0, swy3 = 0;

            foreach (((double x, double y), double w) in vs.Zip(weights)) {
                double x2 = x * x, x3 = x * x2;
                double y2 = y * y, y3 = y * y2;

                sw += w;

                swx += w * x;
                swy += w * y;

                swx2 += w * x2;
                swxy += w * x * y;
                swy2 += w * y2;

                swx3 += w * x3;
                swx2y += w * x2 * y;
                swxy2 += w * x * y2;
                swy3 += w * y3;
            }

            return (sw,
                    swx, swy,
                    swx2, swxy, swy2,
                    swx3, swx2y, swxy2, swy3);
        }

        public static (double sx, double sy,
                       double sx2, double sxy, double sy2,
                       double sx3, double sx2y, double sxy2, double sy3,
                       double sx4, double sx3y, double sx2y2, double sxy3, double sy4)
            D4(IReadOnlyList<Vector> vs) {

            double sx = 0, sy = 0,
                   sx2 = 0, sxy = 0, sy2 = 0,
                   sx3 = 0, sx2y = 0, sxy2 = 0, sy3 = 0,
                   sx4 = 0, sx3y = 0, sx2y2 = 0, sxy3 = 0, sy4 = 0;

            foreach ((double x, double y) in vs) {
                double x2 = x * x, x3 = x * x2, x4 = x * x3;
                double y2 = y * y, y3 = y * y2, y4 = y * y3;

                sx += x;
                sy += y;

                sx2 += x2;
                sxy += x * y;
                sy2 += y2;

                sx3 += x3;
                sx2y += x2 * y;
                sxy2 += x * y2;
                sy3 += y3;

                sx4 += x4;
                sx3y += x3 * y;
                sx2y2 += x2 * y2;
                sxy3 += x * y3;
                sy4 += y4;
            }

            return (sx, sy,
                    sx2, sxy, sy2,
                    sx3, sx2y, sxy2, sy3,
                    sx4, sx3y, sx2y2, sxy3, sy4);
        }

        public static (double sw,
                       double swx, double swy,
                       double swx2, double swxy, double swy2,
                       double swx3, double swx2y, double swxy2, double swy3,
                       double swx4, double swx3y, double swx2y2, double swxy3, double swy4)
            D4(IReadOnlyList<Vector> vs, IReadOnlyList<double> weights) {

            if (vs.Count != weights.Count) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            double sw = 0,
                   swx = 0, swy = 0,
                   swx2 = 0, swxy = 0, swy2 = 0,
                   swx3 = 0, swx2y = 0, swxy2 = 0, swy3 = 0,
                   swx4 = 0, swx3y = 0, swx2y2 = 0, swxy3 = 0, swy4 = 0;

            foreach (((double x, double y), double w) in vs.Zip(weights)) {
                double x2 = x * x, x3 = x * x2, x4 = x * x3;
                double y2 = y * y, y3 = y * y2, y4 = y * y3;

                sw += w;

                swx += w * x;
                swy += w * y;

                swx2 += w * x2;
                swxy += w * x * y;
                swy2 += w * y2;

                swx3 += w * x3;
                swx2y += w * x2 * y;
                swxy2 += w * x * y2;
                swy3 += w * y3;

                swx4 += w * x4;
                swx3y += w * x3 * y;
                swx2y2 += w * x2 * y2;
                swxy3 += w * x * y3;
                swy4 += w * y4;
            }

            return (sw,
                    swx, swy,
                    swx2, swxy, swy2,
                    swx3, swx2y, swxy2, swy3,
                    swx4, swx3y, swx2y2, swxy3, swy4);
        }
    }
}
