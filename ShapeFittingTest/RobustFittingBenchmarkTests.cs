using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeFitting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFittingTest {
    [TestClass]
    public class RobustFittingBenchmarkTests {
        private Vector[] line_points, circle_points, ellipse_points;

        public RobustFittingBenchmarkTests() {
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

        public IEnumerable<Vector> RandomPoints(IEnumerable<Vector> points, double prob, double range, Random random) {
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

        [TestMethod]
        public void MAEFitLineBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = MAEFitting.FitLine(RandomPoints(line_points, prob, range, random));

                        (double theta, double phi) = line;

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
        public void HuberMADFitLineBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomPoints(line_points, prob, range, random), new HuberMAD());

                        (double theta, double phi) = line;

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
        public void HuberAADFitLineBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomPoints(line_points, prob, range, random), new HuberAAD());

                        (double theta, double phi) = line;

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
        public void TukeyMADFitLineBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomPoints(line_points, prob, range, random), new TukeyMAD());

                        (double theta, double phi) = line;

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
        public void TukeyAADFitLineBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Line line = RobustFitting.FitLine(RandomPoints(line_points, prob, range, random), new TukeyAAD());

                        (double theta, double phi) = line;

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
        public void MAEFitCircleBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = MAEFitting.FitCircle(RandomPoints(circle_points, prob, range, random));

                        (double cx, double cy, double r) = circle;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void HuberMADFitCircleBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomPoints(circle_points, prob, range, random), new HuberMAD());

                        (double cx, double cy, double r) = circle;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void HuberAADFitCircleBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomPoints(circle_points, prob, range, random), new HuberAAD());

                        (double cx, double cy, double r) = circle;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void TukeyMADFitCircleBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomPoints(circle_points, prob, range, random), new TukeyMAD());

                        (double cx, double cy, double r) = circle;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void TukeyAADFitCircleBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Circle circle = RobustFitting.FitCircle(RandomPoints(circle_points, prob, range, random), new TukeyAAD());

                        (double cx, double cy, double r) = circle;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 || Math.Abs(r - 4) > 0.4;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} circle={circle} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void MAEFitEllipseBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = MAEFitting.FitEllipse(RandomPoints(ellipse_points, prob, range, random));

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void HuberMADFitEllipseBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomPoints(ellipse_points, prob, range, random), new HuberMAD());

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void HuberAADFitEllipseBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomPoints(ellipse_points, prob, range, random), new HuberAAD());

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void TukeyMADFitEllipseBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomPoints(ellipse_points, prob, range, random), new TukeyMAD());

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }

        [TestMethod]
        public void TukeyAADFitEllipseBenchmarkTest() {
            const int tests = 4;

            Random random = new(1234);

            int mistakes = 0;

            foreach (double prob in new double[] { 0.1, 0.2, 0.4, 0.6, 0.8, 0.9, 1.0 }) {
                foreach (double range in new double[] { 0.1, 0.2, 0.4, 0.8, 1.6, 3.2, 6.4 }) {

                    for (int i = 0; i < tests; i++) {
                        Ellipse ellipse = RobustFitting.FitEllipse(RandomPoints(ellipse_points, prob, range, random), new TukeyAAD());

                        (double cx, double cy, double rx, double ry, double angle) = ellipse;

                        bool miss = Math.Abs(cx - 2) > 0.2 || Math.Abs(cy - 3) > 0.3 ||
                                    Math.Abs(rx - 4) > 0.4 || Math.Abs(ry - 1) > 0.1 || Math.Abs(angle - 0.5) > 0.05;

                        if (miss) {
                            mistakes++;
                        }

                        Console.WriteLine($"prob={prob} range={range} ellipse={ellipse} {(miss ? "NG" : "OK")}");
                    }
                }
            }

            Console.WriteLine($"mistakes = {mistakes}");
        }
    }
}
