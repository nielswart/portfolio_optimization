// Copyright (c) 2013: DJ Swart, AJ Hoffman

//#define RDEP

using PortfolioEngine.Settings;

namespace PortfolioEngine.Portfolios
{
    public static class EfficientFrontier
    {
        public static IPortfolioCollection CalculateMVFrontier(this PortfolioCollection portfCol, IPortfolio samplePortf, uint numPortfolios)
        {
#if(RDEP)
            var mvfrontier = new MVOFrontier_R(samplePortf, numPortfolios);
            var portfCollection = mvfrontier.Calculate();
#else
            var mvfrontier = new MVOFrontier(samplePortf, numPortfolios);
            var portfCollection = mvfrontier.Calculate();
#endif
            portfCol.AddRange(portfCollection);

            return portfCol;
        }
    }
}
