using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFittingBenchmark {
    [TestClass]
    public class FittingBenchmarkLessPointsTests {
        private readonly Vector[] line_points, circle_points, ellipse_points;

        public FittingBenchmarkLessPointsTests() {
            Line line = new(1, 0.5);
            double[] xs = (new double[17]).Select((_, i) => (i - 16) / 16d).ToArray();
            double[] ys = line.Fx(xs);
            line_points = Vector.Concat(xs, ys).ToArray();

            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.29m; theta += 0.5m) {
                thetas.Add((double)theta);
            }

            Circle circle = new((2, 3), 4);
            circle_points = circle.Points(thetas).ToArray();

            Ellipse ellipse = new((2, 3), (4, 1), 0.5);
            ellipse_points = ellipse.Points(thetas).ToArray();
        }

        public Vector[] URandomShift(IReadOnlyList<Vector> points, double prob, double range, Random random) {
            Vector[] vs = new Vector[points.Count];

            for (int i = 0; i < points.Count; i++) {
                Vector v = points[i];

                if (random.NextDouble() < prob) {
                    vs[i] = v + new Vector(random.NextDouble() * range * 2 - range, random.NextDouble() * range * 2 - range);
                }
                else {
                    vs[i] = v;
                }
            }

            return vs;
        }

        public Vector[] NRandomShift(IReadOnlyList<Vector> points, double prob, double range, Random random) {
            Vector[] vs = new Vector[points.Count];

            static double nrandom(Random random) {
                double r1 = Math.Max(double.Epsilon, random.NextDouble());
                double r2 = Math.Max(double.Epsilon, random.NextDouble());

                return Math.Sqrt(-2 * Math.Log(r1)) * Math.Cos(2 * Math.PI * r2);
            }

            for (int i = 0; i < points.Count; i++) {
                Vector v = points[i];

                if (random.NextDouble() < prob) {
                    vs[i] = v + new Vector(nrandom(random) * range, nrandom(random) * range);
                }
                else {
                    vs[i] = v;
                }
            }

            return vs;
        }

        public Vector[] FixedShift(IReadOnlyList<Vector> points, double prob, double range, Random random) {
            double theta = random.NextDouble() * (2 * Math.PI);

            Vector sft = new Vector(Math.Cos(theta), Math.Sin(theta)) * range;

            Vector[] vs = new Vector[points.Count];

            for (int i = 0; i < points.Count; i++) {
                Vector v = points[i];

                if (random.NextDouble() < prob) {
                    vs[i] = v + sft;
                }
                else {
                    vs[i] = v;
                }
            }

            return vs;
        }

        [TestMethod]
        public void FitLineURandomShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MAEFitting.FitLine(URandomShift(line_points, prob, range, random));

                        (double theta, double phi) = line;

                        Assert.IsTrue(line.IsValid);

                        bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitLineURandomShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MSEFitting.FitLine(URandomShift(line_points, prob, range, random));

                        (double theta, double phi) = line;

                        Assert.IsTrue(line.IsValid);

                        bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitLineURandomShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {
                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(URandomShift(line_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineURandomShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(URandomShift(line_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                            Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineURandomShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(URandomShift(line_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineURandomShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(URandomShift(line_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineURandomShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(URandomShift(line_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineURandomShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(URandomShift(line_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleURandomShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MAEFitting.FitCircle(URandomShift(circle_points, prob, range, random));

                        (double cx, double cy, double r) = circle;

                        Assert.IsTrue(circle.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitCircleURandomShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MSEFitting.FitCircle(URandomShift(circle_points, prob, range, random));

                        (double cx, double cy, double r) = circle;

                        Assert.IsTrue(circle.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitCircleURandomShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(URandomShift(circle_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleURandomShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(URandomShift(circle_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleURandomShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(URandomShift(circle_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleURandomShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(URandomShift(circle_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleURandomShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(URandomShift(circle_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }


        [TestMethod]
        public void FitCircleURandomShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(URandomShift(circle_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseURandomShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MAEFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random));

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        Assert.IsTrue(ellipse.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitEllipseURandomShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MSEFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random));

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        Assert.IsTrue(ellipse.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif                    
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitEllipseURandomShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseURandomShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseURandomShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseURandomShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseURandomShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseURandomShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(URandomShift(ellipse_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineNRandomShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MAEFitting.FitLine(NRandomShift(line_points, prob, range, random));

                        (double theta, double phi) = line;

                        Assert.IsTrue(line.IsValid);

                        bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitLineNRandomShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MSEFitting.FitLine(NRandomShift(line_points, prob, range, random));

                        (double theta, double phi) = line;

                        Assert.IsTrue(line.IsValid);

                        bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitLineNRandomShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(NRandomShift(line_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineNRandomShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(NRandomShift(line_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineNRandomShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(NRandomShift(line_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineNRandomShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(NRandomShift(line_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineNRandomShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(NRandomShift(line_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineNRandomShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(NRandomShift(line_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleNRandomShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MAEFitting.FitCircle(NRandomShift(circle_points, prob, range, random));

                        (double cx, double cy, double r) = circle;

                        Assert.IsTrue(circle.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitCircleNRandomShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MSEFitting.FitCircle(NRandomShift(circle_points, prob, range, random));

                        (double cx, double cy, double r) = circle;

                        Assert.IsTrue(circle.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitCircleNRandomShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(NRandomShift(circle_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleNRandomShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(NRandomShift(circle_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleNRandomShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(NRandomShift(circle_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleNRandomShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(NRandomShift(circle_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleNRandomShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(NRandomShift(circle_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleNRandomShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(NRandomShift(circle_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseNRandomShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MAEFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random));

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        Assert.IsTrue(ellipse.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitEllipseNRandomShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MSEFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random));

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        Assert.IsTrue(ellipse.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif                    
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitEllipseNRandomShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseNRandomShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseNRandomShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseNRandomShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseNRandomShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseNRandomShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(NRandomShift(ellipse_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineFixedShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MAEFitting.FitLine(FixedShift(line_points, prob, range, random));

                        (double theta, double phi) = line;

                        Assert.IsTrue(line.IsValid);

                        bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitLineFixedShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MSEFitting.FitLine(FixedShift(line_points, prob, range, random));

                        (double theta, double phi) = line;

                        Assert.IsTrue(line.IsValid);

                        bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitLineFixedShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineFixedShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineFixedShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineFixedShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineFixedShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitLineFixedShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double theta, double phi) = line;

                            Assert.IsTrue(line.IsValid);

                            bool miss = Math.Abs(theta - 1) > 0.1 || Math.Abs(phi - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleFixedShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MAEFitting.FitCircle(FixedShift(circle_points, prob, range, random));

                        (double cx, double cy, double r) = circle;

                        Assert.IsTrue(circle.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitCircleFixedShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MSEFitting.FitCircle(FixedShift(circle_points, prob, range, random));

                        (double cx, double cy, double r) = circle;

                        Assert.IsTrue(circle.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitCircleFixedShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleFixedShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleFixedShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleFixedShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleFixedShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitCircleFixedShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double cx, double cy, double r) = circle;

                            Assert.IsTrue(circle.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseFixedShiftMAEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MAEFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random));

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        Assert.IsTrue(ellipse.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitEllipseFixedShiftMSEBenchmarkTest() {
            const int tests = 8;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MSEFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random));

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        Assert.IsTrue(ellipse.IsValid);

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void FitEllipseFixedShiftHuberMADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new HuberMAD(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseFixedShiftHuberAADBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new HuberAAD(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseFixedShiftHuberMedianBenchmarkTest() {
            const int tests = 8;

            for (double k = 0.125; k <= 8; k += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new HuberMedian(k, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"k = {k} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseFixedShiftTukeyMADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new TukeyMAD(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseFixedShiftTukeyAADBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new TukeyAAD(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }

        [TestMethod]
        public void FitEllipseFixedShiftTukeyMedianBenchmarkTest() {
            const int tests = 8;

            for (double c = 0.125; c <= 8; c += 0.125) {

                Random random = new(1234);

                int mistakes = 0;

                foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                    foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                        for (int i = 0; i < tests; i++) {
                            Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new TukeyMedian(c, min_scale: 0.01));

                            (double cx, double cy, double rx, double ry, double angle) = ellipse;

                            Assert.IsTrue(ellipse.IsValid);

                            bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                        Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                            if (miss) {
                                mistakes++;
                            }

#if DEBUG
                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
#endif
                        }
                    }
                }

                Console.WriteLine($"c = {c} mistakes = {mistakes}");
            }
        }
    }
}
