// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System;
using System.Linq;
using RDotNet;
using DataSciLib.DataStructures;

namespace DataSciLib.REngine.Rmetrics
{
    public static class fPortfolioExtensions
    {
        /// <summary>
        /// Get Portfolio Covariance Matrix
        /// </summary>
        /// <param name="portfolio">portfolio object</param>
        /// <returns></returns>
        public static double[,] GetCovarianceMatrix(this fPortfolio portfolio)
        {
            var expr = fPortfolio.Engine.CallFunction("getCov", portfolio.Expression);
            var mat = expr.AsNumericMatrix();

            int numrows = mat.RowCount;
            int numcols = mat.ColumnCount;

            double[,] cov = new double[numrows, numcols];
            mat.CopyTo(cov, numrows, numcols);

            return cov;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <returns></returns>
        public static double[,] GetCorrelationMatrix(this fPortfolio portfolio)
        {
            var data = fPortfolio.Engine.CallFunction("getSeries", portfolio.Expression);
            var expr = fPortfolio.Engine.CallFunction("cor", data);
            var mat = expr.AsNumericMatrix();

            int numrows = mat.RowCount;
            int numcols = mat.ColumnCount;

            double[,] cor = new double[numrows, numcols];
            mat.CopyTo(cor, numrows, numcols);

            return cor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="weights"></param>
        public static void GetWeights(this fPortfolio portfolio, out double[] weights)
        {
            // Get weights
            var expr = fPortfolio.Engine.CallFunction("getWeights", portfolio.Expression);

            var w = expr.AsNumeric();
            weights = w.ToArray();
        }

        /// <summary>
        /// Get Portfolio weights
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="weights"></param>
        public static void GetWeights(this fPortfolio portfolio, out double[,] weights)
        {
            // Get weights
            var expr = fPortfolio.Engine.CallFunction("getWeights", portfolio.Expression, "portfolio");

            var wmat = expr.AsNumericMatrix();
            int numrows = wmat.RowCount;
            int numcols = wmat.ColumnCount;

            weights = new double[numrows, numcols];
            wmat.CopyTo(weights, numrows, numcols);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="covrisk"></param>
        public static void GetCovRiskBudgets(this fPortfolio portfolio, out double[] covrisk)
        {
            var expr = fPortfolio.Engine.CallFunction("getCovRiskBudgets", portfolio.Expression);
            covrisk = expr.AsNumeric().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="covrisk"></param>
        public static void GetCovRiskBudgets(this fPortfolio portfolio, out double[,] covrisk)
        {
            var expr = fPortfolio.Engine.CallFunction("getCovRiskBudgets", portfolio.Expression);
            var crmat = expr.AsNumericMatrix();
            int numrows = crmat.RowCount;
            int numcols = crmat.ColumnCount;

            covrisk = new double[numrows, numcols];
            crmat.CopyTo(covrisk, numrows, numcols);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="mean"></param>
        public static void GetMeanReturns(this fPortfolio portfolio, out double[] mean)
        {
            // Get returns
            var expr = fPortfolio.Engine.CallFunction("getMean", portfolio.Expression);
            var ret = expr.AsNumeric().ToArray();
            mean = ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="mean"></param>
        public static void GetPortfolioMeanReturns(this fPortfolio portfolio, out double[,] mean)
        {
            // Get returns
            var expr = fPortfolio.Engine.CallFunction("getTargetReturn", portfolio.Expression, "portfolio");
            var meanmat = expr.AsNumericMatrix();

            int numrows = meanmat.RowCount;
            int numcols = meanmat.ColumnCount;

            mean = new double[numrows, numcols];
            meanmat.CopyTo(mean, numrows, numcols);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="timeseries"></param>
        public static void GetData(this fPortfolio portfolio, out double[,] timeseries, out string[] timevector)
        {
            var expr = fPortfolio.Engine.CallFunction("getSeries", portfolio.Expression);
            var timeexpr = fPortfolio.Engine.CallFunction("time", expr);
            var timestring = fPortfolio.Engine.CallFunction("as.character", timeexpr);

            var mat = expr.AsNumericMatrix();
            var time = timestring.AsCharacter();
            timevector = time.ToArray<string>();

            int numrows = mat.RowCount;
            int numcols = mat.ColumnCount;

            timeseries = new double[numrows, numcols];
            mat.CopyTo(timeseries, numrows, numcols);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="timeseries"></param>
        public static ITimeSeries<double> GetTimeSeries(this fPortfolio portfolio)
        {
            string[] timevector;
            double[,] timeseries;

            var expr = fPortfolio.Engine.CallFunction("getSeries", portfolio.Expression);
            var timeexpr = fPortfolio.Engine.CallFunction("time", expr);
            var timestring = fPortfolio.Engine.CallFunction("as.character", timeexpr);

            var mat = expr.AsNumericMatrix();
            var time = timestring.AsCharacter();
            timevector = time.ToArray<string>();

            int numrows = mat.RowCount;
            int numcols = mat.ColumnCount;

            timeseries = new double[numrows, numcols];
            mat.CopyTo(timeseries, numrows, numcols);

            return TimeSeriesFactory<double>.Create(timeseries, timevector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="risk"></param>
        public static void GetPortfolioRisk(this fPortfolio portfolio, out double[,] risk)
        {
            // Get returns
            var expr = fPortfolio.Engine.CallFunction("getTargetRisk", portfolio.Expression, "portfolio");
            var riskmat = expr.AsNumericMatrix();

            int numrows = riskmat.RowCount;
            int numcols = riskmat.ColumnCount;

            risk = new double[numrows, numcols];
            riskmat.CopyTo(risk, numrows, numcols);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="cvar"></param>
        public static void CalculatePortfolioCVaR(this fPortfolio portfolio, out double cvar)
        {
            var data = fPortfolio.Engine.CallFunction("getSeries", portfolio.Expression);
            var weights = fPortfolio.Engine.CallFunction("getWeights", portfolio.Expression);
            var expr = fPortfolio.Engine.CallFunction("cvarRisk", data, weights);

            cvar = expr.AsNumeric().ToArray().First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="var"></param>
        public static void CalculatePortfolioVaR(this fPortfolio portfolio, out double var)
        {
            var data = fPortfolio.Engine.CallFunction("getSeries", portfolio.Expression);
            var weights = fPortfolio.Engine.CallFunction("getWeights", portfolio.Expression);
            var expr = fPortfolio.Engine.CallFunction("varRisk", data, weights);

            var = expr.AsNumeric().ToArray().First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="stddev"></param>
        public static void CalculatePortfolioStdDev(this fPortfolio portfolio, out double stddev)
        {
            var data = fPortfolio.Engine.CallFunction("getSeries", portfolio.Expression);
            var weights = fPortfolio.Engine.CallFunction("getWeights", portfolio.Expression);
            var expr = fPortfolio.Engine.CallFunction("varRisk", data, weights);

            stddev = expr.AsNumeric().ToArray().First();
        }
    }

}
