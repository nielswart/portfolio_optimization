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
        string Name { get; }

        double Weight { get; set; }

        double Mean { get; }

        Dictionary<string, double> Covariance { get; }
    }

    public class Instrument : IInstrument
    {
        #region Properties
        public string Name { get; private set; }

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

        public Instrument(string name)
        {

        }

        public Instrument(string name, double weight)
        {

        }
    }
}
