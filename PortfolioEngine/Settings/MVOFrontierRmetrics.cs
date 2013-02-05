using System;
using System.Text;
using System.Collections.Generic;
using DataSciLib.Optimization.QuadProg;
using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.REngine.Rmetrics.Specification;
using DataSciLib.REngine.Rmetrics.Constraints;


namespace PortfolioEngine.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MVOFrontierRmetrics
    {
        private PortfolioSpecification _spec;
        private OptimizationConstraints _constraints;
        private int _numfp;
        private double _rf;
        private PortfolioConfig _portfConf;

        public Dictionary<int, ResultSet<double>> ResultsCollection { get; set; }

        public MVOFrontierRmetrics(PortfolioSettings portfSet)
        {
            _spec = portfSet.Spec;
            _constraints = portfSet.Constr;
            _portfConf = new PortfolioConfig(portfSet);

            ResultsCollection = new Dictionary<int, ResultSet<double>>();
            for (int i = 1; i < portfSet.Spec.NumberofFrontierPoints + 1; i++)
            {
                ResultsCollection.Add(i, new ResultSet<double>(i));
            }
        }

        public void Calculate(double[,] cov, double[] mean)
        {
            
        }

        #region Rmetrics implementation
        /// <summary>
        /// Calculate the efficient frontier given data in timeseries form - the covariance matrix and mean vector is estimated from the data
        /// </summary>
        /// <param name="data">Time series data for the universe of investable instruments</param>
        public void Calculate(ITimeSeries<double> data)
        {
            var portf = calculateEfficientFrontier(data);
            calculateMetrics(portf);
        }

        private fPortfolio calculateEfficientFrontier(ITimeSeries<double> data)
        {
            
            // Call function : return value, function name, parameters...
            fPortfolioSpec specexpr;
            timeSeries dataexpr;
            fPortfolioConstraint constrexpr;

            try
            {
                // Call function : return value, function name, parameters...
                specexpr = fPortfolioSpec.Create(_portfConf.Model, _portfConf.Portfolio, _portfConf.Solver);
                // TODO: first check availability of dataset in R workspace?? Throw exception if not set.
                dataexpr = timeSeries.Create(data);

                constrexpr = fPortfolioConstraint.Create(_portfConf.ConstraintString);

                // Calculate Efficient Frontier
                return fPortfolio.portfolioFrontier(dataexpr, specexpr, constrexpr, true, "Efficient Frontier", "Something here...");
            }
            catch (ApplicationException appexc)
            {
                // Specify Exception in more detail
                throw new ApplicationException(appexc.Message, appexc);
            }
        }

        private void calculateEfficientFrontier(double[,] covariance, IEnumerable<double> meanvector)
        {

        }

        private void calculateMetrics(fPortfolio rmetricsportfolio)
        {
            var covriskb = getCovRiskBudgets(rmetricsportfolio);
            var weights = getWeights(rmetricsportfolio);
            var pmean = getPortfolioMeanReturns(rmetricsportfolio);
            var prisk = getPortfolioRisk(rmetricsportfolio);
            double[] mean = getMeanReturns(rmetricsportfolio);
            var cor = getCorrelationMatrix(rmetricsportfolio);

            int rows = covriskb.GetLength(0);
            int cols = covriskb.GetLength(1);

            double[] riskbudget = new double[cols];
            double[] pweights = new double[cols];

            // Extract data for each portfolio in the collection
            for (int r = 0; r < rows; r++)
            {
                ResultsCollection.Add(r+1, new ResultSet<double>(r + 1));

                // Get Vector metrics
                for (int c = 0; c < cols; c++)
                {
                    riskbudget[c] = covriskb[r, c];
                    pweights[c] = weights[r, c];
                }

                ResultsCollection[r + 1].Metrics.Add(Metrics.Mean, pmean[r, 0]);
                ResultsCollection[r + 1].Metrics.Add(Metrics.StdDev, prisk[r, 0]);
                ResultsCollection[r + 1].VectorMetrics.Add(VMetrics.MarginalRisk, riskbudget);
                ResultsCollection[r + 1].VectorMetrics.Add(VMetrics.MeanReturns, mean);
                ResultsCollection[r + 1].VectorMetrics.Add(VMetrics.Weights, pweights);
                ResultsCollection[r + 1].MatrixMetrics.Add(MatrixMetrics.CorrelationMatrix, cor);
            }
        }

        #endregion

        #region Extractor function wrappers
        private double[,] getCorrelationMatrix(fPortfolio rmetricsportfolio)
        {
            // Get portfolio correlation matrix
            var cor = rmetricsportfolio.GetCorrelationMatrix();
            return cor;
        }

        private double[,] getCovRiskBudgets(fPortfolio rmetricsportfolio)
        {
            double[,] covriskb;
            rmetricsportfolio.GetCovRiskBudgets(out covriskb);

            if (covriskb == null)
                throw new NullReferenceException("Covariance risk budgets is null");

            return covriskb;
        }

        private double[,] getWeights(fPortfolio rmetricsportfolio)
        {
            double[,] weights;
            rmetricsportfolio.GetWeights(out weights);
            return weights;
        }

        private double[] getMeanReturns(fPortfolio rmetricsportfolio)
        {
            // Get mean returns
            double[] mean;
            rmetricsportfolio.GetMeanReturns(out mean);
            return mean;
        }

        private double[,] getPortfolioMeanReturns(fPortfolio rmetricsportfolio)
        {
            // Get portfolio mean returns
            double[,] pmean;
            rmetricsportfolio.GetPortfolioMeanReturns(out pmean);
            return pmean;
        }

        private double[,] getPortfolioRisk(fPortfolio rmetricsportfolio)
        {
            // Get portfolio risk measures
            double[,] risk;
            rmetricsportfolio.GetPortfolioRisk(out risk);
            return risk;
        }

        #endregion

    }
}
