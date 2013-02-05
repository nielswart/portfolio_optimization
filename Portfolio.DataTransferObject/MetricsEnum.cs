// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioEngine
{
    /// <summary>
    /// 
    /// </summary>
    public enum Metrics
    {
        Mean,
        StdDev,
        Variance,
        HistoricalVaR,
        CornishFisherVaR,
        GaussianVaR,
        CVaR
    }

    /// <summary>
    /// 
    /// </summary>
    public enum VMetrics
    {
        MarginalRisk,
        MarginalReturn,
        Weights,
        MeanReturns
    }

    public enum MatrixMetrics
    {
        CorrelationMatrix
    }
}
