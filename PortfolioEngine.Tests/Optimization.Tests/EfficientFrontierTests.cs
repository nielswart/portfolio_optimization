using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioEngine;
using PortfolioEngine.Portfolios;
using DataSciLib.REngine;
using DataSciLib.DataStructures;
using PortfolioEngine.Settings;
using DataSciLib;
using MathNet.Numerics.Distributions;
using PerformanceTools;
using DataSciLib.IO;
using LINQtoCSV;

namespace PortfolioEngine.Tests
{
    [TestClass]
    public class EfficientFrontierTests
    {
        Dictionary<string, double> mean;
        CovarianceMatrix cov;

        [TestInitialize]
        public void InitializeData()
        {
            // Load data
            cov = CovarianceMatrix.Create("D:/repos/windows/portfolio_optimization/datasetcov.csv");

            var norm = new Normal(0.0175, 0.02);
            norm.RandomSource = new Random(125);
            var data = norm.Samples();

            var iter = data.GetEnumerator();
            var meankv = from k in cov.Keys
                         let d = iter.MoveNext()
                         select new KeyValuePair<string, double>(k, Math.Round(iter.Current,4));

            mean = meankv.ToDictionary(a => a.Key, b => b.Value);

            //PortfolioOptimizer.Initialize();
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
            var res = PortfolioOptimizer.CalcEfficientFrontier(portf, rf, 50);
            var riskreturn = from p in res
                             select new { p.StdDev, p.Mean };

        
            foreach (var rr in riskreturn)
            {
                Console.WriteLine("Risk {0}, Return {1} ", rr.StdDev, rr.Mean);
            }
        }
        
        [TestMethod]
        public void EfficientFrontierPerformanceTest()
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
            double rf = 0.05;

            int runs = 100;
            for (int c = 0; c < runs; c++)
            {
                PerformanceLogger.Start("EfficientFrontierTests", "EfficientFrontierPerformanceTest", "Optimization.CalcEfficientFrontier");
                var res = PortfolioOptimizer.CalcEfficientFrontier(portf, rf, 50);
                Console.WriteLine(c);
                PerformanceLogger.Stop("EfficientFrontierTests", "EfficientFrontierPerformanceTest", "Optimization.CalcEfficientFrontier");
            }
            PerformanceLogger.WriteToCSV("performancedata.csv");
        }

        [TestMethod]
        public void MaxSharpeRatioPortfolioTest()
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
            double rf = 0.05/12;
            var res = PortfolioOptimizer.CalcEfficientFrontier(portf, rf, 50);
            var metrics = (from p in res
                          let sharpe = PortfolioAnalytics.SharpeRatio(p, rf)
                          select new {p.StdDev, p.Mean, sharpe});

            foreach (var m in metrics)
            {
                Console.WriteLine("Risk {0}, Return {1}, Sharpe {2}, Sharpe Ratio {3} ", m.StdDev, m.Mean, (m.Mean-rf)/m.StdDev, m.sharpe);
            }

            var maxsharpe = (from p in res
                             let sharpe = PortfolioAnalytics.SharpeRatio(p, rf)
                             select sharpe).Max();

            Console.WriteLine("Max Sharpe Ratio Portfolio {0}", maxsharpe);
        }
    }
}
