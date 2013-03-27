// Copyright (c) 2013: DJ Swart, AJ Hoffman

using DataSciLib.DataStructures;
using DataSciLib.REngine.Optimization;
using PortfolioEngine.Portfolios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioEngine.Settings
{
    public class MVOFrontier_R
    {
        private uint _numPortfolios;
        private IPortfolio _samplePortfolio;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfSet"></param>
        /// <param name="meanvar"></param>
        public MVOFrontier_R(IPortfolio samplePortfolio, uint numPortfolios)
        {
            _numPortfolios = numPortfolios;
            _samplePortfolio = samplePortfolio;
        }
        /// <summary>
        /// Calculate the efficient frontier for a given covariance matrix and mean vector
        /// </summary>
        /// <param name="covariance">Covariance Matrix of assets included in portfolio</param>
        /// <param name="mean">Expected return of assets in portfolio</param>
        public IEnumerable<IPortfolio> Calculate(int digits = 4)
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

            var cov = from ins in _samplePortfolio
                      select new KeyValuePair<string, Dictionary<string, double>>(ins.ID, ins.Covariance);

            var covariance = CovarianceMatrix.Create(cov.ToDictionary(a => a.Key, b => b.Value));
            List<IPortfolio> portfolios = new List<IPortfolio>();

            // generate sequence of target returns for the efficient frontier and minimum variance locus
            var targetReturns = new double[_numPortfolios].Seq(meanReturns.Min(), meanReturns.Max(), (int)_numPortfolios, digits);
            // For every target return find the minimum variance portfolio
            for (int i = 0; i < _numPortfolios; i++)
            {
                var portfConf = new ConfigurationManager(_samplePortfolio, targetReturns[i]);
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
                var portf = PortfolioFactory.Create(_samplePortfolio, i.ToString(), targetReturns[i], result.Value);
                for (int c = 0; c < result.Solution.Length; c++)
                {
                    portf[_samplePortfolio[c].ID].Weight = result.Solution[c];
                }
                portfolios.Add(portf);
            }

            return portfolios;
        }
    }
}
