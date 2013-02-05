using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace REngine.Tests.Rmetrics
{
    [TestClass]
    public class fPortfolioTests
    {
        [TestInitialize]
        public void Start()
        {
            R Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void CalcFeasiblePortfolio()
        {
            
        }

        [TestMethod]
        public void CalcEfficientPortfolio()
        {

        }

        [TestMethod]
        public void CalcPortfolioFrontier()
        {

        }

        [TestMethod]
        public void CreateFromTimeSeriesandWeightsTest()
        {
            var weight = 1.0 / 10;
            double[] weights = new double[10];
            for (int i = 0; i < 10; i++)
            {
                weights[i] = weight;
            }
            var mean = 0.001;
            var stddev = 0.02;

            var assets = TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100, 10);
            /*
            Assert.DoesNotThrow(() =>
            {
                fPortfolio.Create(assets, weights);
            });
             * */
        }

        [TestMethod]
        public void CreateMultipleTimes()
        {
            int runs = 100;

            var weight = 1.0 / 10;
            double[] weights = new double[10];
            for (int i = 0; i < 10; i++)
            {
                weights[i] = weight;
            }
            var mean = 0.001;
            var stddev = 0.02;

            var assets = TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, 100, 10);
            for (int c = 0; c < runs; c++)
            {
                fPortfolio.Create(assets, weights);
                Console.WriteLine(c);
            }
        }
    }
}
