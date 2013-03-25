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
    public class AnnualisedReturnTest
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
            // Random Sample settings
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;

            // Daily data
            var ard = PortfolioEngine.PortfolioAnalytics.AnnualisedReturn(dailySampleTS);
            Assert.IsNotNull(ard);
            var returns = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements);
            Assert.AreEqual(Statistics.Mean(returns) * 252, ard, 0.001);
        }

        [TestMethod]
        public void ResultMonthlyData()
        {
            // Random Sample settings
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;

            // Monthly Data
            var arm = PortfolioEngine.PortfolioAnalytics.AnnualisedReturn(monthlySampleTS);
            Assert.IsNotNull(arm);
            var returns = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements);
            Assert.AreEqual(Statistics.Mean(returns) * 12, arm, 0.001);
        }
    }
}
