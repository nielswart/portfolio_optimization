// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using DataSciLib.Statistics;
using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.PerformanceAnalytics;
using DataSciLib.REngine.Rmetrics;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PortfolioEngine.Portfolios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioEngine
{
    /// <summary>
    /// 
    /// </summary>
    public struct CAPMCoefficients
    {
        public readonly double Alpha;
        public readonly double Beta;

        public CAPMCoefficients(double alpha, double beta)
        {
            this.Alpha = alpha;
            this.Beta = beta;
        }
    }

    public struct NamedValue
    {
        public readonly string Name;
        public readonly double Value;

        public NamedValue(string name, double value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class Analytics
    {
        #region Initialization
        /// <summary>
        /// R Engine Instance
        /// </summary>
        public static R Engine;

        public static string WarningMessage { get; private set; }

        private static void Initialize()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);

            WarningMessage = "No warnings";
        }

        #endregion

        /// <summary>
        /// Calculates the annualised return of the given time series
        /// </summary>
        /// <param name="timeseries">A time series of returns</param>
        /// <returns>Annualised return in the same units as the <paramref name="timeseries"/> parameter </returns>
        public static double AnnualisedReturn(ITimeSeries<double> timeseries)
        {
            return Math.Round(timeseries.AnnualisedMean(),3);
        }

        /// <summary>
        /// Calculates the anualised standard deviation of the given time series
        /// </summary>
        /// <param name="timeseries">A time series of returns</param>
        /// <returns>Annualised return in the same units as the <paramref name="timeseries"/> parameter</returns>
        public static double AnnualisedStdDev(ITimeSeries<double> timeseries)
        {
            return Math.Round(timeseries.AnnualisedStdDev(), 3);
        }

        /// <summary>
        /// Returns the regression coefficients and the residual vector of the linear CAPM regresion model
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree"></param>
        /// <returns>Tuple with the following elements - alpha, beta and residual vector of the same size as the asset and benchmark time series</returns>
        public static CAPMCoefficients CAPModel(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            var Ra = asset.AsTimeSeries() - riskfree;
            var Rb = benchmark.AsTimeSeries() - riskfree;
            
            var capm = Model.LinearRegression(Ra.Data, Rb.Data);
            return new CAPMCoefficients(Math.Round(capm.Intercept,3), Math.Round(capm.Coefficients.First(), 3));
        }

        /// <summary>
        /// Calculate the annualised realised alpha from a CAPM linear regression model
        /// </summary>
        /// <param name="asset">The asset time series to use in calculating alpha</param>
        /// <param name="benchmark">The benchmark index/portfolio to calculate alpha against</param>
        /// <param name="riskfree">The annualised risk-free rate with the same periodicity as the returns</param>
        /// <returns>Array of alphas, one for each series in asset time series</returns>
        public static double RealisedAlpha(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            // Find a better way of comparing two series for equivalence - time and length (maybe extract part of benchmark that is comparable to asset ??
            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            return Math.Round(CAPModel(asset, benchmark, riskfree).Alpha, 3);
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

            // Find a better way of comparing two series for equivalence - time and length (maybe extract part of benchmark that is comparable to asset ??
            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("Time series should be of the same length");

            return Math.Round(CAPModel(asset, benchmark, riskfree).Beta, 3);
        }

        public static double ResidualRisk(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            var Ra = asset.AsTimeSeries() - riskfree;
            var Rb = benchmark.AsTimeSeries() - riskfree;

            return Math.Round(Statistics.StandardDeviation(Model.LinearRegression(Ra.Data, Rb.Data).Residuals), 3);
        }

        public static double SharpeRatio(ITimeSeries<double> portfolio, double riskFreeRate = 0)
        {
            // Check parameters
            if (riskFreeRate >= 1 || riskFreeRate < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            return Math.Round((portfolio.AnnualisedMean() - riskFreeRate) / portfolio.AnnualisedStdDev(), 3);
        }

        public static double SharpeRatio(IPortfolio portfolio, double riskFreeRate = 0)
        {
            if (riskFreeRate >= 1 || riskFreeRate < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            return Math.Round((portfolio.Mean - riskFreeRate) / Math.Sqrt(portfolio.Covariance[portfolio.Name]), 3);
        }

        public static double ActivePremium(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            return Math.Round(portfolio.AnnualisedMean() - benchmark.AnnualisedMean(), 3);
        }

        public static double TrackingError(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            return Math.Round((portfolio.AsTimeSeries() - benchmark.AsTimeSeries()).AnnualisedStdDev(), 3);
        }

        public static double InformationRatio(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            if (portfolio.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            return Math.Round(Analytics.ActivePremium(portfolio, benchmark) / Analytics.TrackingError(portfolio, benchmark), 3);
        }

        public static double VaR(ITimeSeries<double> portfolio, double percentile = 0.95, VaRMethod vartype = PortfolioEngine.VaRMethod.CornishFisher)
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

            return PerformanceRatios.SortinoRatio(timeSeries.Create(portfolio), timeSeries.Create(targetReturn)).First();
        }

        public static IEnumerable<NamedValue> MarginalReturn(IPortfolio portfolio)
        {
            var marr = from p in portfolio
                       select new NamedValue(p.Name, p.Weight / portfolio.Mean);

            return marr;
        }

        public static IEnumerable<NamedValue> MarginalRisk(IPortfolio portfolio)
        {
            // Marginal contribution to risk --> d/dw (sigma) = (cov*w)/sigma

            var marr = from ins in portfolio
                       select new Tuple<string, Dictionary<string, double>>(ins.Name, ins.Covariance);

            var cov = CovarianceMatrix.Create(marr.ToDictionary<Tuple<string, Dictionary<string, double>>, string, Dictionary<string, double>>(x => x.Item1, 
                y => y.Item2));

            var weights = new DenseVector((from ins in portfolio
                                           select ins.Weight).ToArray());

            var sigma = Math.Sqrt(weights.ToRowMatrix().Multiply(cov).Multiply(weights).First());
            var mcr = cov.Multiply(weights).Divide(sigma);

            return null;
        }
    }
}
