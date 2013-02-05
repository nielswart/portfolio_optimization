using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioEngine
{
    public class ResultSet<T>
    {
        public Dictionary<Metrics, T> Metrics;

        public Dictionary<VMetrics, T[]> VectorMetrics;

        public Dictionary<MatrixMetrics, T[,]> MatrixMetrics;

        public int ID { get; private set; }

        public ResultSet(Dictionary<Metrics, T> metrics, Dictionary<VMetrics, T[]> vmetrics, 
            Dictionary<MatrixMetrics, T[,]> mmetrics)
        {
        }

        public ResultSet(int id)
        {
            this.ID = id;
            Metrics = new Dictionary<Metrics, T>();
            VectorMetrics = new Dictionary<VMetrics,T[]>();
            MatrixMetrics = new Dictionary<MatrixMetrics, T[,]>();
        }
    }
}
