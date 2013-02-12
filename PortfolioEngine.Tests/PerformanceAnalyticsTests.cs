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
    public class PerformanceAnalytics
    {
        [TestInitialize]
        public void StartEngine()
        {
            R Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void AnnualisedReturnTest()
        {
            var res = PortfolioEngine.Analytics.AnnualisedReturn(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100));
            // Check output type
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(double[]));

            // Check results
            double delta = 0.001;

            // Data
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;
            var datam1 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, freq: DataFrequency.Monthly);
            var datad1 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02);
            var returns1 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements).ToArray();

            // Monthly data
            var arm = PortfolioEngine.Analytics.AnnualisedReturn(datam1);
            Assert.AreEqual(Statistics.Mean(returns1) * 12, arm, delta);

            // Daily data
            var ard = PortfolioEngine.Analytics.AnnualisedReturn(datad1);
            Assert.AreEqual(Statistics.Mean(returns1) * 252, ard, delta);
        }

        [TestMethod]
        public void AnnualisedStdDevTest()
        {
            var res = PortfolioEngine.Analytics.AnnualisedStdDev(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100));
            // Check output type
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(double[]));

            double delta = 0.001;

            // Data
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;
            var datam1 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, freq: DataFrequency.Monthly);
            var datad1 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02);
            var returns1 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements).ToArray();

            // Monthly data test
            var sdm = PortfolioEngine.Analytics.AnnualisedStdDev(datam1);
            Assert.AreEqual(Statistics.StandardDeviation(returns1) * Math.Sqrt(12), sdm, delta);

            // Daily data test
            var sdd = PortfolioEngine.Analytics.AnnualisedStdDev(datad1);
            Assert.AreEqual(Statistics.StandardDeviation(returns1) * Math.Sqrt(252), sdd, delta);
        }

        [TestMethod]
        public void ExecutionPerformanceTest()
        {
            int runs = 100;

            var mean = 0.001;
            var stddev = 0.02;
            double[] res = new double[runs];

            var starttime = DateTime.Now;

            for (int c = 0; c < runs; c++)
            {
                res[c] = PortfolioEngine.Analytics.AnnualisedReturn(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100));
            }

            var stoptime = DateTime.Now;
            Console.WriteLine("{0} milliseconds per calculation", (stoptime - starttime).Milliseconds / runs);
        }
    }
}
