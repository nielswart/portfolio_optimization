// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using DataSciLib.DataStructures;
using DataSciLib.REngine;
using PortfolioEngine.Portfolios;
using PortfolioEngine.Settings;
using System;
using System.Collections.Generic;

namespace PortfolioEngine
{
    /// <summary>
    /// Portfolio Manager class - API for interacting with the PortfolioEngine library
    /// </summary>
    public sealed class Optimization
    {
        /// <summary>
        /// 
        /// </summary>
        public static R Engine;

        /// <summary>
        /// 
        /// </summary>
        public string WarningMessage { get; private set; }

        /// <summary>
        /// Creates an instance of the R Engine,
        /// Loads the required R packages for portfolio optimization,
        /// Set the working directory to the path specified in the App.config file - key: "workingdir",
        /// Load the Workspace (.RData) file located in the working directory,
        /// Sets the dataset name to the value specified in the App.config file - key: "DataSetName",
        /// </summary>
        public Optimization()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
            WarningMessage = "No warnings";
        }

        /// <summary>
        /// Use this constructor if you manage the creation and destruction of the R Engine,
        /// Loads the required R packages for portfolio optimization,
        /// Set the working directory to the path specified in the App.config file - key: "workingdir",
        /// Load the Workspace (.RData) file located in the working directory,
        /// Sets the dataset name to the value specified in the App.config file - key: "DataSetName",
        /// </summary>
        /// <param name="_engine">Instance of the R Engine</param>
        public Optimization(R _engine)
        {
            Engine = _engine;
            Engine.Start(REngineOptions.QuietMode);
            WarningMessage = "No warnings";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portfset"></param>
        /// <param name="meanvector"></param>
        /// <param name="covariance"></param>
        /// <returns></returns>
        public IPortfolioCollection CalcEfficientFrontier(PortfolioSettings portfset, SortedList<string, double> meanvector, double[,] covariance)
        {
            return EfficientFrontier.CalculateMVFrontier(portfset, meanvector, covariance);
        }

        public IPortfolioCollection CalcEfficientFrontier(PortfolioSettings portfset, SortedList<string, double> meanvector, CovarianceMatrix covariance)
        {
            return EfficientFrontier.CalculateMVFrontier(portfset, meanvector, covariance);
        }

        public IPortfolio CalcPortfolio(PortfolioSettings portfset, ITimeSeries<double> data)
        {
            throw new NotImplementedException();
        }

        public IPortfolio CalcPortfolio(ITimeSeries<double> data, double[] weights)
        {
            return PortfolioFactory.Create(data, weights);
        }
    }
}

        