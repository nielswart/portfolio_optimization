// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using DataSciLib.DataStructures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PortfolioEngine.Portfolios
{
    /// <summary>
    /// Represents the generic portfolio with specific targets for either the weights or the risk and return, therefore any feasible portfolio
    /// - Is a base class for many of the special portfolio classes
    /// - Implements the IPortfolio interface
    /// </summary>
    [DataContract]
    public abstract class Portfolio : IPortfolio
    {
        
        #region Properties

        public ITimeSeries<double> PriceSeries 
        {
            get; 
            set; 
        }

        public IEnumerable<ArrayItem<string, double>> Weights { get; set; }

        public Dictionary<VMetrics, double[]> VectorMetrics { get; set; }

        public Dictionary<Metrics, double> Metrics { get; set; }

        public Dictionary<MatrixMetrics, double[,]> MatrixMetrics { get; set; }

        public string Name { get; protected set; }
        #endregion

        public Portfolio()
        {
            PriceSeries = new TimeSeries<double>();
            VectorMetrics = new Dictionary<VMetrics,double[]>();
            Metrics = new Dictionary<Metrics,double>();
            Name = "Portfolio";
        }

        public Portfolio(string id)
        {
            PriceSeries = new TimeSeries<double>();
            VectorMetrics = new Dictionary<VMetrics, double[]>();
            Metrics = new Dictionary<Metrics, double>();
            Name = id;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual IPortfolio Calculate(ITimeSeries<double> data)
        {
            throw new NotImplementedException();
        }        
    }
}
