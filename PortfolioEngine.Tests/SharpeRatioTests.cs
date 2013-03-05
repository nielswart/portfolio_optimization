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
    public class SharpeRatioTests
    {
        const int elements = 1000;
        const double mu = 0.01;
        const double sigma = 0.02;
        const int seed = 5;

        ITimeSeries<double> monthlySampleTS = TimeSeriesFactory<double>.SampleData.Gaussian(mu, sigma, numperiods: elements, freq: DataFrequency.Monthly);
        ITimeSeries<double> dailySampleTS = TimeSeriesFactory<double>.SampleData.Gaussian(mu, sigma, numperiods: elements, freq: DataFrequency.Daily);

        [TestMethod]
        public void NormalOperation()
        {
            // General case
            double rf = 0.04;
            var res = PortfolioEngine.Analytics.SharpeRatio(monthlySampleTS, rf);
            var shouldbe = (mu*12 - rf) / (Math.Sqrt(12)*sigma);
            // Delta should be the sampling error of the Sharpe Ratio and not that of the mean
            Assert.AreEqual(shouldbe, res, (Math.Sqrt(12) * sigma));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeRiskfreeRate()
        {
            var res = PortfolioEngine.Analytics.SharpeRatio(monthlySampleTS, -0.04);
        }

        [TestMethod]
        public void ZeroStdDev()
        {
            // Create zero std dev time series object
            var ts = TimeSeriesFactory<double>.Create(Enumerable.Repeat(10.0, 100), 1, DataFrequency.Daily);  
            var res = PortfolioEngine.Analytics.SharpeRatio(ts, 0.04);
            Assert.IsTrue(double.IsInfinity(res));
        }
    }
}
