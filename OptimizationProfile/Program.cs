using DataSciLib.DataStructures;
using MathNet.Numerics.Distributions;
using PortfolioEngine;
using PortfolioEngine.Portfolios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizationProfile
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, double> mean;
            CovarianceMatrix cov;

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

            //PortfolioOptimizer.Initialize();

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
                var res = PortfolioOptimizer.CalcEfficientFrontier(portf, rf, 50);
                Console.WriteLine(c);
            }
        }
    }
}
