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

namespace PortfolioEngine.Tests
{
    [TestClass]
    public class PerformanceAnalytics
    {
        [TestMethod]
        public void ExecutionPerformanceTest()
        {
            var runs = 100;

            var mean = 0.001;
            var stddev = 0.02;
            var res = new double[runs];

            var starttime = DateTime.Now;

            for (int c = 0; c < runs; c++)
            {
                res[c] = PortfolioEngine.Analytics.AnnualisedReturn(TimeSeriesFactory<double>.SampleData.Gaussian(mean, stddev, 100));
            }

            var stoptime = DateTime.Now;
            Console.WriteLine("{0} milliseconds per calculation", (stoptime - starttime).Milliseconds / runs);
        }
    }
}
