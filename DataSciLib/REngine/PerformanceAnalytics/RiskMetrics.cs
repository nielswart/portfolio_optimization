// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System;
using System.Linq;
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics;
using RDotNet;

namespace DataSciLib.REngine.PerformanceAnalytics
{
    public static class RiskMetrics
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
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double[] ConditionalVaR(timeSeries portfolio, double p)
        {
            Initialize();
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double[] HistoricalVaR(timeSeries portfolio, double p)
        {
            Initialize();
            var expr = Engine.CallFunction("VaR", new Tuple<string, SymbolicExpression>("R",portfolio.Expression),
                new Tuple<string, SymbolicExpression>("p", Engine.RNumeric(p)),
                new Tuple<string, SymbolicExpression>("method", Engine.RString("historical")));

            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double[] GaussianVaR(timeSeries portfolio, double p)
        {
            Initialize();          

            var expr = Engine.CallFunction("VaR", new Tuple<string, SymbolicExpression>("R", portfolio.Expression),
                new Tuple<string, SymbolicExpression>("p", Engine.RNumeric(p)),
                new Tuple<string, SymbolicExpression>("method", Engine.RString("gaussian")));

            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double[] CornishFisherVaR(timeSeries portfolio, double p)
        {
            Initialize();
            
            var expr = Engine.CallFunction("VaR", new Tuple<string, SymbolicExpression>("R", portfolio.Expression),
                new Tuple<string, SymbolicExpression>("p", Engine.RNumeric(p)),
                new Tuple<string, SymbolicExpression>("method", Engine.RString("modified")));

            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <returns></returns>
        public static double[] AnnualisedStdDev(timeSeries portfolio)
        {
            Initialize();
            var expr = Engine.CallFunction("sd.annualized", portfolio.Expression);
            return expr.AsNumeric().ToArray();
        }

        public static double[] TrackingError(timeSeries portfolio, timeSeries benchmark)
        {
            Initialize();
            throw new NotImplementedException();
        }

        public static double[] SemiDeviation(timeSeries portfolio)
        {
            Initialize();
            throw new NotImplementedException();
        }

        public static double[] SemiVariance(timeSeries portfolio)
        {
            Initialize();
            throw new NotImplementedException();
        }

        public static double[] Skewness(timeSeries portfolio)
        {
            Initialize();
            throw new NotImplementedException();
        }

        public static double[] Kurtosis(timeSeries portfolio)
        {
            Initialize();
            throw new NotImplementedException();
        }

        public static timeSeries ResidualRisk(timeSeries asset, timeSeries benchmark, double riskfree)
        {
            Initialize();
            throw new NotImplementedException();
        }
    }
}
