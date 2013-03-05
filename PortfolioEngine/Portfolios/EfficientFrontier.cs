// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System.Runtime.Serialization;
using DataSciLib.DataStructures;
using PortfolioEngine.Settings;
using System;
using System.Collections.Generic;
using PerformanceTools;

namespace PortfolioEngine.Portfolios
{
    public static class EfficientFrontier
    {
        public static IPortfolioCollection CalculateMVFrontier(this IPortfolioCollection portfCol,  IPortfolio samplePortf, uint numPortfolios)
        {
            //PerformanceLogger.Start("EfficientFrontier", "CalculateMVFrontier", "MVOFrontier.Calculate");
            var mvfrontier = new MVOFrontier(samplePortf, numPortfolios);
            var portfCollection = mvfrontier.Calculate();
            //PerformanceLogger.Stop("EfficientFrontier", "CalculateMVFrontier", "MVOFrontier.Calculate");

            return (IPortfolioCollection)portfCollection;
        }
    }
}
