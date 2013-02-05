using System;
using System.Collections.Generic;
using DataSciLib.DataStructures;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.REngine.Rmetrics.Specification;
using DataSciLib.REngine.Rmetrics.Constraints;

namespace PortfolioEngine.Settings
{
    public class MVOMinVariance 
    {
        private PortfolioSpecification _spec;
        private OptimizationConstraints _constraints;
        private int _numfp;
        private double _rf;

        public ResultSet<double> Results { get; protected set; }

        public MVOMinVariance(PortfolioSpecification spec, OptimizationConstraints constr)
        {
            this._spec = spec;
            this._constraints = constr;

            //Results = new Dictionary<int, ResultSet<double>>();
        }

        public MVOMinVariance(PortfolioSettings portfset)
        {
            this._spec = portfset.Spec;
            this._constraints = portfset.Constr;

        }

        public void Calculate()
        {

        }
        
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
