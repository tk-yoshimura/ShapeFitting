using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFittingTest {
    [TestClass]
    public class FittingBenchmarkTests {
        private Vector[] line_points, circle_points, ellipse_points;

        public FittingBenchmarkTests() {
            Line line = new(1, 0.5);
            double[] xs = (new double[513]).Select((_, i) => (i - 256) / 256d).ToArray();
            double[] ys = line.Fx(xs);
            line_points = Vector.Concat(xs, ys).ToArray();

            List<double> thetas = new();
            for (decimal theta = 0; theta < 6.29m; theta += 0.01m) {
                thetas.Add((double)theta);
            }

            Circle circle = new((2, 3), 4);
            circle_points = circle.Points(thetas).ToArray();

            Ellipse ellipse = new((2, 3), (4, 1), 0.5);
            ellipse_points = ellipse.Points(thetas).ToArray();
        }

        public Vector[] RandomShift(IReadOnlyList<Vector> points, double prob, double range, Random random) {
            Vector[] vs = new Vector[points.Count];

            for (int i = 0; i < points.Count; i++) {
                Vector v = points[i];

                if (random.NextDouble() < prob) {
                    vs[i] = v +
                        new Vector(
                            random.NextDouble() * range * 2 - range,
                            random.NextDouble() * range * 2 - range
                        );
                }
                else {
                    vs[i] = v;
                }
            }

            return vs;
        }

        public Vector[] FixedShift(IReadOnlyList<Vector> points, double prob, double range, Random random) {
            double theta = random.NextDouble() * (2 * Math.PI);

            Vector sft = new Vector(
                Math.Cos(theta) * range,
                Math.Sin(theta) * range
            );

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
        public void FitLineRandomShiftMAEBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MAEFitting.FitLine(RandomShift(line_points, prob, range, random));

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
        public void FitLineRandomShiftMSEBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MSEFitting.FitLine(RandomShift(line_points, prob, range, random));

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
        public void FitLineRandomShiftHuberMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomShift(line_points, prob, range, random), new HuberMAD(min_scale: 0.01));

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
        public void FitLineRandomShiftHuberAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomShift(line_points, prob, range, random), new HuberAAD(min_scale: 0.01));

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
        public void FitLineRandomShiftTukeyMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomShift(line_points, prob, range, random), new TukeyMAD(min_scale: 0.01));

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
        public void FitLineRandomShiftTukeyAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomShift(line_points, prob, range, random), new TukeyAAD(min_scale: 0.01));

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
        public void FitCircleRandomShiftMAEBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MAEFitting.FitCircle(RandomShift(circle_points, prob, range, random));

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
        public void FitCircleRandomShiftMSEBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MSEFitting.FitCircle(RandomShift(circle_points, prob, range, random));

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
        public void FitCircleRandomShiftHuberMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomShift(circle_points, prob, range, random), new HuberMAD(min_scale: 0.01));

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
        public void FitCircleRandomShiftHuberMedianBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomShift(circle_points, prob, range, random), new HuberMedian(k: 1.44, min_scale: 0.01));

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
        public void FitCircleRandomShiftHuberAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomShift(circle_points, prob, range, random), new HuberAAD(min_scale: 0.01));

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
        public void FitCircleTukeyMADRandomShiftBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomShift(circle_points, prob, range, random), new TukeyMAD(min_scale: 0.01));

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
        public void FitCircleRandomShiftTukeyAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomShift(circle_points, prob, range, random), new TukeyAAD(min_scale: 0.01));

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
        public void FitEllipseRandomShiftMAEBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MAEFitting.FitEllipse(RandomShift(ellipse_points, prob, range, random));

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
        public void FitEllipseRandomShiftMSEBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MSEFitting.FitEllipse(RandomShift(ellipse_points, prob, range, random));

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
        public void FitEllipseRandomShiftHuberMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomShift(ellipse_points, prob, range, random), new HuberMAD(min_scale: 0.01));

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
        public void FitEllipseRandomShiftHuberAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomShift(ellipse_points, prob, range, random), new HuberAAD(min_scale: 0.01));

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
        public void FitEllipseRandomShiftTukeyMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomShift(ellipse_points, prob, range, random), new TukeyMAD(min_scale: 0.01));

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
        public void FitEllipseRandomShiftTukeyAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomShift(ellipse_points, prob, range, random), new TukeyAAD(min_scale: 0.01));

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
        public void FitLineFixedShiftMAEBenchmarkTest() {
            const int tests = 4;

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
            const int tests = 4;

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
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new HuberMAD(min_scale: 0.01));

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
        public void FitLineFixedShiftHuberAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new HuberAAD(min_scale: 0.01));

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
        public void FitLineFixedShiftTukeyMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new TukeyMAD(min_scale: 0.01));

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
        public void FitLineFixedShiftTukeyAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(FixedShift(line_points, prob, range, random), new TukeyAAD(min_scale: 0.01));

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
        public void FitCircleFixedShiftMAEBenchmarkTest() {
            const int tests = 4;

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
            const int tests = 4;

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
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new HuberMAD(min_scale: 0.01));

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
        public void FitCircleFixedShiftHuberMedianBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new HuberMedian(k: 1.44, min_scale: 0.01));

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
        public void FitCircleFixedShiftHuberAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new HuberAAD(min_scale: 0.01));

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
        public void FitCircleFixedShiftTukeyMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new TukeyMAD(min_scale: 0.01));

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
        public void FitCircleFixedShiftTukeyMedianBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new TukeyMedian(c: 5, min_scale: 0.01));

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
        public void FitCircleFixedShiftTukeyAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(FixedShift(circle_points, prob, range, random), new TukeyAAD(min_scale: 0.01));

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
        public void FitEllipseFixedShiftMAEBenchmarkTest() {
            const int tests = 4;

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
            const int tests = 4;

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
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new HuberMAD(min_scale: 0.01));

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
        public void FitEllipseFixedShiftHuberAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new HuberAAD(min_scale: 0.01));

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
        public void FitEllipseFixedShiftTukeyMADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new TukeyMAD(min_scale: 0.01));

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
        public void FitEllipseFixedShiftTukeyAADBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.01, 0.05, 0.1, 0.2 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(FixedShift(ellipse_points, prob, range, random), new TukeyAAD(min_scale: 0.01));

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
    }
}
