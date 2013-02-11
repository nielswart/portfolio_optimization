using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.DataStructures;

namespace PortfolioEngine
{    
    /// <summary>
    /// Represents the mean and variance-covariance of each variable (instrument)
    /// </summary>
    public class MeanVariance
    {
        public string Symbol { get; set; }
        public IEnumerable<ArrayItem<string, double>> Covariance { get; set; }
        public double Mean { get; set; }
    }
}
