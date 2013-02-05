using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.Optimization.QuadProg;
using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.REngine.Rmetrics.Specification;
using DataSciLib.REngine.Rmetrics.Constraints;
using DataSciLib.REngine.QuadProg;
using DataSciLib.Statistics;
using MathNet.Numerics.LinearAlgebra.Double;
using PerformanceTools;

namespace PortfolioEngine.Settings
{
    public class MVOFrontier
    {
        private PortfolioSettings _portfSet;
        private CovarianceMatrix _covariance;
        private SortedList<string, double> _meanReturns;

        public Dictionary<int, ResultSet<double>> ResultsCollection { get; set; }

        /// <summary>
        /// Create new mean-variance optimization object used for calculating the efficient frontier
        /// </summary>
        /// <param name="portfSet"></param>
        /// <param name="mean"></param>
        /// <param name="covariance"></param>
        public MVOFrontier(PortfolioSettings portfSet, SortedList<string, double> meanReturns, double[,] covariance)
        {
            _portfSet = portfSet;
            _meanReturns = meanReturns;
            _covariance = new CovarianceMatrix(covariance);
            
            ResultsCollection = new Dictionary<int, ResultSet<double>>();
            for (int i = 1; i < portfSet.Spec.NumberofFrontierPoints + 1; i++)
            {
                ResultsCollection.Add(i, new ResultSet<double>(i));
            }
        }

        /// <summary>
        ///  Create new mean-variance optimization object used for calculating the efficient frontier
        /// </summary>
        /// <param name="portfSet"></param>
        /// <param name="mean"></param>
        /// <param name="covariance"></param>
        public MVOFrontier(PortfolioSettings portfSet, SortedList<string, double> meanReturns, CovarianceMatrix covariance)
        {
            _portfSet = portfSet;
            _meanReturns = meanReturns;
            _covariance = covariance;

            ResultsCollection = new Dictionary<int, ResultSet<double>>();
            for (int i = 1; i < portfSet.Spec.NumberofFrontierPoints + 1; i++)
            {
                ResultsCollection.Add(i, new ResultSet<double>(i));
            }
        }
        /// <summary>
        /// Calculate the efficient frontier for a given covariance matrix and mean vector
        /// </summary>
        /// <param name="covariance">Covariance Matrix of assets included in portfolio</param>
        /// <param name="mean">Expected return of assets in portfolio</param>
        public void Calculate()
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

            int numvars = _covariance.ColumnCount;

            // calculate minimum variance portfolio

            // generate sequence of target returns for the efficient frontier locus
            var targetReturns = new double[_portfSet.Spec.NumberofFrontierPoints].Seq(_meanReturns.Values.Min(), _meanReturns.Values.Max(), _portfSet.Spec.NumberofFrontierPoints);
            for (int i = 0; i < _portfSet.Spec.NumberofFrontierPoints; i++)
            {
                PerformanceLogger.Start("MVOFrontier", "Calculate", "PortfolioConfig");
                var portfConf = new PortfolioConfig(_portfSet, _meanReturns, targetReturns[i]);
                PerformanceLogger.Stop("MVOFrontier", "Calculate", "PortfolioConfig");

                PerformanceLogger.Start("MVOFrontier", "Calculate", "QuadProg.Solve");
                var result = QuadProg.Solve(_covariance, null, portfConf.Amat.Transpose(), portfConf.Bvec, portfConf.NumEquals);
                PerformanceLogger.Stop("MVOFrontier", "Calculate", "QuadProg.Solve");

                ResultsCollection[i + 1].Metrics[Metrics.Mean] = targetReturns[i];
                ResultsCollection[i + 1].VectorMetrics[VMetrics.Weights] = result.Item1;
                ResultsCollection[i + 1].Metrics[Metrics.Variance] = result.Item2;
                ResultsCollection[i + 1].Metrics[Metrics.StdDev] = Math.Sqrt(result.Item2);
                ResultsCollection[i + 1].MatrixMetrics[MatrixMetrics.CorrelationMatrix] = _covariance.ToCorrelation();
            }       
        }
    }
}
