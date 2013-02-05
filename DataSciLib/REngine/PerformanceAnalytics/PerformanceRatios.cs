// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Linq;
using DataSciLib.REngine.Rmetrics;
using RDotNet;

namespace DataSciLib.REngine.PerformanceAnalytics
{
    public static class PerformanceRatios
    {
        public static R Engine = R.Instance;

        private static bool packageloaded = false;

        private static void Initialize()
        {
            if (!packageloaded)
            {
                if (!Engine.IsRunning)
                    throw new ApplicationException("R Engine not yet initialized");

                Engine.LoadPackage("PerformanceAnalytics");
                packageloaded = true;
            }
        }

        /// <summary>
        /// Calculates the Annualized Sharpe Ratio
        /// </summary>
        /// <param name="portfolio">Portfolio returns as a timeSeries object</param>
        /// <param name="riskfree">The risk-free rate with the same periodicity as the portfolio returns</param>
        /// <param name="geometric">True if the geometric mean of the data should be used, false if the returns are already in logarithmic form</param>
        /// <returns></returns>
        public static double[] SharpeRatio(timeSeries portfolio, double riskfree, bool geometric = false)
        {
            Initialize();
            var expr = Engine.CallFunction("SharpeRatio.annualized", new Tuple<string, SymbolicExpression>("R", portfolio.Expression),
                    new Tuple<string, SymbolicExpression>("Rf", Engine.RNumeric(riskfree)),
                    new Tuple<string, SymbolicExpression>("geometric", Engine.RBool(geometric)));
            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] InformationRatio(timeSeries portfolio, timeSeries benchmark)
        {
            Initialize();
            var expr = Engine.CallFunction("InformationRatio", portfolio.Expression, benchmark.Expression);
            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] TreynorRatio(timeSeries portfolio, double riskfree)
        {
            Initialize();
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="minreturn"></param>
        /// <returns></returns>
        public static double[] SortinoRatio(timeSeries portfolio, timeSeries minreturn)
        {
            Initialize();
            var expr = Engine.CallFunction("SortinoRatio", portfolio.Expression, minreturn.Expression);
            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="minreturn"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static double[] UpsidePotentialRatio(timeSeries portfolio, timeSeries minreturn, string method)
        {
            Initialize();
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <param name="riskaversion"></param>
        /// <returns></returns>
        public static double ValueAdded(timeSeries portfolio, timeSeries benchmark, double riskfree, double riskaversion)
        {
            Initialize();
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <returns></returns>
        public static double CalmarRatio(timeSeries portfolio)
        {
            Initialize();
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="excess"></param>
        /// <returns></returns>
        public static double SterlingRatio(timeSeries portfolio, double excess = 0.1)
        {
            Initialize();
            throw new NotImplementedException();
        }
    }
}
