// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RDotNet;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.DataStructures;

namespace DataSciLib.REngine.Rmetrics
{
    public sealed class fPortfolio : S4Class
    {
        private static bool packageloaded = false;

        private fPortfolio(SymbolicExpression expression)
            : base(expression)
        {
        }

        ~fPortfolio()
        {
            //if (Expression.IsProtected)
            //    Expression.Unprotect();
        }

        private static void Initialize()
        {
            if (!packageloaded)
            {
                Engine.LoadPackage("fPortfolio");
                Engine.LoadPackage("quadprog");
                packageloaded = true;
            }
        }

        #region Rmetrics Portfolio Methods

        private static Function feasiblePortfolio()
        {
            Initialize();
            return Engine.GetFunction("feasiblePortfolio");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="spec"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static fPortfolio feasiblePortfolio(timeSeries data, fPortfolioSpec spec, fPortfolioConstraint constraint)
        {
            Initialize();
            return new fPortfolio(Engine.CallFunction("feasiblePortfolio", data.Expression, spec.Expression, constraint.Expression));
        }

        private static Function efficientPortfolio()
        {
            Initialize();
            return Engine.GetFunction("efficientPortfolio");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="spec"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static fPortfolio efficientPortfolio(SymbolicExpression data, SymbolicExpression spec, SymbolicExpression constraint)
        {
            Initialize();
            var expr =  efficientPortfolio().Invoke( new SymbolicExpression[] {data, spec, constraint});
            return new fPortfolio(expr);
        }

        private static Function portfolioFrontier()
        {
            Initialize();
            return Engine.GetFunction("portfolioFrontier");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="spec"></param>
        /// <param name="constraint"></param>
        /// <param name="IncludeMinVarLocus"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static fPortfolio portfolioFrontier(timeSeries data, fPortfolioSpec spec, fPortfolioConstraint constraint, 
            bool IncludeMinVarLocus = true, string title = "Title", string description = "Description")
        {
            Initialize();
            var expr = Engine.CallFunction("portfolioFrontier", new Tuple<string, SymbolicExpression>("data", data.Expression), new Tuple<string, SymbolicExpression>("spec", spec.Expression),
                new Tuple<string, SymbolicExpression>("constraints", constraint.Expression), new Tuple<string, SymbolicExpression>("include.mvl", Engine.RBool(IncludeMinVarLocus)),
                new Tuple<string, SymbolicExpression>("title", Engine.RString(title)), new Tuple<string, SymbolicExpression>("description", Engine.RString(description)));
            
            return new fPortfolio(expr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static fPortfolio Create(ITimeSeries<double> portfolio, double[] weights)
        {
            try
            {
                var specexpr = fPortfolioSpec.Create(weights);
                var constrexpr = fPortfolioConstraint.Create();

                return feasiblePortfolio(timeSeries.Create(portfolio.DataMatrix,
                    portfolio.DateTime, portfolio.Names), specexpr, constrexpr);
            }
            catch (REngineException e)
            {
                // Log to logger
                Console.WriteLine("REngineException thrown; automatically retrying to recover from error");
                return Create(portfolio, weights, 2);
            }
        }

        public static fPortfolio Create(ITimeSeries<double> portfolio, IEnumerable<double> weights)
        {
            
            int i = 0;
            foreach (var w in weights)
            {
                i++;
            }
            double[] pw = new double[i];
            int c= 0;
            foreach (var w in weights)
            {
                pw[c] = w;
            }

            var specexpr = fPortfolioSpec.Create(pw);
            var constrexpr = fPortfolioConstraint.Create();

            return feasiblePortfolio(timeSeries.Create(portfolio.DataMatrix,
                    portfolio.DateTime, portfolio.Names), specexpr, constrexpr);
        }

        private static fPortfolio Create(ITimeSeries<double> portfolio, double[] weights, int recover)
        {
            try
            {
                var specexpr = fPortfolioSpec.Create(weights);
                var constrexpr = fPortfolioConstraint.Create();

                return feasiblePortfolio(timeSeries.Create(portfolio.DataMatrix,
                    portfolio.DateTime, portfolio.Names), specexpr, constrexpr);
            }
            catch (REngineException e)
            {
                Console.WriteLine("REngineException thrown; automatically retrying to recover from error");
                if (recover<=0)
                    throw new ApplicationException("Could not recover from error: operation failed");

                return Create(portfolio, weights, recover - 1);
            }
        }

        #endregion

#if DEBUG
        public static bool IsLoaded()
        {
            return packageloaded;
        }
#endif

    }
}
