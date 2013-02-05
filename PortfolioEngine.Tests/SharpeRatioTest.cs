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
    public class SharpeRatioTests
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
            var res = PortfolioEngine.Analytics.SharpeRatio(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0, 0.2, 100, 1));
            // Check output type
            Assert.IsNotNull(res);
            //Assert.IsInstanceOf(typeof(double[]), res);

            // Risk-free rate larger than 1
            /*
            Assert.Throws(typeof(ArgumentException), () => 
            { 
                PortfolioEngine.Analytics.SharpeRatio(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.015, 100, 2), 1.2); 
            });
            */
        }

        [TestMethod]
        public void SimplePerformanceTest()
        {
            int runs = 100;

            var mean = 0.001;
            var stddev = 0.02;
            var rf = 0.05 / 252;

            double[] res = new double[runs];

            var starttime = DateTime.Now;

            for (int c = 0; c < runs; c++)
            {
                TimeSeriesFactory<double>.SampleData.RandomSeed = c;
                res[c] = PortfolioEngine.Analytics.SharpeRatio(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100, 1), rf).First();
                Console.WriteLine("SR: {0}", res[c]);
            }
            var meanoutp = (mean * 252 - rf) / (stddev * Math.Sqrt(252));
            Console.WriteLine("Expected SharpeRatio: {0}", meanoutp);
            Console.WriteLine("The mean Sharpe Ratio for all calculationss are: {0}", Statistics.Mean(res));

            var stoptime = DateTime.Now;
            Console.WriteLine("{0} milliseconds per calculation", (stoptime - starttime).Milliseconds / runs);
        }

        [TestMethod]
        public void MultivariateTest()
        {
            var mean = 0.001;
            var stddev = 0.02;
            var rf = 0.05 / 252;

            var res = PortfolioEngine.Analytics.SharpeRatio(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100, 10), rf);
            
            Console.WriteLine(res.Print());
        }

        [TestMethod]
        public void CheckResultExact()
        {
            // Monthly data
            double[] returns = new[] { 0, 0.02, -0.03, 0.02, -0.015, 0, 0.02, 0.03, 0.02, -0.015, 0, -0.02, 0.03, 0.02, -0.015 };
            var datam = TimeSeriesFactory<double>.Create(returns, DataFrequency.Monthly);
            var sharpem = PortfolioEngine.Analytics.SharpeRatio(datam, 0);

            double delta = 0.001;
            Assert.AreEqual((Statistics.Mean(returns) * 12) / (Statistics.StandardDeviation(returns) * Math.Sqrt(12)), sharpem.First(), delta);

            // Daily data
            var datad = TimeSeriesFactory<double>.Create(returns, freq: DataFrequency.Daily);
            var sharped = PortfolioEngine.Analytics.SharpeRatio(datad, 0);

            Assert.AreEqual((Statistics.Mean(returns) * 252) / (Statistics.StandardDeviation(returns) * Math.Sqrt(252)), sharped.First(), delta);
        }
    }
}
