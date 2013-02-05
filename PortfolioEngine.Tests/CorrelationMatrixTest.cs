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
    public class CorrelationMatrixTest
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
            var res = PortfolioEngine.Analytics.CorrelationMatrix(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 10));
            // Check output type
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(double[,]));

            // Should throw ArgumentException if only single return series in TimeSeries
            /*
            Assert.Throws(typeof(ArgumentException), () =>
            {
                PortfolioEngine.Analytics.CorrelationMatrix(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1));
            });
             * */
        }

        [TestMethod]
        public void SimplePerformanceTest()
        {
            int runs = 100;

            var mean = 0.001;
            var stddev = 0.02;
            List<double[,]> res = new List<double[,]>();
            var starttime = DateTime.Now;

            for (int c = 0; c < runs; c++)
            {
                res.Add(PortfolioEngine.Analytics.CorrelationMatrix(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100, 10)));
            }

            var stoptime = DateTime.Now;
            Console.WriteLine("{0} milliseconds per calculation", (stoptime - starttime).Milliseconds / runs);
        }

        [TestMethod]
        public void CheckResultExact()
        {
            double delta = 0.001;

            // Data
            int elements = 100;
            TimeSeriesFactory<double>.SampleData.RandomSeed = 5;
            var datam1 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, numseries: 10, freq: DataFrequency.Monthly);
            var datad1 = TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, numseries: 10, freq: DataFrequency.Daily);
            var returns1 = Normal.Samples(new Random(5), 0.01, 0.02).Take(elements).ToArray();

            var cmm = PortfolioEngine.Analytics.CorrelationMatrix(datam1);
            Console.WriteLine("Monthly Data Correlation:");
            Console.WriteLine(cmm.Print());
            //Assert.AreEqual(-0.0077346, srm.First(), delta);
        }
    }
}
