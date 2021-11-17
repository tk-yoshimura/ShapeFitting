using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    internal static class Summator {

        public static (double sx, double sy,
                       double sx2, double sxy, double sy2)
            D2(IEnumerable<Vector> vs) {

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
            D2(IEnumerable<Vector> vs, IEnumerable<double> weights) {

            if (vs.Count() != weights.Count()) {
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
            D3(IEnumerable<Vector> vs) {

            double sx = 0, sy = 0,
                   sx2 = 0, sxy = 0, sy2 = 0,
                   sx3 = 0, sx2y = 0, sxy2 = 0, sy3 = 0;

            foreach ((double x, double y) in vs) {
                sx += x;
                sy += y;

                sx2 += x * x;
                sxy += x * y;
                sy2 += y * y;

                sx3 += x * x * x;
                sx2y += x * x * y;
                sxy2 += x * y * y;
                sy3 += y * y * y;
            }

            return (sx, sy,
                    sx2, sxy, sy2,
                    sx3, sx2y, sxy2, sy3);
        }

        public static (double sw,
                       double swx, double swy,
                       double swx2, double swxy, double swy2,
                       double swx3, double swx2y, double swxy2, double swy3)
            D3(IEnumerable<Vector> vs, IEnumerable<double> weights) {

            if (vs.Count() != weights.Count()) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            double sw = 0,
                   swx = 0, swy = 0,
                   swx2 = 0, swxy = 0, swy2 = 0,
                   swx3 = 0, swx2y = 0, swxy2 = 0, swy3 = 0;

            foreach (((double x, double y), double w) in vs.Zip(weights)) {
                sw += w;

                swx += w * x;
                swy += w * y;

                swx2 += w * x * x;
                swxy += w * x * y;
                swy2 += w * y * y;

                swx3 += w * x * x * x;
                swx2y += w * x * x * y;
                swxy2 += w * x * y * y;
                swy3 += w * y * y * y;
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
            D4(IEnumerable<Vector> vs) {

            double sx = 0, sy = 0,
                   sx2 = 0, sxy = 0, sy2 = 0,
                   sx3 = 0, sx2y = 0, sxy2 = 0, sy3 = 0,
                   sx4 = 0, sx3y = 0, sx2y2 = 0, sxy3 = 0, sy4 = 0;

            foreach ((double x, double y) in vs) {
                sx += x;
                sy += y;

                sx2 += x * x;
                sxy += x * y;
                sy2 += y * y;

                sx3 += x * x * x;
                sx2y += x * x * y;
                sxy2 += x * y * y;
                sy3 += y * y * y;

                sx4 += x * x * x * x;
                sx3y += x * x * x * y;
                sx2y2 += x * x * y * y;
                sxy3 += x * y * y * y;
                sy4 += y * y * y * y;
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
            D4(IEnumerable<Vector> vs, IEnumerable<double> weights) {

            if (vs.Count() != weights.Count()) {
                throw new ArgumentException(ExceptionMessage.MismatchLength);
            }

            double sw = 0,
                   swx = 0, swy = 0,
                   swx2 = 0, swxy = 0, swy2 = 0,
                   swx3 = 0, swx2y = 0, swxy2 = 0, swy3 = 0,
                   swx4 = 0, swx3y = 0, swx2y2 = 0, swxy3 = 0, swy4 = 0;

            foreach (((double x, double y), double w) in vs.Zip(weights)) {
                sw += w;

                swx += w * x;
                swy += w * y;

                swx2 += w * x * x;
                swxy += w * x * y;
                swy2 += w * y * y;

                swx3 += w * x * x * x;
                swx2y += w * x * x * y;
                swxy2 += w * x * y * y;
                swy3 += w * y * y * y;

                swx4 += w * x * x * x * x;
                swx3y += w * x * x * x * y;
                swx2y2 += w * x * x * y * y;
                swxy3 += w * x * y * y * y;
                swy4 += w * y * y * y * y;
            }

            return (sw,
                    swx, swy,
                    swx2, swxy, swy2,
                    swx3, swx2y, swxy2, swy3,
                    swx4, swx3y, swx2y2, swxy3, swy4);
        }
    }
}
