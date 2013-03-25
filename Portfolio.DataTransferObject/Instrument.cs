using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioEngine
{
    public interface IInstrument
    {
        /// <summary>
        /// 
        /// </summary>
        string ID { get; }

        double Weight { get; set; }

        double Mean { get; }

        Dictionary<string, double> Covariance { get; }
    }

    public class Instrument : IInstrument
    {
        #region Properties
        public string ID { get; private set; }

        public double Weight { get; set; }

        public double Mean
        {
            get;
            private set;
        }

        public Dictionary<string, double> Covariance
        {
            get;
            private set;
        }
        #endregion

        public Instrument(string name, double mean, Dictionary<string, double> cov)
        {
            this.ID = name;
            this.Mean = mean;
            this.Covariance = cov;
        }

        public Instrument(string name, double mean, Dictionary<string, double> cov, double weight)
        {
            this.ID = name;
            this.Mean = mean;
            this.Covariance = cov;
            this.Weight = weight;
        }
    }
}
