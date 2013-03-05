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
        public void NormalOperation()
        {
            var res = PortfolioEngine.Analytics.AnnualisedStdDev(TimeSeriesFactory<double>.SampleData.Gaussian(0, 0.2, 100));
            // Check output type
            Assert.IsNotNull(res);

            double delta = 0.001;

            // Data
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;

            // Monthly data test
            var returns1 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements);
            var sdm = PortfolioEngine.Analytics.AnnualisedStdDev(monthlySampleTS);
            Assert.AreEqual(Statistics.StandardDeviation(returns1) * Math.Sqrt(12), sdm, delta);

            // Daily data test
            var sdd = PortfolioEngine.Analytics.AnnualisedStdDev(dailySampleTS);
            var returns2 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements);
            Assert.AreEqual(Statistics.StandardDeviation(returns2) * Math.Sqrt(252), sdd, delta);
        }

    }
}
