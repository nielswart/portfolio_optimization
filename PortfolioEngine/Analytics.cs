// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.PerformanceAnalytics;
using DataSciLib.REngine.Rmetrics;
using System;

namespace PortfolioEngine
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Analytics
    {
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
        public Analytics()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);

            WarningMessage = "No warnings";
        }

        /// <summary>
        /// Constructor of Performance class - pass initialized R Engine
        /// </summary>
        /// <param name="engine">R Engine - will be started if not yet intialized and started</param>
        public Analytics(R engine)
        {
            Engine = engine;
            if (!Engine.IsRunning)
                Engine.Start(REngineOptions.QuietMode);

            WarningMessage = "No warnings";
        }

        /// <summary>
        /// Calculates the annualized return for each return series in the ITimeSeries object
        /// </summary>
        /// <param name="portfolio">Portfolio or Asset returns</param>
        /// <returns>Array of annulaized returns</returns>
        public static double[] AnnualisedReturn(ITimeSeries<double> timeseries)
        {
            return ReturnMetrics.AnnualisedReturn(timeSeries.Create(timeseries.DataMatrix, 
                timeseries.DateTime, timeseries.Names));
        }

        /// <summary>
        /// Calculate the annualized standard deviation of each return series in the ITimeSeries object
        /// </summary>
        /// <param name="portfolio"></param>
        /// <returns></returns>
        public static double[] AnnualisedStdDev(ITimeSeries<double> timeseries)
        {
            return RiskMetrics.AnnualisedStdDev(timeSeries.Create(timeseries.DataMatrix,
                timeseries.DateTime, timeseries.Names));
        }

        /// <summary>
        /// Calculate the realised alpha with the same periodicity as the returns
        /// </summary>
        /// <param name="asset">The asset time series to use in calculating alpha</param>
        /// <param name="benchmark">The benchmark index/portfolio to calculate alpha against</param>
        /// <param name="riskfree">The risk-free rate with the same periodicity as the returns</param>
        /// <returns>Array of alphas, one for each series in asset time series</returns>
        public static double[] RealisedAlpha(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            if (benchmark.ColumnCount != 1)
                throw new ArgumentException("Minimum Acceptable Return TimeSeries should be a single return series");

            try
            {
                return CAPM.Alpha(timeSeries.Create(asset.DataMatrix, asset.DateTime, asset.Names), 
                    timeSeries.Create(benchmark.DataMatrix, benchmark.DateTime, benchmark.Names), riskfree);
            }
            catch (ApplicationException e)
            {
                // NOTE: Log Errors instead of writing to console?
                Console.WriteLine("Exception thrown by application: " + e.Message);
                Console.WriteLine("Automatically retrying; if it fails again, an exception is thrown");

                return CAPM.Alpha(timeSeries.Create(asset.DataMatrix, asset.DateTime, asset.Names), 
                    timeSeries.Create(benchmark.DataMatrix, benchmark.DateTime, benchmark.Names), riskfree);
            }
        }

        /// <summary>
        /// Calculate the realised beta (ex-post)
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns></returns>
        public static double[] RealisedBeta(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            if (benchmark.ColumnCount != 1)
                throw new ArgumentException("Minimum Acceptable Return TimeSeries should be a single return series");

            try
            {
                return CAPM.Beta(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree);
            }
            catch (ApplicationException e)
            {
                // NOTE: Log Errors instead of writing to console?
                Console.WriteLine("Exception thrown by application: " + e.Message);
                Console.WriteLine("Automatically retrying; if it fails again, an exception is thrown");

                return CAPM.Beta(timeSeries.Create(asset), timeSeries.Create(benchmark), riskfree);
            }
        }

        /// <summary>
        /// Calculate the annualized Sharpe Ratio of each return series in the ITimeSeries object
        /// </summary>
        /// <param name="portfolio">Portfolio or asset return time series</param>
        /// <param name="riskfree">The risk-free rate with the same periodicity as the returns, passed as a fraction not percentage</param>
        /// <returns>Array of Sharpe Ratios, one for every returns series</returns>
        public static double[] SharpeRatio(ITimeSeries<double> portfolio, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");
            
            //(Statistics.Mean(returns) * 12) / (Statistics.StandardDeviation(returns) * Math.Sqrt(12))

            return PerformanceRatios.SharpeRatio(timeSeries.Create(portfolio), riskfree);
        }

        public static double SharpeRatio(IPortfolio portfolio, double riskfree = 0)
        {
            throw new NotImplementedException();
        }

        public static double[] ActivePremium(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            throw new NotImplementedException();
        }

        public static double[] TrackingError(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            throw new NotImplementedException();
        }

        public static double[] InformationRatio(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            if (portfolio.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            if (benchmark.ColumnCount != 1)
                throw new ArgumentException("Minimum Acceptable Return TimeSeries should be a single return series");

            return PerformanceRatios.InformationRatio(timeSeries.Create(portfolio), timeSeries.Create(benchmark));
        }

        public static double[] VaR(ITimeSeries<double>portfolio, double percentile = 0.95, VaRMethod vartype = PortfolioEngine.VaRMethod.Gaussian)
        {
            if (percentile > 1)
                percentile = percentile / 100;
            if (percentile <= 0 || percentile > 1)
                throw new ArgumentException("Percentile value should be between 0 and 1 or 0 and 100");

            switch (vartype)
            {
                case PortfolioEngine.VaRMethod.Gaussian:
                    return RiskMetrics.GaussianVaR(timeSeries.Create(portfolio), percentile);

                case PortfolioEngine.VaRMethod.Historical:
                    return RiskMetrics.HistoricalVaR(timeSeries.Create(portfolio), percentile);

                case PortfolioEngine.VaRMethod.CornishFisher:
                    return RiskMetrics.CornishFisherVaR(timeSeries.Create(portfolio), percentile);

                default:
                    return RiskMetrics.GaussianVaR(timeSeries.Create(portfolio), percentile);;
            }
        }

        public static double[] SortinoRatio(ITimeSeries<double> portfolio, ITimeSeries<double> targetReturn)
        {
            if (portfolio.RowCount != targetReturn.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            if (targetReturn.ColumnCount != 1 )
                throw new ArgumentException("Minimum Acceptable Return TimeSeries should be a single return series");

            try
            {
                return PerformanceRatios.SortinoRatio(timeSeries.Create(portfolio), timeSeries.Create(targetReturn));
            }
            catch (REngineException e)
            {
                // NOTE: Log Errors instead of writing to console?
                Console.WriteLine("Exception thrown by application: " + e.Message);
                Console.WriteLine("Automatically retrying; if it fails again, an exception is thrown");

                throw new ApplicationException(e.Message);
            }
        }

        public static double[,] CorrelationMatrix(ITimeSeries<double> assets)
        {
            if (assets.ColumnCount < 2)
                throw new ArgumentException("Can't calculate correlation matrix of a single series");

            var weight = 1.0 / assets.ColumnCount;
            double[] weights = new double[assets.ColumnCount];

            for (int i = 0; i < assets.ColumnCount; i++)
            {
                weights[i] = weight;
            }

            try
            {
                return fPortfolio.Create(assets, weights).GetCorrelationMatrix();
            }
            catch (REngineException e)
            {
                // NOTE: Log Errors instead of writing to console?
                Console.WriteLine("Exception thrown by application: " + e.Message);
                Console.WriteLine("Automatically retrying; if it fails again, an exception is thrown");

                throw new ApplicationException(e.Message);
            }
        }
    }
}
