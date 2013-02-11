// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Linq;
using DataSciLib.REngine;
using RDotNet;
using DataSciLib.REngine.Rmetrics;

namespace DataSciLib.REngine.PerformanceAnalytics
{
    public static class CAPM
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
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] Beta(timeSeries portfolio, timeSeries benchmark, double riskfree)
        {
            Initialize();
            var expr = Engine.CallFunction("CAPM.beta", portfolio.Expression,benchmark.Expression, Engine.RNumeric(riskfree));
            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] Alpha(timeSeries portfolio, timeSeries benchmark, double riskfree)
        {
            Initialize();
            var expr = Engine.CallFunction("CAPM.alpha", portfolio.Expression, benchmark.Expression, Engine.RNumeric(riskfree));
            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] SMLSlope(timeSeries benchmark, double riskfree)
        {
            Initialize();
            var expr = Engine.CallFunction("CAPM.SML.slope", benchmark.Expression, Engine.RNumeric(riskfree));
            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] CMLSlope(timeSeries benchmark, double riskfree)
        {
            Initialize();
            var expr = Engine.CallFunction("CAPM.CML.slope", benchmark.Expression, Engine.RNumeric(riskfree));
            return expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] RiskPremium(timeSeries asset, double riskfree)
        {
            Initialize();
            var expr = Engine.CallFunction("CAPM.RiskPremium", asset.Expression, Engine.RNumeric(riskfree));
            return expr.AsNumeric().ToArray();
        }
    }
}
