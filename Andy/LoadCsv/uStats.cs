using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Net;
using sMath = System.Math;

namespace LoadCsv
{
    public static class uStats
    {
        //-------------------------------------------------------------------------------
        public static double Mean(this IEnumerable<byte> values)
        {
            if (null == values || values.Count() <= 0) return 0;
            var converted = values.Select(Convert.ToDouble).ToArray();
            return Mean(converted);
        }
        public static double Mean(this IEnumerable<int> values)
        {
            if (null == values || values.Count() <= 0) return 0;
            var converted = values.Select(Convert.ToDouble).ToArray();
            return Mean(converted);
        }
        public static double Mean(this IEnumerable<double> values)
        {
            if (null == values || values.Count() <= 0) return 0;
            double s = 0;
            double n = values.Count();
            for (int i = 0; i < values.Count(); i++)
                s += values.ElementAt(i);
            return s / n;
        }

        //-------------------------------------------------------------------------------
        public static double Variance(this IEnumerable<byte> values, bool isPopulation = true)
        {
            if (null == values || values.Count() <= 0) return 0;
            var converted = values.Select(Convert.ToDouble).ToArray();
            return Variance(converted, isPopulation);
        }
        public static double Variance(this IEnumerable<int> values, bool isPopulation = true)
        {
            if (null == values || values.Count() <= 0) return 0;
            var converted = values.Select(Convert.ToDouble).ToArray();
            return Variance(converted, isPopulation);
        }
        public static double Variance(this IEnumerable<double> values, bool isPopulation = true)
        {
            return values.Variance(values.Mean(), isPopulation);
        }
        public static double Variance(this IEnumerable<double> values, double mean, bool isPopulation = true)
        {
            if (null == values || values.Count() <= 1) return 0;
            double variance = 0;
            int n = values.Count();
            for (int i = 0; i < n; i++)
                variance += sMath.Pow((values.ElementAt(i) - mean), 2);
            if (isPopulation)
                return variance / n;
            return variance / (n-1);
        }

        //-------------------------------------------------------------------------------
        public static double Std(this IEnumerable<byte> values, bool isPopulation = true)
        {
            if (null == values || values.Count() <= 1) return 0;
            var converted = values.Select(Convert.ToDouble).ToArray();
            return Std(converted, isPopulation);
        }
        public static double Std(this IEnumerable<int> values, bool isPopulation = true)
        {
            if (null == values || values.Count() <= 1) return 0;
            var converted = values.Select(Convert.ToDouble).ToArray();
            return Std(converted, isPopulation);
        }
        public static double Std(this IEnumerable<double> values, bool isPopulation = true)
        {
            if (null == values || values.Count() <= 1) return 0;
            double mean     = values.Mean();
            double variance = values.Variance(mean, isPopulation);
            return sMath.Sqrt(variance);
        }

        //-------------------------------------------------------------------------------
        public static double Covariance(this IEnumerable<byte> xs, IEnumerable<byte> ys, bool isPopulation = true)
        {
            if (null == xs || xs.Count() <= 0) return double.NaN;
            if (null == ys || ys.Count() <= 0) return double.NaN;
            var convertedx = xs.Select(Convert.ToDouble).ToArray();
            var convertedy = ys.Select(Convert.ToDouble).ToArray();
            return Covariance(xs, ys, isPopulation);
        }
        public static double Covariance(this IEnumerable<int> xs, IEnumerable<int> ys, bool isPopulation = true)
        {
            if (null == xs || xs.Count() <= 0) return double.NaN;
            if (null == ys || ys.Count() <= 0) return double.NaN;
            var convertedx = xs.Select(Convert.ToDouble).ToArray();
            var convertedy = ys.Select(Convert.ToDouble).ToArray();
            return Covariance(xs, ys, isPopulation);
        }
        /// <summary>
        /// cov(X, Y) = 1/n * \sum{i=1~n} (xi-E(X)) * (yi-E(Y))
        /// </summary>
        public static double Covariance(IEnumerable<double> x, IEnumerable<double> y, bool isPopulation = true)
        {
            if (null == x || x.Count() <= 0) return double.NaN;
            if (null == y || y.Count() <= 0) return double.NaN;
            double n = x.Count();
            if (n != y.Count()) return double.NaN;

            double mx = Mean(x);
            double my = Mean(y);
            double sumSum = 0;
            for (int i = 0; i < x.Count(); i++)
                sumSum += (x.ElementAt(i) - mx) * (y.ElementAt(i) - my);

            double divBy = isPopulation? x.Count() : x.Count()-1;
            double cov = sumSum / divBy;
            return cov;
        }

        //-------------------------------------------------------------------------------
        public static double CorrCoeff(this IEnumerable<byte> xs, IEnumerable<byte> ys, bool isPopulation = true)
        {
            if (null == xs || xs.Count() <= 0) return double.NaN;
            if (null == ys || ys.Count() <= 0) return double.NaN;
            var convertedx = xs.Select(Convert.ToDouble).ToArray();
            var convertedy = ys.Select(Convert.ToDouble).ToArray();
            return CorrCoeff(xs, ys, isPopulation);
        }
        public static double CorrCoeff(this IEnumerable<int> xs, IEnumerable<int> ys, bool isPopulation = true)
        {
            if (null == xs || xs.Count() <= 0) return double.NaN;
            if (null == ys || ys.Count() <= 0) return double.NaN;
            var convertedx = xs.Select(Convert.ToDouble).ToArray();
            var convertedy = ys.Select(Convert.ToDouble).ToArray();
            return CorrCoeff(xs, ys, isPopulation);
        }
        /// <summary>
        /// Returns the correlation coefficient
        /// Population: rho{X,Y} = cov(X,Y) / (std{X} * std{Y})
        /// Sample    : rho{X,Y} = {\sum{i=1~n}{xi*yi} - n*mx*my} / { (n-1) stdSam{x} stdSam{y} }
        /// </summary>
        public static double CorrCoeff(IEnumerable<double> x, IEnumerable<double> y, bool isPopulation = true)
        {
            if (null == x || x.Count() <= 0) return double.NaN;
            if (null == y || y.Count() <= 0) return double.NaN;
            double n = x.Count();
            if (n != y.Count())      return double.NaN;

            double rho = 0;

            // Population
            if (isPopulation)
            {
                double stdx = Std(x, true);
                double stdy = Std(y, true);
                double cov = Covariance(x, y, true);
                rho = cov / (stdx * stdy);
            }
            else
            {
                double mx = Mean(x);
                double my = Mean(y);
                double nmxmy = n * mx * my;
                double stdx = Std(x, false);
                double stdy = Std(y, false);
                double sum = 0;
                for (int i = 0; i < x.Count(); i++)
                    sum += x.ElementAt(i) * y.ElementAt(i);
                rho = (sum - nmxmy) / ((n-1)*stdx* stdy);
            }
            return rho;
        }
        #region Non Standard calculations

        //-------------------------------------------------------------------------------
        /// <summary>
        /// If percentile = 0.05 (5%) and the list contains 60 values, this will return the averaged value of the 3 elements with the highest values.
        /// Percentile must be between 0 and 1.
        /// </summary>
        public static double GetAverageOfTopOrBottomPercentile(IEnumerable<byte> vals, double percentile = 0.05, bool useTopOfHighest = true)
        {
            var list = new double[vals.Count()];
            return GetAverageOfTopOrBottomPercentile(list, percentile, useTopOfHighest);
        }
        public static double GetAverageOfTopOrBottomPercentile(IEnumerable<double> vals, double percentile = 0.05, bool useTopOfHighest = true)
        {
            if (percentile < 0 || 1 < percentile) return double.NaN;
            if (uNet.IsNullOrEmpty(vals))         return double.NaN;
            if (vals.Count() == 1)                return vals.First();

            var sorted = new List<double>(vals);

            // Choose between top of highest values, or bottom of lowest values
            if (useTopOfHighest) sorted = sorted.OrderByDescending(v => v).ToList();
            else                 sorted.Sort();

            int nbOfValsToInclude = (vals.Count() * percentile).Int();
            if (nbOfValsToInclude <= 0) return 0;
            
            double sum = 0;
            for (int i = 0; i < nbOfValsToInclude; i++)
                sum = sorted[i];

            return sum / nbOfValsToInclude;
        }
        #endregion

    }
}
