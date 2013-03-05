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
        public void NormalOperation()
        {
            var res = PortfolioEngine.Analytics.AnnualisedReturn(monthlySampleTS);

            // Check output
            Assert.IsNotNull(res);

            // Check results
            double delta = 0.001;

            // Random Sample settings
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;

            // Monthly Data
            var arm = PortfolioEngine.Analytics.AnnualisedReturn(monthlySampleTS);
            var returns1 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements);
            Assert.AreEqual(Statistics.Mean(returns1) * 12, arm, delta);

            // Daily data
            var ard = PortfolioEngine.Analytics.AnnualisedReturn(dailySampleTS);
            var returns2 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements);
            Assert.AreEqual(Statistics.Mean(returns2) * 252, ard, delta);
        }
    }
}
