using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics;
using RDotNet;

namespace DataSciLib.REngine.Rmetrics.Specification
{
    public class PortfolioInfo
    {
        private SymbolicExpression tgtWeights;
        public SymbolicExpression TargetWeights
        {
            get { return tgtWeights; }
            private set
            {
                tgtWeights = value;
                consistencyChecks();
            }
        }         // numeric vector

        private SymbolicExpression tgtRet;
        public SymbolicExpression TargetReturn
        {
            get { return tgtRet; }
            set
            {
                tgtRet = value;
                consistencyChecks();
            }
        }           // numeric value

        private SymbolicExpression tgtRisk;
        public SymbolicExpression TargetRisk
        {
            get { return tgtRisk; }
            set
            {
                tgtRisk = value;
                consistencyChecks();
            }
        }             // numeric value

        public NumericVector RiskFreeRate { get; set; }            // numeric value
        public NumericVector nFrontierPoints { get; set; }            // integer value
        public int? Status { get; private set; }            // integer value status=0 indicates a succesful termination

        private static R Engine = R.Instance;

        public PortfolioInfo(double[] weights = null, double? targetRet = null, double? targetRisk = null, double riskFreerate = 0, int nfPoints = 50, int? _status = null)
        {
            // -1 values indicate 
            if (weights == null)
                tgtWeights = Engine.RNull();
            else
                tgtWeights = Engine.RVector(weights);

            tgtRet = Engine.RNumeric(targetRet);
            tgtRisk = Engine.RNumeric(targetRisk);
            RiskFreeRate = Engine.RNumeric(riskFreerate);
            nFrontierPoints = Engine.RNumeric(nfPoints);
            Status = _status;

            consistencyChecks();
        }

        public PortfolioInfo(PortfolioInfo portfInfo, int nfPoints = 50)
        {
            TargetWeights = portfInfo.TargetWeights;
            TargetReturn = portfInfo.TargetReturn;
            TargetRisk = portfInfo.TargetRisk;
            RiskFreeRate = portfInfo.RiskFreeRate;
            nFrontierPoints = Engine.RNumeric(nfPoints);
            Status = portfInfo.Status;
        }

        public PortfolioInfo(PortfolioInfo portfInfo, double riskFreeRate = 0)
        {
            TargetWeights = portfInfo.TargetWeights;
            TargetReturn = portfInfo.TargetReturn;
            TargetRisk = portfInfo.TargetRisk;
            RiskFreeRate = Engine.RNumeric(riskFreeRate);
            nFrontierPoints = portfInfo.nFrontierPoints;
            Status = portfInfo.Status;
        }

        public override string ToString()
        {
            return "Not yet implemented for class PortfolioInfo!";
        }

        private void consistencyChecks()
        {
            // The list entries $weights, $targetReturn and $targetRisk from the @portfolio slot have to be considered collectively.
            // For example, if the weights for a portfolio are given, then the target return and target risk are determined, i.e. they are no longer free.
            // As a consequence, if we set the weights to a new value, then the target return and risk also take new values, determined by the portfolio optimization.
            // Since we do not know these values in advance, i.e. when we reset the weights, the values for the target return and risk are both set to NA.
            // The same holds if we assign a new value to the target return or target risk; both of the other values are set to NA. 
            // By default, all three values are set to NULL. If this is the case, then it is assumed that an equal-weights portfolio should be calculated.
            // 
            // In summary, if only one of the three values is different from NULL, then the following procedure will be started:
            // 1. If the weights are specified, it is assumed that a feasible portfolio should be considered.
            // 2. If the target return is fixed, it is assumed that the efficient portfolio with the minimal risk should be considered.
            // 3. And finally if the risk is fixed, the return should be maximized.

            // If targetReturn is specified Weights=NA and TargetRisk=NA, --> minimize Risk
            // If all 3 values are specified default to value for TargetReturn and minimize Risk
            if (tgtRet.IsVector())
            {
                tgtWeights = Engine.RNull();
                tgtRisk = Engine.RNull();
            }

            // If targetRisk is specified Weights=NA and TargetReturn= NA, --> maximize Return
            if (tgtRisk.IsVector())
            {
                tgtWeights = Engine.RNull();
                tgtRet = Engine.RNull();
            }

            // If Weights is specified calculate feasible portfolio
            if (tgtWeights.IsVector())
            {
                tgtRet = Engine.RNull();
                tgtRisk = Engine.RNull();
            }
        }

        internal void SetWeights(double[] weights)
        {
            this.TargetWeights = Engine.RVector(weights);
        }

        internal void SetTargetRisk(double risk)
        {
            this.TargetRisk = Engine.RNumeric(risk);
        }

        internal void SetTargetReturn(double returns)
        {
            this.TargetReturn = Engine.RNumeric(returns);
        }

        private void setRiskFreeRate(double riskfree)
        {
            this.RiskFreeRate = Engine.RNumeric(riskfree); 
        }

        private void setNumberOfFrontierPoints(int numPoints)
        {
            this.nFrontierPoints = Engine.RNumeric(numPoints);
        }

    }
}
