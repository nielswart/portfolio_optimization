using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.PerformanceAnalytics;
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
    public class ActivePremiumTest
    {
        const int elements = 1000;
        const double mu_a = 0.001;
        const double sigma_a = 0.002;
        const double mu_b = 0.0015;
        const double sigma_b = 0.0014;

        ITimeSeries<double> benchmark = TimeSeriesFactory<double>.SampleData.Gaussian(mu_b, sigma_b, numperiods: elements, freq: DataFrequency.Daily);
                
        /// <summary>
        /// The return on an investment's annualised return minus the benchmark's annualised return
        /// </summary>
        [TestMethod]
        public void DailyDataResult()
        {
            ITimeSeries<double> asset = TimeSeriesFactory<double>.SampleData.Gaussian(mu_a, sigma_a, numperiods: elements, freq: DataFrequency.Daily);

            var res = Analytics.ActivePremium(asset, benchmark);
            var rap = (mu_a - mu_b) * 252;
            Assert.AreEqual(rap, res, sigma_a * Math.Sqrt(252));
        }

        [TestMethod]
        public void Uncorrelated()
        {
            TimeSeriesFactory<double>.SampleData.RandomSeed = 8;
            ITimeSeries<double> asset = TimeSeriesFactory<double>.SampleData.Gaussian(mu_a, sigma_a, numperiods: elements, freq: DataFrequency.Daily);

            var res = Analytics.ActivePremium(asset, benchmark);
            var rap = (mu_a - mu_b) * 252;
            Assert.AreEqual(rap, res, sigma_a*Math.Sqrt(252));
        }

    }
}
