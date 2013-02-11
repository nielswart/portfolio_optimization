// Copyright (c) 2012: DJ Swart, AJ Hoffman
//
using System;
using System.Linq;
using DataSciLib.REngine;
using RDotNet;
using DataSciLib.REngine.Rmetrics;

namespace DataSciLib.REngine.PerformanceAnalytics
{
    public static class ReturnMetrics
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

        public static double[] ActivePremium(timeSeries portfolio, timeSeries benchmark)
        {
            Initialize();
            var expr = Engine.CallFunction("ActivePremium", portfolio.Expression, benchmark.Expression);
            return expr.AsNumeric().ToArray();
        }

        public static double[] AnnualisedReturn(timeSeries portfolio, bool geometric = false)
        {
            Initialize();
            var expr = Engine.CallFunction("Return.annualized", new Tuple<string, SymbolicExpression>("R", portfolio.Expression),
                    new Tuple<string, SymbolicExpression>("geometric", Engine.RBool(geometric)));
            return expr.AsNumeric().ToArray();
        }

        public static double[] ExcessReturn(timeSeries portfolio, timeSeries benchmark)
        {
            Initialize();
            var expr = Engine.CallFunction("Return.excess", portfolio.Expression, benchmark.Expression);
            return expr.AsNumeric().ToArray();
        }
    }
}
