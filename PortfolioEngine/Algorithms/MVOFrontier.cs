using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.Optimization;
using DataSciLib.Statistics;
using MathNet.Numerics.LinearAlgebra.Double;
using PerformanceTools;
using PortfolioEngine.Portfolios;

namespace PortfolioEngine.Settings
{
    public class MVOFrontier
    {
        private uint _numPortfolios;
        private IPortfolio _samplePortfolio;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfSet"></param>
        /// <param name="meanvar"></param>
        public MVOFrontier(IPortfolio samplePortfolio, uint numPortfolios)
        {
            _numPortfolios = numPortfolios;
            _samplePortfolio = samplePortfolio;
        }
        /// <summary>
        /// Calculate the efficient frontier for a given covariance matrix and mean vector
        /// </summary>
        /// <param name="covariance">Covariance Matrix of assets included in portfolio</param>
        /// <param name="mean">Expected return of assets in portfolio</param>
        public IEnumerable<IPortfolio> Calculate()
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

            var meanReturns = from sp in _samplePortfolio
                              select sp.Mean;

            var cov = from sp in _samplePortfolio
                      select new KeyValuePair<string, Dictionary<string, double>>(sp.Name, sp.Covariance);

            var covariance = CovarianceMatrix.Create(cov.ToDictionary(a => a.Key, b => b.Value));
            List<Portfolio> portfolios = new List<Portfolio>();

            // generate sequence of target returns for the efficient frontier and minimum variance locus
            var targetReturns = new double[_numPortfolios].Seq(meanReturns.Min(), meanReturns.Max(), _numPortfolios);
            for (int i = 0; i < _numPortfolios; i++)
            {
                //PerformanceLogger.Start("MVOFrontier", "Calculate", "PortfolioConfig");
                var portfConf = new ConfigurationManager(_samplePortfolio, targetReturns[i]);
                //PerformanceLogger.Stop("MVOFrontier", "Calculate", "PortfolioConfig");

                //PerformanceLogger.Start("MVOFrontier", "Calculate", "QuadProg.Solve");

                var result = QuadProg.Solve(covariance, null, portfConf.Amat.Transpose(), portfConf.Bvec, portfConf.NumEquals);
                //PerformanceLogger.Stop("MVOFrontier", "Calculate", "QuadProg.Solve");
                
                var portf = PortfolioFactory.Create(_samplePortfolio, i.ToString(), targetReturns[i], result.Item2);

                for (int c = 0; c < result.Item1.Length; c++)
                {
                    portf.SetWeight(_samplePortfolio[c].Name, result.Item1[c]);
                }
            }

            return portfolios;
        }
    }
}
