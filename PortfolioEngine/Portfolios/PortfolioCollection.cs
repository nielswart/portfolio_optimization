// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System.Collections.Generic;
using System.Data;
using DataSciLib.DataStructures;

namespace PortfolioEngine.Portfolios
{
    /// <summary>
    /// 
    /// </summary>
    public class PortfolioCollection : List<IPortfolio>, IPortfolioCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; protected set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PortfolioCollection()
        {
            
        }

        public PortfolioCollection(string id)
        {
            this.ID = id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultSetCollection"></param>
        public void AddMetrics(Dictionary<int, ResultSet<double>> resultSetCollection)
        {
            int c = 0;
            foreach (var r in resultSetCollection)
            {
                this.Add(PortfolioFactory.Create(r.Value, c));
                c++;
            }
        }
    }
}
