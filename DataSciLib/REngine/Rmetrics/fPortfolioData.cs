// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Data;
using DataSciLib.DataStructures;
using RDotNet;
using DataSciLib.REngine.Rmetrics.Specification;

namespace DataSciLib.REngine.Rmetrics
{
    public sealed class fPortfolioData : S4Class
    {
        private static bool packageloaded = false;
        private static void Initialize()
        {
            if (!packageloaded)
            {
                Engine.LoadPackage("fPortfolio");
                packageloaded = true;
            }
        }

        private fPortfolioData(SymbolicExpression expression)
            : base(expression)
        {

        }

        private static Function portfolioData()
        {
            Initialize();
            return Engine.GetFunction("portfolioData");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSeriesData"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static fPortfolioData Create(timeSeries timeseries, fPortfolioSpec spec)
        {
            return new fPortfolioData(portfolioData().Invoke(new SymbolicExpression[] { timeseries.Expression, spec.Expression }));
        }

        
        public static fPortfolioData Create(ITimeSeries<double> timeseries, fPortfolioSpec spec)
        {
            return new fPortfolioData(portfolioData().Invoke(new SymbolicExpression[] { timeSeries.Create(timeseries.Data, 
                timeseries.DateTime, timeseries.Name).Expression, spec.Expression }));
        }

        
    }
}
