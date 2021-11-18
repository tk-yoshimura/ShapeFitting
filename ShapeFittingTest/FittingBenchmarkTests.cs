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

        public IEnumerable<Vector> RandomShift(IEnumerable<Vector> points, double prob, double range, Random random) {
            foreach (Vector v in points) {
                if (random.NextDouble() < prob) {
                    yield return v +
                        new Vector(
                            random.NextDouble() * range * 2 - range,
                            random.NextDouble() * range * 2 - range
                        );
                }
                else {
                    yield return v;
                }
            }
        }

        public IEnumerable<Vector> FixedShift(IEnumerable<Vector> points, double prob, double range, Random random) {
            double theta = random.NextDouble() * (2 * Math.PI);

            Vector sft = new Vector(
                Math.Cos(theta) * range,
                Math.Sin(theta) * range
            );

            foreach (Vector v in points) {
                if (random.NextDouble() < prob) {
                    yield return v + sft;
                }
                else {
                    yield return v;
                }
            }
        }

        [TestMethod]
        public void MAEFitLineRandomShiftBenchmarkTest() {
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

                        Console.WriteLine($"prob={prob} range={range} line={line} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void MSEFitLineRandomShiftBenchmarkTest() {
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
        public void HuberMADFitLineRandomShiftBenchmarkTest() {
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
        public void HuberAADFitLineRandomShiftBenchmarkTest() {
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
        public void TukeyMADFitLineRandomShiftBenchmarkTest() {
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
        public void TukeyAADFitLineRandomShiftBenchmarkTest() {
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
        public void MAEFitCircleRandomShiftBenchmarkTest() {
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
        public void MSEFitCircleRandomShiftBenchmarkTest() {
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
        public void HuberMADFitCircleRandomShiftBenchmarkTest() {
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
        public void HuberAADFitCircleRandomShiftBenchmarkTest() {
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
        public void TukeyMADFitCircleRandomShiftBenchmarkTest() {
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
        public void TukeyAADFitCircleRandomShiftBenchmarkTest() {
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
        public void MAEFitEllipseRandomShiftBenchmarkTest() {
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
        public void MSEFitEllipseRandomShiftBenchmarkTest() {
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
        public void HuberMADFitEllipseRandomShiftBenchmarkTest() {
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
        public void HuberAADFitEllipseRandomShiftBenchmarkTest() {
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
        public void TukeyMADFitEllipseRandomShiftBenchmarkTest() {
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
        public void TukeyAADFitEllipseRandomShiftBenchmarkTest() {
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
        public void MAEFitLineFixedShiftBenchmarkTest() {
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
        public void MSEFitLineFixedShiftBenchmarkTest() {
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
        public void HuberMADFitLineFixedShiftBenchmarkTest() {
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
        public void HuberAADFitLineFixedShiftBenchmarkTest() {
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
        public void TukeyMADFitLineFixedShiftBenchmarkTest() {
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
        public void TukeyAADFitLineFixedShiftBenchmarkTest() {
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
        public void MAEFitCircleFixedShiftBenchmarkTest() {
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
        public void MSEFitCircleFixedShiftBenchmarkTest() {
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
        public void HuberMADFitCircleFixedShiftBenchmarkTest() {
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
        public void HuberAADFitCircleFixedShiftBenchmarkTest() {
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
        public void TukeyMADFitCircleFixedShiftBenchmarkTest() {
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
        public void TukeyAADFitCircleFixedShiftBenchmarkTest() {
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
        public void MAEFitEllipseFixedShiftBenchmarkTest() {
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
        public void MSEFitEllipseFixedShiftBenchmarkTest() {
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
        public void HuberMADFitEllipseFixedShiftBenchmarkTest() {
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
        public void HuberAADFitEllipseFixedShiftBenchmarkTest() {
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
        public void TukeyMADFitEllipseFixedShiftBenchmarkTest() {
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
        public void TukeyAADFitEllipseFixedShiftBenchmarkTest() {
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
