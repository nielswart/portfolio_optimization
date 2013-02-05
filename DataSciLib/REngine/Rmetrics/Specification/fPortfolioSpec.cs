using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;

namespace DataSciLib.REngine.Rmetrics.Specification
{
    public sealed class fPortfolioSpec : S4Class
    {
        private static bool packageloaded = false;

        private fPortfolioSpec(SymbolicExpression expression)
            : base(expression)
        {
            
        }

        ~fPortfolioSpec()
        {
            //if (Expression.IsProtected)
            //    Expression.Unprotect();
        }

        private static void Initialize()
        {
            if (!packageloaded)
            {
                Engine.LoadPackage("fPortfolio");
                packageloaded = true;
            }
        }

        private static Function portfolioSpec()
        {
            return Engine.GetFunction("portfolioSpec");
        }

        #region Public Static Methods

        /// <summary>
        /// Create fPortfolioSpec object in R runtime environment
        /// </summary>
        /// <returns>SymbolicExpression (R S4 object)</returns>
        public static fPortfolioSpec Create()
        {
            Initialize();
            var expr = Engine.RunCommand("portfolioSpec()");
            return new fPortfolioSpec(expr);
        }
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static fPortfolioSpec Create(ModelInfo modelInfo, PortfolioInfo portfInfo, SolverInfo solverInfo)
        {
            Initialize();
            var expr = CreateExpressions(modelInfo, portfInfo, solverInfo);
            var specexpr = portfolioSpec().Invoke(new SymbolicExpression[] { expr.Item1, expr.Item2, expr.Item3 });

            return new fPortfolioSpec(specexpr);
        }

        public static fPortfolioSpec Create(double[] weights)
        {
            Initialize();
            ModelInfo modelInfo = new ModelInfo();
            PortfolioInfo portfInfo = new PortfolioInfo(weights: weights);
            SolverInfo solverInfo = new SolverInfo();

            var expr = CreateExpressions(modelInfo, portfInfo, solverInfo);
            return new fPortfolioSpec(Engine.CallFunction("portfolioSpec", expr.Item1, expr.Item2, expr.Item3 ));
        }

        #endregion

        private static Tuple<SymbolicExpression, SymbolicExpression, SymbolicExpression> CreateExpressions(ModelInfo modelInfo, 
            PortfolioInfo portfInfo, SolverInfo solverInfo)
        {
            var modelexpr = Engine.RList(new Tuple<string, SymbolicExpression>("type", Engine.RString(modelInfo.Type.ToRString())),                    // modeltype
                    new Tuple<string, SymbolicExpression>("optimize", Engine.RString(modelInfo.OptimizationObjective.ToRString())),     // objective function
                    new Tuple<string, SymbolicExpression>("estimator",  Engine.RString(modelInfo.Estimator.ToRString())),               // estimator
                    new Tuple<string, SymbolicExpression>("tailRisk", Engine.RList()),
                    new Tuple<string, SymbolicExpression>("params", Engine.RList(new Tuple<string, SymbolicExpression>("alpha", Engine.RNumeric(0.05)))));

            var portfolioexpr = Engine.RList(new Tuple<string, SymbolicExpression>("weights", portfInfo.TargetWeights),         // target weights (default to NULL, dependent on objective function etc.. - correctly implement business rules
                    new Tuple<string, SymbolicExpression>("targetReturn", portfInfo.TargetReturn),                                  // target return (default to NULL, dependent on objective function etc.. - 
                    new Tuple<string, SymbolicExpression>("targetRisk", portfInfo.TargetRisk),                                     // "
                    new Tuple<string, SymbolicExpression>("riskFreeRate", portfInfo.RiskFreeRate),                            // default to 0
                    new Tuple<string, SymbolicExpression>("nFrontierPoints", portfInfo.nFrontierPoints),               // default to 50
                    new Tuple<string, SymbolicExpression>("status", Engine.R_NA()));

            var optimexpr = Engine.RList(new Tuple<string, SymbolicExpression>("solver", Engine.RString(solverInfo.Solver.ToRString())),
                    new Tuple<string, SymbolicExpression>("objective", Engine.RString(solverInfo.OptimizationObjective.ToRString())),            
                    new Tuple<string, SymbolicExpression>("params", Engine.RList()),
                    new Tuple<string, SymbolicExpression>("control", Engine.RList()),
                    new Tuple<string, SymbolicExpression>("trace", Engine.RBool(false)));

            return new Tuple<SymbolicExpression, SymbolicExpression, SymbolicExpression>(modelexpr, portfolioexpr, optimexpr);
        }
    }
}
