﻿using DataSciLib.DataStructures;
using DataSciLib.REngine;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using DataSciLib.REngine.PerformanceAnalytics;
using DataSciLib.REngine.Rmetrics;
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
    public class ResidualRiskTest
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
            var res = Analytics.RealisedAlpha(asset, benchmark, -0.04);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LargeRiskfreeRate()
        {
            var res = Analytics.RealisedAlpha(asset, benchmark, 1.05);
        }

        [TestMethod]
        public void ResultDailyData()
        {
            var riskfree = 0.05;
            var res = Analytics.RealisedAlpha(asset, benchmark, riskfree);
            var ralpha = CAPM.Alpha(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree).First();
            Assert.AreEqual(ralpha, res, 0.001);
        }
    }
}