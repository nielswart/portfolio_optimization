using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.PerformanceAnalytics;
using DataSciLib.REngine.Rmetrics;
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
    public class RealisedBetaTest
    {
        const int elements = 1000;
        const double mu = 0.01;
        const double sigma = 0.02;
        const int seed = 5;

        ITimeSeries<double> asset = TimeSeriesFactory<double>.SampleData.Gaussian(mu, sigma, numperiods: elements, freq: DataFrequency.Daily);
        ITimeSeries<double> benchmark = TimeSeriesFactory<double>.SampleData.Gaussian(mu, sigma, numperiods: elements, freq: DataFrequency.Daily);

        R Engine;

        [TestInitialize]
        public void StartEngine()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeRiskfreeRate()
        {
            // Should throw exception for a risk-free rate smaller than 0
            var res = Analytics.RealisedBeta(asset, benchmark, -0.04);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LargeRiskfreeRate()
        {
            // Should throw exception for a risk-free rate larger than 1
            var res = Analytics.RealisedBeta(asset, benchmark, 1.05);
        }

        [TestMethod]
        public void ResultDailyData()
        {
            var riskfree = 0.05;
            var res = Analytics.RealisedBeta(asset, benchmark, riskfree);
            var rbeta = CAPM.Beta(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree).First();
            Assert.AreEqual(rbeta, res, 0.001);
        }


    }
}
