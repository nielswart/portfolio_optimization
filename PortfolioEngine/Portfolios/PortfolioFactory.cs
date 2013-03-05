using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.REngine;
using DataSciLib.DataStructures;

namespace PortfolioEngine.Portfolios
{
    public class PortfolioFactory
    {
        public static IPortfolio Create(IPortfolio samplePortfolio, string name, double mean, double variance)
        {
            Portfolio portf = new Portfolio(name, mean, variance);

            portf.AddRange(samplePortfolio);
            portf.SetConstraints(samplePortfolio.Constraints);
            return portf;
        }
    }

    public static class PortfolioTimeSeriesExtensions
    {

    }
}
