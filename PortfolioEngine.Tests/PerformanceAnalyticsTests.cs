using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioEngine;
using PortfolioEngine.Portfolios;
using DataSciLib.REngine;
using DataSciLib.DataStructures;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;

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
        }

        [TestMethod]
        public void AnnualisedStdDevTest()
        {
            var res = PortfolioEngine.Analytics.AnnualisedStdDev(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100));
            // Check output type
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(double[]));
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

        [TestMethod]
        public void ResultRangeTest()
        {
            int runs = 100;

            var mean = 0.001;
            var stddev = 0.02;
            double[] res = new double[runs];

            for (int c = 0; c < runs; c++)
            {
                res[c] = PortfolioEngine.Analytics.AnnualisedReturn(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100));
            }

            Console.WriteLine("Mean and std dev of standard deviations are {0}, {1}", Statistics.Mean(res), Statistics.StandardDeviation(res));
        }

        [TestMethod]
        public void CheckResultExact()
        {
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
    }
}
