// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.REngine.PerformanceAnalytics;
using System;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace PortfolioEngine
{
    public class Result<Tval> : ArrayItem<string, Tval>
    {
        public string ID
        {
            get { return _id; }
        }

        public Tval Value
        {
            get { return _value; }
        }

        public Result(string id, Tval value)
            : base(id, value)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class Analytics
    {
        #region Static Constructor
        /// <summary>
        /// R Engine Instance
        /// </summary>
        public static R Engine;

        /// <summary>
        /// Warning message that should be passed to the user
        /// </summary>
        public static string WarningMessage { get; private set; }

        /// <summary>
        /// Constructor of Performance class - automatically initializes and starts the R Engine
        /// </summary>
        static Analytics()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);

            WarningMessage = "No warnings";
        }

        #endregion

        public static double AnnualisedReturn(ITimeSeries<double> timeseries)
        {
            return timeseries.AnnualisedMean();
        }

        public static double AnnualisedStdDev(ITimeSeries<double> timeseries)
        {
            return timeseries.AnnualisedStdDev();
        }

        /// <summary>
        /// Calculate the realised alpha with the same periodicity as the returns
        /// </summary>
        /// <param name="asset">The asset time series to use in calculating alpha</param>
        /// <param name="benchmark">The benchmark index/portfolio to calculate alpha against</param>
        /// <param name="riskfree">The risk-free rate with the same periodicity as the returns</param>
        /// <returns>Array of alphas, one for each series in asset time series</returns>
        public static double RealisedAlpha(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            return CAPM.Beta(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree).First();
        }

        /// <summary>
        /// Calculate the realised beta (ex-post)
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double RealisedBeta(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("Time series should be of the same length");

            return CAPM.Beta(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree).First();
        }

        /// <summary>
        /// Calculate the annualized Sharpe Ratio of each return series in the ITimeSeries object
        /// </summary>
        /// <param name="portfolio">Portfolio or asset return time series</param>
        /// <param name="riskfree">The risk-free rate with the same periodicity as the returns, passed as a fraction not percentage</param>
        /// <returns>Array of Sharpe Ratios, one for every returns series</returns>
        public static double SharpeRatio(ITimeSeries<double> portfolio, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");
            
            var res = (portfolio.AnnualisedMean() - riskfree) / portfolio.AnnualisedStdDev();
            return res;
        }

        public static double ActivePremium(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            var ap = portfolio.AnnualisedMean() - benchmark.AnnualisedMean();
            return ap;
        }

        public static double TrackingError(TimeSeries portfolio, TimeSeries benchmark)
        {
            var te = (portfolio - benchmark).AnnualisedStdDev();
            return te;
        }

        public static double InformationRatio(TimeSeries portfolio, TimeSeries benchmark)
        {
            if (portfolio.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            return Analytics.ActivePremium(portfolio, benchmark) / Analytics.TrackingError(portfolio, benchmark);
        }

        public static double VaR(ITimeSeries<double>portfolio, double percentile = 0.95, VaRMethod vartype = PortfolioEngine.VaRMethod.CornishFisher)
        {
            if (percentile > 1)
                percentile = percentile / 100;
            if (percentile <= 0 || percentile > 1)
                throw new ArgumentException("Percentile value should be between 0 and 1 or 0 and 100");
            
            switch (vartype)
            {
                case PortfolioEngine.VaRMethod.Gaussian:
                    return RiskMetrics.GaussianVaR(timeSeries.Create(portfolio), percentile).First();

                case PortfolioEngine.VaRMethod.Historical:
                    return RiskMetrics.HistoricalVaR(timeSeries.Create(portfolio), percentile).First();

                case PortfolioEngine.VaRMethod.CornishFisher:
                    return RiskMetrics.CornishFisherVaR(timeSeries.Create(portfolio), percentile).First();

                default:
                    return RiskMetrics.GaussianVaR(timeSeries.Create(portfolio), percentile).First();
            }
        }

        public static double SortinoRatio(ITimeSeries<double> portfolio, ITimeSeries<double> targetReturn)
        {
            if (portfolio.RowCount != targetReturn.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            try
            {
                return PerformanceRatios.SortinoRatio(timeSeries.Create(portfolio), timeSeries.Create(targetReturn)).First();
            }
            catch (REngineException e)
            {
                throw new ApplicationException(e.Message);
            }
        }
    }
}
