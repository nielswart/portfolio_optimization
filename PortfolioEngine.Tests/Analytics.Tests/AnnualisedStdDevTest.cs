using DataSciLib.DataStructures;
using DataSciLib.REngine;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioEngine;
using PortfolioEngine.Portfolios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioEngine.Tests
{
    [TestClass]
    public class AnnualisedStdDevTest
    {
        const int elements = 100;
        const double mu = 0.01;
        const double sigma = 0.02;
        const int seed = 5;

        ITimeSeries<double> monthlySampleTS = TimeSeriesFactory<double>.SampleData.Gaussian(mu, sigma, numperiods: elements, freq: DataFrequency.Monthly);
        ITimeSeries<double> dailySampleTS = TimeSeriesFactory<double>.SampleData.Gaussian(mu, sigma, numperiods: elements, freq: DataFrequency.Daily);

        [TestMethod]
        public void ResultDailyData()
        {
            // Data
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = seed;

            // Monthly data test
            var sdm = PortfolioAnalytics.AnnualisedStdDev(monthlySampleTS);
            Assert.IsNotNull(sdm);
            Assert.AreEqual(sigma * Math.Sqrt(12), sdm, sigma);
        }

        [TestMethod]
        public void ResultMonthlyData()
        {
            // Data
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;

            // Monthly data test
            var returns1 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements);
            var sdm = PortfolioAnalytics.AnnualisedStdDev(monthlySampleTS);
            Assert.IsNotNull(sdm);
            Assert.AreEqual(Statistics.StandardDeviation(returns1) * Math.Sqrt(12), sdm, 0.001);
        }

        [TestMethod]
        public void ZeroChangeData()
        {
            var ts = TimeSeriesFactory<double>.Create(Enumerable.Repeat(10.0, 100), 1, DataFrequency.Daily);
            var sd = PortfolioAnalytics.AnnualisedStdDev(ts);

            Assert.AreEqual(0, sd);
        }

        [TestMethod]
        public void IntegratedData()
        {

        }
    }
}
