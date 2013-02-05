using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortfolioEngine;
using PortfolioEngine.Portfolios;
using DataSciLib.REngine;
using DataSciLib.DataStructures;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PortfolioEngineLib.Tests
{
    [TestClass]
    public class RealizedBetaTests
    {
        [TestInitialize]
        public void StartEngine()
        {
            R Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void CheckParametersTest()
        {
            var res = PortfolioEngine.Analytics.RealisedBeta(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1), TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1));
            // Check output type
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(double[]));

            // Risk-free rate larger than 1
            /*
            Assert.Throws(typeof(ArgumentException), () =>
            {
                PortfolioEngine.Analytics.RealisedBeta(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.015, 100, 2),
                   TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1), 1.15);
            });

            // Should throw ArgumentException if both TimeSeries' not of the same length
            Assert.Throws(typeof(ArgumentException), () =>
            {
                PortfolioEngine.Analytics.RealisedBeta(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1),
                    TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 101, 1));
            });

            // Should throw ArgumentException if the Benchmark return TimeSeries is not a single series.
            Assert.Throws(typeof(ArgumentException), () =>
            {
                PortfolioEngine.Analytics.RealisedBeta(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 5),
                    TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 2));
            });
             * */
        }

        [TestMethod]
        public void SimplePerformanceTest()
        {
            int runs = 100;

            var mean = 0.001;
            var stddev = 0.02;
            double[] res = new double[runs];
            var starttime = DateTime.Now;

            for (int c = 0; c < runs; c++)
            {
                res[c] = PortfolioEngine.Analytics.RealisedBeta(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100, 1),
                    TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1)).First();
            }

            var stoptime = DateTime.Now;
            Console.WriteLine("{0} milliseconds per calculation", (stoptime - starttime).Milliseconds / runs);
        }

        [TestMethod]
        public void MultivariateTest()
        {
            var mean = 0.001;
            var stddev = 0.02;

            var res = PortfolioEngine.Analytics.RealisedBeta(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100, 10),
                TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1));

            Console.WriteLine(res.Print());
        }

        [TestMethod]
        public void CheckResultExact()
        {
            double delta = 0.001;

            // Data
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;
            var datam1 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, freq: DataFrequency.Monthly);
            var returns1 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements).ToArray();

            TimeSeriesFactory<double>.SampleData.RandomSeed = 7;
            var datam2 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, freq: DataFrequency.Monthly);
            var returns2 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements).ToArray();

            var srm = PortfolioEngine.Analytics.RealisedBeta(datam1, datam2);
            Console.WriteLine("Monthly Data Alpha: {0}", srm.First());
            Assert.AreEqual(0.04869, srm.First(), delta);
        }
    }
}
