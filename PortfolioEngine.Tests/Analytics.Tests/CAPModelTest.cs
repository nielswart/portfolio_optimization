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
using DataSciLib.Statistics;

namespace PortfolioEngineLib.Tests
{
    [TestClass]
    public class CAPModelTest
    {
        const int elements = 100;
        const double mu = 0.01;
        const double sigma = 0.02;
        const int seed = 5;

        ITimeSeries<double> asset = TimeSeriesFactory<double>.SampleData.Gaussian(mu, sigma, numperiods: elements, freq: DataFrequency.Monthly);
        ITimeSeries<double> benchmark = TimeSeriesFactory<double>.SampleData.Gaussian(0.015, 0.015, numperiods: elements, freq: DataFrequency.Monthly);

        R Engine;

        [TestInitialize]
        public void StartEngine()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void SimpleRegression()
        {
            var riskfree = 0.04;
            var capm = PortfolioAnalytics.CAPModel(asset, benchmark, riskfree);

            var ralpha = CAPM.Alpha(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree).First();
            Assert.AreEqual(ralpha, capm.Alpha, 0.001);

            var rbeta = CAPM.Beta(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree).First();
            Assert.AreEqual(rbeta, capm.Beta, 0.001);
        }
    }
}
