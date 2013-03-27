using DataSciLib.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioEngine;
using System;

namespace PortfolioEngineLib.Tests
{
    [TestClass]
    public class ResidualRiskTest
    {
        const int elements = 1000;
        const double mu_a = 0.001;
        const double sigma_a = 0.01;
        const double mu_b = 0.0015;
        const double sigma_b = 0.015;
        
        ITimeSeries<double> benchmark = TimeSeriesFactory<double>.SampleData.Gaussian(mu_b, sigma_b, numperiods: elements, freq: DataFrequency.Daily);

        [TestMethod]
        public void ItRuns()
        {
            TimeSeriesFactory<double>.SampleData.RandomSeed = 11;
            ITimeSeries<double> asset = TimeSeriesFactory<double>.SampleData.Gaussian(mu_a, sigma_a, numperiods: elements, freq: DataFrequency.Daily);
            var riskfree = 0.05;
            var res = PortfolioAnalytics.ResidualRisk(asset, benchmark, riskfree);

            Console.WriteLine(res);
        }
    }
}
