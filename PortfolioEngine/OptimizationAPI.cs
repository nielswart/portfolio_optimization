// Copyright (c) 2013: DJ Swart, AJ Hoffman
//

//#define RDEP

using DataSciLib.REngine;
using PortfolioEngine.Portfolios;

namespace PortfolioEngine
{
    /// <summary>
    /// API for calculating various Markowitz Mean-variance portfolios using constraint optimization
    /// </summary>
    public sealed class PortfolioOptimizer
    {
#if RDEP
        /// <summary>
        /// Optimization Engine Instance
        /// </summary>
        public static R Engine;

        private static bool _initialized = false;
        private static void _initialize()
        {
            Engine = R.Instance;
            if (!Engine.IsRunning)
                Engine.Start(REngineOptions.QuietMode);

            _initialized = true;
        }

        /// <summary>
        /// Initialize the PortfolioOptimizer optimization engine
        /// </summary>
        public static void Initialize()
        {
            _initialize();
        }
#endif

        /// <summary>
        /// Calculate the Efficient Frontier and Minimum Variance locus using the universe of assets and constraints defined in <paramref name="portfolio"/>
        /// </summary>
        /// <param name="portfolio">The portfolio definition</param>
        /// <param name="riskFreeRate">The risk-free rate specified as a fractional decimal value e.g. 0.05 for 5%</param>
        /// <param name="numberOfPortfolios">The number of portfolios in the efficient frontier locus</param>
        /// <returns>A collection of portfolios</returns>
        public static IPortfolioCollection CalcEfficientFrontier(IPortfolio portfolio, double riskFreeRate, uint numberOfPortfolios)
        {
#if RDEP
            if(!_initialized)
                Initialize();
#endif
            PortfolioCollection frontier = new PortfolioCollection("Efficient Frontier", riskFreeRate);
            return frontier.CalculateMVFrontier(portfolio, numberOfPortfolios);
        }

        /// <summary>
        /// Calculate the Global Minimum Variance Portfolio using the universe of assets and constraints defined in <paramref name="portfolio"/>
        /// </summary>
        /// <param name="portfolio">The portfolio definition</param>
        /// <param name="riskFreeRate">The risk-free rate specified as a fractional decimal value e.g. 0.05 for 5%</param>
        /// <returns>The Global Minimum Variance portfolio as an instance that implements <typeparamref name="IPortfolio"/></returns>
        public static IPortfolio CalcMinimumVariance(IPortfolio portfolio, double riskFreeRate)
        {
 #if RDEP
            if(!_initialized)
                Initialize();
#endif
            return portfolio.CalculateMinVariancePortfolio();
        }
    }
}

        