using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.DataStructures;

namespace PortfolioEngine.Portfolios
{
    public enum Rebalance
    {
        Never,
        Continuous,
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        SemiAnnually,
        Annually
    }

    public class PortfolioFactory
    {
        public static IPortfolio Create(string ID)
        {
            //GeneralTargetPortfolio portf = new GeneralTargetPortfolio(
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static IPortfolio Create(ITimeSeries<double> data, double[] weights, Rebalance rebalance = Rebalance.Never)
        {
            var gentp = new GeneralPortfolio();

            if (data.ColumnCount != weights.Length)
                throw new ArgumentException("Number of elements in weight vector does not match the number of timeseries");

            switch (rebalance)
            {
                case Rebalance.Never:
                    {
                        /*
                        foreach (var ts in data)
                        {

                        }
                         * */
                    }
                    break;
            }
            // Calculate PortfolioValue timeseries
            return gentp;
        }

        public static IPortfolio Create(ITimeSeries<double> data, IEnumerable<double> weights, DateTime startdate, DateTime enddate, Rebalance rebalance = Rebalance.Never)
        {
            throw new NotImplementedException();
        }

        public static IPortfolio Create(PortfolioSettings portfset, ITimeSeries<double> data)
        {
            throw new NotImplementedException();
        }

        public static IPortfolio Create(ResultSet<double> results)
        {
            var gentp = new GeneralPortfolio();

            gentp.Metrics = results.Metrics;
            gentp.VectorMetrics = results.VectorMetrics;
            gentp.Weights = results.VectorMetrics[VMetrics.Weights];
            gentp.MatrixMetrics = results.MatrixMetrics;

            // Calculate PortfolioValue timeseries
            return gentp;
        }

        public static IPortfolio Create(ResultSet<double> results, int id)
        {
            var gentp = new GeneralPortfolio(id.ToString());

            gentp.Metrics = results.Metrics;
            gentp.VectorMetrics = results.VectorMetrics;
            gentp.Weights = results.VectorMetrics[VMetrics.Weights];
            gentp.MatrixMetrics = results.MatrixMetrics;

            return gentp;
        }
    }
}
