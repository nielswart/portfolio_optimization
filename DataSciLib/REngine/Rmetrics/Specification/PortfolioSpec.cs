using System;
using System.Linq;
using System.Text;
using PortfolioEngine.RInternals;
using PortfolioEngine.Rmetrics;
using PortfolioEngine.Semantics;
using RDotNet;

namespace PortfolioEngine.RInternals.Rmetrics.Specification
{
    public sealed class PortfolioSpec
    {
        private ModelInfo modelInfo;
        private SolverInfo solverInfo;
        private PortfolioInfo portfInfo;

        internal bool ChangedFlag { get; private set; }

        internal PortfolioSpec(ModelInfo mod, SolverInfo opt, PortfolioInfo frontier)
        {
            modelInfo = mod;
            solverInfo = opt;
            portfInfo = frontier;

            // Ensure the solver's objective funtion is the same as that of the model
            solverInfo = new SolverInfo(opt: modelInfo.OptimizationObjective);

            this.ChangedFlag = true;
        }

        internal PortfolioSpec()
        {
            modelInfo = new ModelInfo();
            solverInfo = new SolverInfo();
            portfInfo = new PortfolioInfo();

            // Ensure the solver's objective funtion is the same as that of the model
            solverInfo = new SolverInfo(opt: modelInfo.OptimizationObjective);

            this.ChangedFlag = true;
        }

        /*
        internal string ToRValue(Varnames variable)
        {
            string command = "portfolioSpec(" +
                "model=list(type=" + modelInfo.Type.ToRValue() +                    // modeltype
                    ",optimize=" + modelInfo.OptimizationObjective.ToRValue() +     // objective function
                    ",estimator=" + modelInfo.Estimator.ToRValue() +                // estimator
                    ",tailRisk=list()," +
                    "params=list(alpha=0.05))," +
                "portfolio=list(weights=" + variable.Weights +       // target weights (default to NULL, dependent on objective function etc.. - correctly implement business rules
                    ",targetReturn=" + variable.Return +       // target return (default to NULL, dependent on objective function etc.. - 
                    ",targetRisk=" + variable.Risk +           // "
                    ",riskFreeRate=" + variable.RiskFreeRate +       // default to 0
                    ",nFrontierPoints=" + variable.NumberofFrontierPoints + // default to 50
                    ",status=NA" +
                    ")," +
                "optim=list(solver=" + solverInfo.Solver.ToRValue() +
                    ",objective=" + solverInfo.OptimizationObjective.ToRValue() +            // default to NULL
                    ",params=list()" +
                    ",control=list()" +
                    ",trace=FALSE))";

            return command;
        }

         * */


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Hash: " + this.GetHashCode());
            sb.AppendLine(modelInfo.ToString()).AppendLine(solverInfo.ToString()).AppendLine(portfInfo.ToString());

            return sb.ToString();
        }

        internal void Execute(R Engine, Varnames variable)
        {
            string spec = variable.Spec;
            //Console.WriteLine("Creating specification object {0} in R...", spec);

            // Load all the necessary variables into R Environment (all the non-string parameters)
            portfInfo.Create(Engine, variable);

            // Execute PortfolioSpec in R
            Engine.Workspace.SetVariable(spec, this.ToRValue(variable));

            // R version and this version of portfoliospec is in sync
            this.ChangedFlag = false;
        }

        // Check if al the specifications is consistent with each other e.g. correct solver for model type and objectives etc
        private void runConsistencyChecks()
        {
            throw new NotImplementedException();
        }

        #region Set Portfolio Specifications
        // Setters
        internal void setModelType(ModelType mtype)
        {
            var estim = modelInfo.Estimator;
            var obj = modelInfo.OptimizationObjective;

            // Overwrite current model
            this.modelInfo = new ModelInfo(type: mtype, opt: obj, est: estim);

            this.ChangedFlag = true;
        }

        internal void setModelObjective(Objective obj)
        {
            var estim = modelInfo.Estimator;
            var mtype = modelInfo.Type;

            // Overwrite current model
            this.modelInfo = new ModelInfo(type: mtype, opt: obj, est: estim);

            // Ensure integrity
            this.solverInfo = new SolverInfo(opt: modelInfo.OptimizationObjective);
            this.ChangedFlag = true;

        }

        internal void setModelEstimator(Estimator estim)
        {
            var mtype = modelInfo.Type;
            var obj = modelInfo.OptimizationObjective;

            this.modelInfo = new ModelInfo(type: mtype, opt: obj, est: estim);

            this.ChangedFlag = true;
        }

        /// <summary>
        /// Sets the solver to use for the optimization
        ///  - When an incorrect solver is specified an alternative will automatically be chosen instead
        /// </summary>
        /// <param name="slvrtype"></param>
        internal void setSolver(SolverType slvrtype)
        {
            var obj = solverInfo.OptimizationObjective;
            var trace = solverInfo.Trace;

            // Implement rules for ensuring the correct solver is chosen
            if (modelInfo.Type == ModelType.MarkowitzMeanVariance && obj == Objective.MinimizeRisk)
            {
                // Solver type must be either QP or if unconstrained Analytic
                if (slvrtype == SolverType.QP || slvrtype == SolverType.Analytic)
                    this.solverInfo = new SolverInfo(slvr: slvrtype, opt: obj, trace: trace);
            }
            else if (modelInfo.Type == ModelType.MarkowitzMeanVariance && obj == Objective.MaximizeReturn)
            {
                // Do some stuff here
            }
            else
            {
                // no reason why it can't be changed to whatever the user wants
                this.solverInfo = new SolverInfo(slvr: slvrtype, opt: obj, trace: trace);
            }

            this.ChangedFlag = true;

        }

        internal void setSolverTrace(bool trace)
        {
            throw new NotImplementedException();
        }

        internal void setSolverParams(params string[] ptionalParameters)
        {
            throw new NotImplementedException();
        }

        internal void setTargetWeights(double[] w)
        {
            this.portfInfo = new PortfolioInfo(weights: w, riskFreerate: this.portfInfo.RiskFreeRate, nfPoints: this.portfInfo.nFrontierPoints);
            this.ChangedFlag = true;
        }

        internal void setTargetReturn(double targetReturn)
        {
            // Automatically set objective
            this.setModelObjective(Objective.MinimizeRisk);

            this.portfInfo.TargetReturn = targetReturn;

            this.ChangedFlag = true;
        }

        /// <summary>
        /// 
        ///  - Default to null wehen calculating efficient frontier
        /// </summary>
        /// <param name="targetrisk"></param>
        /// <returns></returns>
        internal void setTargetRisk(double targetrisk)
        {
            // Automatically set objective
            this.setModelObjective(Objective.MaximizeReturn);

            this.portfInfo = new PortfolioInfo(targetRisk: targetrisk, riskFreerate: this.portfInfo.RiskFreeRate, nfPoints: this.portfInfo.nFrontierPoints);

            this.ChangedFlag = true;
        }

        internal void setRiskFreeRate(double riskfree)
        {
            this.portfInfo = new PortfolioInfo(this.portfInfo, riskfree);

            this.ChangedFlag = true;
        }

        internal void setNumberOfFrontierPoints(int numPoints)
        {
            this.portfInfo = new PortfolioInfo(this.portfInfo, nfPoints: numPoints);

            this.ChangedFlag = true;
        }

        /*  Optim slot - Constructor functions:
        setControl sets the control list of the solver
        */

        #endregion

        #region Get Portfolio Specifications
        // Getters

        /*
            *  Optim slot - Extractor functions:
            getSolver Extracts the name of the solver
            getTrace Extracts solver's trace flag
            getObjective Extracts the name of the objective function
            getOptions Extracts optional solver parameters
            getControl Extracts the control list of the solver
            * 
         * 
         * Portfolio Slot - Extractor Functions:
            getWeights Extracts weights from a portfolio object
            getTargetReturn Extracts target return from specification
            getTargetRisk Extracts target risk from specification
            getRiskFreeRate Extracts risk-free rate from specification
            getNFrontierPoints Extracts number of frontier points
            getStatus Extracts the status of optimization
            * 
         * */

        internal int getNumberOfFrontierPoints(R Engine, string objname)
        {
            var expr = "getNFrontierPoints(" + objname + "_spec" + ")";
            Engine.Workspace.SetVariable("nfp", expr);

            var nfp = Engine.Workspace.GetVariable("nfp").AsNumeric().First();

            return (int)nfp;
        }

        public void getModelType()
        {

        }
        #endregion
    }
}
