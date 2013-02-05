// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.DataStructures;

namespace PortfolioEngine
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInstrument
    {
        /// <summary>
        /// 
        /// </summary>
        string ID { get; }

        ITimeSeries<double> PriceSeries { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPortfolio : IInstrument
    {
        double[] Weights { get; }
        
        Dictionary<VMetrics, double[]> VectorMetrics { get; }

        Dictionary<Metrics, double> Metrics { get; }

        Dictionary<MatrixMetrics, double[,]> MatrixMetrics { get; }

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPortfolioCollection : IList<IPortfolio>
    {
        string ID { get; }
    }
}
