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
    public class VaRTest
    {
        const int elements = 1000;
        const double mu_a = 0.001;
        const double sigma_a = 0.01;
        const double mu_b = 0.0015;
        const double sigma_b = 0.015;

        ITimeSeries<double> benchmark = TimeSeriesFactory<double>.SampleData.Gaussian(mu_b, sigma_b, numperiods: elements, freq: DataFrequency.Daily);

        R Engine;

        [TestInitialize]
        public void StartEngine()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void ItRuns()
        {
            ITimeSeries<double> asset = TimeSeriesFactory<double>.SampleData.Gaussian(mu_a, sigma_a, numperiods: elements, freq: DataFrequency.Daily);
            var res = PortfolioAnalytics.VaR(asset);

            Console.WriteLine(res);
        }
    }
}
