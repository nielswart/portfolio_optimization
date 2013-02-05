// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System.Runtime.Serialization;
using DataSciLib.DataStructures;
using PortfolioEngine.Settings;
using System;
using System.Collections.Generic;
using PerformanceTools;

namespace PortfolioEngine.Portfolios
{
    /// <summary>
    /// Represents the total efficient frontier
    /// Implements IPortfolio interface
    /// </summary>
    [CollectionDataContract]
    public sealed class EfficientFrontier : PortfolioCollection
    {
        public EfficientFrontier()
        {
            
        }

        public static IPortfolioCollection CalculateMVFrontier(PortfolioSettings portfset, SortedList<string, double> mean, double[,] covariance)
        {
            var mvfrontier = new MVOFrontier(portfset, mean, covariance);
            mvfrontier.Calculate();

            var portfCollection = new PortfolioCollection();
            portfCollection.AddMetrics(mvfrontier.ResultsCollection);
            return portfCollection;
        }

        public static IPortfolioCollection CalculateMVFrontier(PortfolioSettings portfset, SortedList<string, double> mean, CovarianceMatrix covariance)
        {
            PerformanceLogger.Start("EfficientFrontier", "CalculateMVFrontier", "MVOFrontier.Calculate");
            var mvfrontier = new MVOFrontier(portfset, mean, covariance);
            mvfrontier.Calculate();
            PerformanceLogger.Stop("EfficientFrontier", "CalculateMVFrontier", "MVOFrontier.Calculate");

            PerformanceLogger.Start("EfficientFrontier", "CalculateMVFrontier", "PortfolioCollection.AddMetrics");
            var portfCollection = new PortfolioCollection();
            portfCollection.AddMetrics(mvfrontier.ResultsCollection);
            PerformanceLogger.Stop("EfficientFrontier", "CalculateMVFrontier", "PortfolioCollection.AddMetrics");
            return portfCollection;
        }
    }
}
