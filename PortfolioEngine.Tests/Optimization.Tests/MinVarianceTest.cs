using DataSciLib.DataStructures;
using MathNet.Numerics.Distributions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioEngine.Portfolios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioEngine.Tests
{
    [TestClass]
    public class MinVarianceTests
    {
        PortfolioOptimizer optimizer;
        Dictionary<string, double> mean;
        CovarianceMatrix cov;

        [TestInitialize]
        public void StartEngine()
        {
            // Load data
            cov = CovarianceMatrix.Create("D:/repos/windows/portfolio_optimization/datasetcov.csv");

            var norm = new Normal(0.008, 0.02);
            norm.RandomSource = new Random(11);
            var data = norm.Samples();

            var iter = data.GetEnumerator();
            var meankv = from k in cov.Keys
                         let d = iter.MoveNext()
                         select new KeyValuePair<string, double>(k, Math.Round(iter.Current, 4));

            mean = meankv.ToDictionary(a => a.Key, b => b.Value);
        }

        [TestMethod]
        public void ItRuns()
        {
            // Create new portfolio
            var portf = new Portfolio("TestPortfolio");

            // Create instruments from data
            var instruments = from k in cov.Keys
                              select new Instrument(k, mean[k], cov[k]);

            portf.AddRange(instruments);

            // Add portfolio constraints
            portf.AddAllInvestedConstraint();
            portf.AddLongOnlyConstraint();

            //
            double rf = 0.05;
            var res = PortfolioOptimizer.CalcMinimumVariance(portf, rf);
            var rr =  new { res.StdDev, res.Mean };

            Console.WriteLine("Risk {0}, Return {1} ", rr.StdDev, rr.Mean);
        }
    }
}
