// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

// Preprocessor directives
#define RDEP

using DataSciLib.DataStructures;

#if RDEP
using DataSciLib.REngine;
using DataSciLib.REngine.PerformanceAnalytics;
using DataSciLib.REngine.Rmetrics;
#endif

using DataSciLib.Statistics;
using MathNet.Numerics.Statistics;
using PortfolioEngine.Portfolios;
using System;
using System.Linq;

namespace PortfolioEngine
{
    /// <remarks>
    /// Structure for the coefficients of the CAPM linear regression model
    /// </remarks>
    public struct CAPMCoefficients
    {
        /// <summary>
        /// CAPM Alpha - linear regression intercept
        /// </summary>
        public readonly double Alpha;

        /// <summary>
        /// CAPM Beta - linear regression slope
        /// </summary>
        public readonly double Beta;

        /// <summary>
        /// Initializes a new instance of the <typeparamref name="PortfolioEngine.CAPMCoefficients"/> value type with the given parameter value
        /// </summary>
        /// <param name="alpha">The linear regression intercept</param>
        /// <param name="beta">The linear regression slope</param>
        public CAPMCoefficients(double alpha, double beta)
        {
            this.Alpha = alpha;
            this.Beta = beta;
        }
    }

    /// <remarks>
    /// API for various asset and portfolio analytics and risk and return metrics
    /// </remarks>
    public sealed class PortfolioAnalytics
    {

#if(RDEP)
        #region Initialization
        /// <summary>
        /// R Engine Instance
        /// </summary>
        public static R Engine;

        private static void Initialize()
        {
            Engine = R.Instance;
            if (!Engine.IsRunning)
                Engine.Start(REngineOptions.QuietMode);
        }

        #endregion
#endif
        public static int DecimalDigits = 3;
        
        /// <summary>
        /// Calculates the annualised return of the given time series
        /// </summary>
        /// <param name="timeseries">A time series of returns</param>
        /// <returns>Annualised return</returns>
        public static double AnnualisedReturn(ITimeSeries<double> timeseries)
        {
            return Math.Round(timeseries.AnnualisedMean(),DecimalDigits);
        }

        /// <summary>
        /// Calculates the anualised standard deviation of the given time series
        /// </summary>
        /// <param name="timeseries">A time series of returns</param>
        /// <returns>Annualised standard deviation</returns>
        public static double AnnualisedStdDev(ITimeSeries<double> timeseries)
        {
            return Math.Round(timeseries.AnnualisedStdDev(), DecimalDigits);
        }

        /// <summary>
        /// Calculates the regression coefficients of the linear CAPM regresion model
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree">Risk-free rate of return specified as a fractional value, e.g. 0.05 for 5%</param>
        /// <returns>The CAPM coefficients - Alpha and Beta</returns>
        public static CAPMCoefficients CAPModel(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            var Ra = asset.AsTimeSeries() - riskfree;
            var Rb = benchmark.AsTimeSeries() - riskfree;
            
            var capm = Model.LinearRegression(Ra.Data, Rb.Data);
            return new CAPMCoefficients(Math.Round(capm.Intercept,DecimalDigits), Math.Round(capm.Coefficients.First(), DecimalDigits));
        }

        /// <summary>
        /// Calculate the annualised realised alpha from a CAPM linear regression model
        /// </summary>
        /// <param name="asset">The asset time series to use in calculating alpha</param>
        /// <param name="benchmark">The benchmark index/portfolio to calculate alpha against</param>
        /// <param name="riskfree">Risk-free rate of return specified as a fractional value, e.g. 0.05 for 5%</param>
        /// <returns>Array of alphas, one for each series in asset time series</returns>
        public static double RealisedAlpha(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            // Find a better way of comparing two series for equivalence - time and length (maybe extract part of benchmark that is comparable to asset ??
            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            return Math.Round(CAPModel(asset, benchmark, riskfree).Alpha, DecimalDigits);
        }

        /// <summary>
        /// Calculate the realised beta (ex-post)
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="benchmark"></param>
        /// <param name="riskfree">Risk-free rate of return specified as a fractional value, e.g. 0.05 for 5%</param>
        /// <returns>Realised beta</returns>
        public static double RealisedBeta(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            // Check parameters
            if (riskfree >= 1 || riskfree < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            // Find a better way of comparing two series for equivalence - time and length (maybe extract part of benchmark that is comparable to asset ??
            if (asset.RowCount != benchmark.RowCount)
                throw new ArgumentException("Time series should be of the same length");

            return Math.Round(CAPModel(asset, benchmark, riskfree).Beta, DecimalDigits);
        }

        /// <summary>
        /// Calculates the residual risk (annualised standard deviation of the error term from the CAPM linear regession)
        /// </summary>
        /// <param name="asset">Asset time series</param>
        /// <param name="benchmark">Benchmark time series</param>
        /// <param name="riskfree">Risk-free rate of return specified as a fractional value, e.g. 0.05 for 5%</param>
        /// <returns>Returns the standard deviation</returns>
        public static double ResidualRisk(ITimeSeries<double> asset, ITimeSeries<double> benchmark, double riskfree = 0)
        {
            var Ra = asset.AsTimeSeries() - riskfree;
            var Rb = benchmark.AsTimeSeries() - riskfree;

            return Math.Round(Statistics.StandardDeviation(Model.LinearRegression(Ra.Data, Rb.Data).Residuals)*Math.Sqrt(asset.PeriodsInYear()), DecimalDigits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="riskFreeRate">Risk-free rate of return specified as a fractional value, e.g. 0.05 for 5%</param>
        /// <returns></returns>
        public static double SharpeRatio(ITimeSeries<double> portfolio, double riskFreeRate = 0)
        {
            // Check parameters
            if (riskFreeRate >= 1 || riskFreeRate < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            return Math.Round((portfolio.AnnualisedMean() - riskFreeRate) / portfolio.AnnualisedStdDev(), DecimalDigits);
        }

        /// <summary>
        /// Calculates the portolio Sharpe ratio, i.e. the mean return of the portfolio above that of the risk-free rate divided by the portfolio's risk (standard deviation)
        /// </summary>
        /// <param name="portfolio">Portfolio definition</param>
        /// <param name="riskFreeRate">Risk-free rate of return specified as a fractional value, e.g. 0.05 for 5%</param>
        /// <returns></returns>
        public static double SharpeRatio(IPortfolio portfolio, double riskFreeRate = 0)
        {
            if (riskFreeRate >= 1 || riskFreeRate < 0)
                throw new ArgumentException("The risk-free rate should be a value between 0 and 1");

            return Math.Round((portfolio.Mean - riskFreeRate) / Math.Sqrt(portfolio.Covariance[portfolio.Name]), DecimalDigits);
        }

        /// <summary>
        /// Calculates the portfolio active premium, i.e. the difference between the annualised mean of the portfolio and the benchmark return
        /// </summary>
        /// <param name="portfolio">Portfolio time series</param>
        /// <param name="benchmark">Benchmark time series</param>
        /// <returns>Annualised active premium</returns>
        public static double ActivePremium(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            return Math.Round(portfolio.AnnualisedMean() - benchmark.AnnualisedMean(), DecimalDigits);
        }

        /// <summary>
        /// Calculates the portfolio tracking error, i.e. the annualised standard deviation of the difference in returns between the portfolio and the benchmark
        /// </summary>
        /// <param name="portfolio">Portfolio time series</param>
        /// <param name="benchmark">Benchmark time series</param>
        /// <returns></returns>
        public static double TrackingError(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            return Math.Round((portfolio.AsTimeSeries() - benchmark.AsTimeSeries()).AnnualisedStdDev(), DecimalDigits);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="benchmark"></param>
        /// <returns></returns>
        public static double InformationRatio(ITimeSeries<double> portfolio, ITimeSeries<double> benchmark)
        {
            if (portfolio.RowCount != benchmark.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            return Math.Round(PortfolioAnalytics.ActivePremium(portfolio, benchmark) / PortfolioAnalytics.TrackingError(portfolio, benchmark), DecimalDigits);
        }

#if(Version_2)
        // Additional Performance Metrics - Bugs - Still needs some work
        public static Dictionary<string, double> MarginalReturn(IPortfolio portfolio)
        {
            var marr = new Dictionary<string,double>();
            foreach (var p in portfolio)
            {
                marr.Add(p.ID, p.Weight / portfolio.Mean);
            }

            return marr;
        }

        public static Dictionary<string, double> MarginalRisk(IPortfolio portfolio)
        {
            // Marginal contribution to risk --> d/dw (sigma) = (cov*w)/sigma

            var marr = from ins in portfolio
                       select new Tuple<string, Dictionary<string, double>>(ins.ID, ins.Covariance);

            var cov = CovarianceMatrix.Create(marr.ToDictionary(x => x.Item1, y => y.Item2));

            var weights = new DenseVector((from ins in portfolio
                                           select ins.Weight).ToArray());

            var sigma = Math.Sqrt(weights.ToRowMatrix().Multiply(cov).Multiply(weights).First());
            var mcr = cov.Multiply(weights).Divide(sigma);

            return null;
        }
#endif

        // REngine dependent methods - Slower than others
#if(RDEP)
        public static double VaR(ITimeSeries<double> portfolio, double percentile = 0.95, VaRMethod vartype = PortfolioEngine.VaRMethod.CornishFisher)
        {
            Initialize();
            if (percentile > 1)
                percentile = percentile / 100;
            if (percentile <= 0 || percentile > 1)
                throw new ArgumentException("Percentile value should be between 0 and 1 or 0 and 100");

            switch (vartype)
            {
                case PortfolioEngine.VaRMethod.Gaussian:
                    return Math.Round(RiskMetrics.GaussianVaR(timeSeries.Create(portfolio), percentile).First(), DecimalDigits);

                case PortfolioEngine.VaRMethod.Historical:
                    return Math.Round(RiskMetrics.HistoricalVaR(timeSeries.Create(portfolio), percentile).First(), DecimalDigits);

                case PortfolioEngine.VaRMethod.CornishFisher:
                    return Math.Round(RiskMetrics.CornishFisherVaR(timeSeries.Create(portfolio), percentile).First(), DecimalDigits);

                default:
                    return Math.Round(RiskMetrics.GaussianVaR(timeSeries.Create(portfolio), percentile).First(), DecimalDigits);
            }
        }

        public static double SortinoRatio(ITimeSeries<double> portfolio, ITimeSeries<double> targetReturn)
        {
            Initialize();

            if (portfolio.RowCount != targetReturn.RowCount)
                throw new ArgumentException("TimeSeries should be of the same length");

            return Math.Round(PerformanceRatios.SortinoRatio(timeSeries.Create(portfolio), timeSeries.Create(targetReturn)).First(), DecimalDigits);
        }
#endif
    }
}
