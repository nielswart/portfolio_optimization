// Copyright (c) 2012: DJ Swart, AJ Hoffman

using PortfolioEngine.Settings;

namespace PortfolioEngine.Portfolios
{
    public static class MinRiskPortfolio
    {
        public static IPortfolio CalculateMinVariancePortfolio(this IPortfolio portfolio)
        {
            var minvar = new MVOMinVariance(portfolio);
            var portf = minvar.Calculate();
            return portf;
        }
    }
}
