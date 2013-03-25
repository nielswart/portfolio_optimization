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

namespace PortfolioEngineLib.Tests
{
    [TestClass]
    public class TrackingErrorTest
    {
        const int elements = 1000;
        const double mu_a = 0.001;
        const double sigma_a = 0.01;
        const double mu_b = 0.0015;
        const double sigma_b = 0.0014;

        ITimeSeries<double> benchmark = TimeSeriesFactory<double>.SampleData.Gaussian(mu_b, sigma_b, numperiods: elements, freq: DataFrequency.Daily);

        /// <summary>
        /// Test the return value
        /// </summary>
        [TestMethod]
        public void Result()
        {
            TimeSeriesFactory<double>.SampleData.RandomSeed = 8;

            ITimeSeries<double> asset = TimeSeriesFactory<double>.SampleData.Gaussian(mu_a, sigma_a, numperiods: elements, freq: DataFrequency.Daily);
            var res = PortfolioAnalytics.TrackingError(asset, benchmark);
            var te = (asset.AsTimeSeries() - benchmark.AsTimeSeries()).AnnualisedStdDev();
            Assert.AreEqual(te, res, 0.001);
        }
    }
}
