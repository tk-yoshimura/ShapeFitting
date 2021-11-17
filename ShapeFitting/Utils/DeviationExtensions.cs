using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeFitting {
    internal static class DeviationExtensions {
        public static double Median(this IEnumerable<double> vs) {
            double[] vs_arr = vs.ToArray();

            if (vs_arr.Length <= 0) {
                return double.NaN;
            }
            if (vs_arr.Length <= 1) {
                return vs_arr[0];
            }
            if (vs_arr.Length <= 2) {
                return (vs_arr[0] + vs_arr[1]) / 2;
            }

            Array.Sort(vs_arr);

            double median = ((vs_arr.Length & 1) == 1)
                ? vs_arr[vs_arr.Length / 2]
                : (vs_arr[vs_arr.Length / 2 - 1] + vs_arr[vs_arr.Length / 2]) / 2;

            return median;
        }

        /// <summary> MAD = median(|x - median(x)|) </summary>
        public static double MedianAbsoluteDeviation(this IEnumerable<double> vs) {
            double median = vs.Median();
            double mad = vs.Select((v) => Math.Abs(v - median)).Median();

            return mad;
        }

        /// <summary> AAD = mean(|x - mean(x)|) </summary>
        public static double AverageAbsoluteDeviation(this IEnumerable<double> vs) {
            double mean = vs.Average();
            double aad = vs.Select((v) => Math.Abs(v - mean)).Average();

            return aad;
        }
    }
}
