// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System.Collections.Generic;
using System.Data;
using DataSciLib.DataStructures;

namespace PortfolioEngine.Portfolios
{

    public interface IPortfolioCollection : IList<IPortfolio>
    {
        string Name { get; }

        double RiskFreeRate { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PortfolioCollection : List<IPortfolio>, IPortfolioCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        public double RiskFreeRate { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PortfolioCollection()
        {
            
        }

        public PortfolioCollection(string name)
        {
            this.Name = name;
        }

        public PortfolioCollection(string name, double riskFreeRate)
        {
            this.Name = name;
            this.RiskFreeRate = riskFreeRate;
        }
    }
}
