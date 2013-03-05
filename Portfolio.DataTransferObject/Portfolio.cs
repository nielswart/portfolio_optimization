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

        List<LinearConstraint> Constraints { get; }

        double Mean { get; }

        Dictionary<string, double> Covariance { get; set; }

        void SetWeight(string code, double weight);
    }

    /// <summary>
    /// Represents the generic portfolio with specific targets for either the weights or the risk and return, therefore any feasible portfolio
    /// - Is a base class for many of the special portfolio classes
    /// - Implements the IPortfolio interface
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

        public Dictionary<string, double> Covariance
        {
            get;
            set;
        }

        #endregion

        LinearConstraint _constraintHandle;

        #region Constructors
        public Portfolio()
        {
            Name = "Portfolio";
            _constraintHandle = new LinearConstraint();
        }

        public Portfolio(string name)
        {
            Name = name;
        }

        public Portfolio(string name, double mean, double variance)
        {
            Name = name;
            Mean = mean;
            Covariance[this.Name] = variance;
        }

        #endregion

        public void SetWeight(string name, double weight)
        {
            var instrument = from ins in this
                             where ins.Name == name
                             select ins;

            instrument.First().Weight = weight;
        }

        public void SetConstraints(IEnumerable<LinearConstraint> constraints)
        {
            Constraints.Clear();
            Constraints.AddRange(constraints);
        }

        public void AddAllInvestedConstraint()
        {
            var universe = from instrument in this
                           select instrument.Name;

            Constraints.Add(LinearConstraint.Create(universe, Relational.Equal, 1));
        }

        public void AddLongOnlyConstraint()
        {
            foreach (var instrument in this)
            {
                // No short positions allowed -> min = 0
                Constraints.Add(LinearConstraint.Create(instrument.Name, Relational.Larger, 0));
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
