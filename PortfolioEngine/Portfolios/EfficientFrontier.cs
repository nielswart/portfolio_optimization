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
        public static IPortfolioCollection CalculateMVFrontier(this PortfolioCollection portfCol, IPortfolio samplePortf, uint numPortfolios)
        {
            var mvfrontier = new MVOFrontier(samplePortf, numPortfolios);
            var portfCollection = mvfrontier.Calculate();

            portfCol.AddRange(portfCollection);

            return portfCol;
        }
    }
}
