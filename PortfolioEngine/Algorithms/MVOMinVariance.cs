using DataSciLib.DataStructures;
using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.Optimization;
using DataSciLib.Statistics;
using MathNet.Numerics.LinearAlgebra.Double;
using PerformanceTools;
using PortfolioEngine.Portfolios;
using PortfolioEngine.Portfolios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioEngine.Settings
{
    public class MVOMinVariance 
    {
        private IPortfolio _samplePortfolio;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfSet"></param>
        /// <param name="meanvar"></param>
        public MVOMinVariance(IPortfolio samplePortfolio)
        {
            _samplePortfolio = samplePortfolio;
        }
        /// <summary>
        /// Calculate the efficient frontier for a given covariance matrix and mean vector
        /// </summary>
        /// <param name="covariance">Covariance Matrix of assets included in portfolio</param>
        /// <param name="mean">Expected return of assets in portfolio</param>
        public IPortfolio Calculate()
        {
            // MVO using quadratic programming
            // Optimal portfolio is:
            //         min(-dvec'bvec+1/2x'Dmat*x) - where ' is the transpose of vector/matrix
            // Dmat - Covariance Matrix - quadratic terms
            // dvec - [0..nAssets] - linear terms
            // Amat - variable weigths in constraints equation
            // bvec - constraint equalities
            // with constraints
            //         A'x >= b
            // meq = eqsumW
            
            // Get mean returns of all instruments in portfolio
            var meanReturns = from sp in _samplePortfolio
                              select sp.Mean;

            // Get covariance matrix for all instruments in portfolio
            var cov = from sp in _samplePortfolio
                      select new KeyValuePair<string, Dictionary<string, double>>(sp.ID, sp.Covariance);

            var covariance = CovarianceMatrix.Create(cov.ToDictionary(a => a.Key, b => b.Value));
            var portfConf = new ConfigurationManager(_samplePortfolio);
            
            OptimizationResult result;
            try
            {
                result = QuadProg.Solve(covariance, null, portfConf.Amat.Transpose(), portfConf.Bvec, portfConf.NumEquals);
            }
            catch (ApplicationException e)
            {
                // Solution not found - create NaN Portfolio
                result = new OptimizationResult(new double[] { double.NaN }, double.NaN);
                // Log error
                Console.WriteLine(e.Message);
            }

            int q = 0;
            double sum = 0;
            foreach (var m in meanReturns)
            {
                sum = sum + m * result.Solution[q];
                q++;
            }

            var portf = PortfolioFactory.Create(_samplePortfolio, _samplePortfolio.Name, sum, result.Value);
            for (int c = 0; c < result.Solution.Length; c++)
            {
                portf[_samplePortfolio[c].ID].Weight = result.Solution[c];
            }

            return portf;
        }
    }

}
