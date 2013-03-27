// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using DataSciLib.DataStructures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace PortfolioEngine.Portfolios
{
    public interface IPortfolio : IList<IInstrument>
    {
        string Name { get; }
        double Mean { get; }
        double StdDev { get; }
        List<LinearConstraint> Constraints { get; }
        Dictionary<string, double> Covariance { get; set; }
        IInstrument this[string id] { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Portfolio : List<IInstrument>, IPortfolio
    {
        #region Properties
        public string Name 
        { get; protected set; }

        public List<LinearConstraint> Constraints 
        { 
            get; 
            private set; 
        }

        public double Mean
        {
            get;
            private set;
        }

        public double StdDev
        {
            get
            {
                if (Covariance.ContainsKey(this.Name))
                    return Math.Round(Math.Sqrt(Covariance[this.Name]),4);
                else
                    return double.NaN;
            }
        }

        public Dictionary<string, double> Covariance
        {
            get;
            set;
        }

        // Gets the element with the sepcified ID
        public IInstrument this[string id]
        {
            get { return this.Find((a) => a.ID == id); }
        }
        #endregion

        #region Constructors
        public Portfolio()
        {
            Name = "Portfolio";
            Covariance = new Dictionary<string, double>();
        }

        public Portfolio(string name)
        {
            Name = name;
            Covariance = new Dictionary<string, double>();
            Constraints = new List<LinearConstraint>();
        }

        public Portfolio(string name, double mean, double variance)
        {
            Name = name;
            Mean = mean;
            Covariance = new Dictionary<string, double>();
            Covariance[this.Name] = variance;
            Constraints = new List<LinearConstraint>();
        }

        #endregion

        public void SetWeight(string name, double weight)
        {
            var instrument = this[name].Weight = weight;
        }

        public void SetConstraints(IEnumerable<LinearConstraint> constraints)
        {
            Constraints.Clear();
            Constraints.AddRange(constraints);
        }

        public void AddAllInvestedConstraint()
        {
            var universe = from instrument in this
                           select instrument.ID;

            Constraints.Add(LinearConstraint.Create(universe, Relational.Equal, 1));
        }

        public void AddLongOnlyConstraint()
        {
            foreach (var instrument in this)
            {
                // No short positions allowed -> min = 0
                Constraints.Add(LinearConstraint.Create(instrument.ID, Relational.Larger, 0));
            }
        }

        public void AddBoxConstraints(HashSet<string> assets, double lowerlimit, double upperlimit)
        {
            // check if asset in portfolio
            foreach (var asset in assets)
            {
                Constraints.Add(LinearConstraint.Create(asset, Relational.Smaller, upperlimit));
                Constraints.Add(LinearConstraint.Create(asset, Relational.Larger, lowerlimit));
            }
        }

        public void AddBoxConstraint(string asset, double lowerlimit, double upperlimit)
        {
            // check if asset in portfolio
            Constraints.Add(LinearConstraint.Create(asset, Relational.Smaller, upperlimit));
            Constraints.Add(LinearConstraint.Create(asset, Relational.Larger, lowerlimit));
        }

        public void AddGroupConstraints(HashSet<string> assets, double lowerlimit, double upperlimit)
        {
            Constraints.Add(LinearConstraint.Create(assets, Relational.Smaller, upperlimit));
            Constraints.Add(LinearConstraint.Create(assets, Relational.Larger, lowerlimit));
        }
    }
}
