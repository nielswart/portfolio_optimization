using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using DataSciLib.Optimization.QuadProg;
using DataSciLib.DataStructures;
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.REngine.Rmetrics.Specification;
using DataSciLib.REngine.Rmetrics.Constraints;

namespace PortfolioEngine.Settings
{
    internal enum Relational
    {
        Larger,
        Smaller,
        Equal
    }

    internal class PortfolioConfig
    {
        private List<LinearConstraint> _constraints;
        private List<MeanVariance> _meanVar;
        private SortedSet<string> _universe;

        public DenseMatrix Amat { get; private set; }
        public double[] Bvec { get; private set; }
        public int NumEquals { get; private set; }

        /// <summary>
        /// Create the portfolio configuration for optimization
        /// </summary>
        /// <param name="portfSet"></param>
        /// <param name="expectedReturns"></param>
        /// <param name="targetReturn"></param>
        public PortfolioConfig(PortfolioSettings portfSet, List<MeanVariance> meanVar, double targetReturn)
        {           
            int c=0;
            _universe = portfSet.Instruments;
            _meanVar = meanVar;

            // Set constraint universe
            LinearConstraint.SetUniverse(_universe);
            _constraints = new List<LinearConstraint>();

            setConstraints(portfSet.Constr);
            setTargetReturnConstraints(targetReturn);
            extractConstraintMatrix();
        }
        
        public PortfolioConfig(PortfolioSettings portfSet)
        {
            //ConstraintString = new StringBuilder();
            _constraints = new List<LinearConstraint>();
            _universe = new SortedSet<string>();
           
            setConstraints(portfSet.Constr);
            extractConstraintMatrix();
        }
        
        /// <summary>
        /// Parse the three types of constraints currently defined for Portfolio Optimization
        /// </summary>
        /// <param name="constr"></param>
        private void setConstraints(OptimizationConstraints constr)
        {
            // Add base constraint -> all weights must sum to 1
            _constraints.Add(LinearConstraint.Create(_universe, Relational.Equal, 1));

            // Add LongOnly constraint
            if (constr.SimpleConstraint != null)
            {
                if (constr.SimpleConstraint == ConstraintType.LongOnly)
                {
                    foreach (var s in _universe)
                    {
                        // No short positions allowed -> min = 0
                        _constraints.Add(LinearConstraint.Create(s, Relational.Larger, 0));
                    }
                }
            }

            if (constr.BoxConstr != null)
            {
                foreach (BoxConstraints b in constr.BoxConstr)
                {
                    setBoxConstraints(b.Assets, b.LowerLimit, b.UpperLimit);
                }
            }
        }

        private void setBoxConstraints(HashSet<string> instruments, double lowerlimit = 0, double upperlimit = 1)
        {
            _constraints.Add(LinearConstraint.Create(instruments, Relational.Smaller, upperlimit));
            _constraints.Add(LinearConstraint.Create(instruments, Relational.Larger, lowerlimit));
        }

        private void setGroupConstraints(HashSet<string> group, double lowerlimit = 0, double upperlimit = 1, double? equal = null)
        {
            
        }

        private void setTargetReturnConstraints(double targetReturn)
        {
            // Sum of all weighted returns should be equal to the target return
            var instList = from mr in _meanVar
                           select new KeyValuePair<string, double>(mr.Symbol, mr.Mean);

            _constraints.Add(LinearConstraint.Create(instList.ToDictionary(a => a.Key, b => b.Value), Relational.Equal, targetReturn));
        }

        private void extractConstraintMatrix()
        {
            // Get the number of constraints
            var numcons = _constraints.Count();
            // Get the number of constrained variables
            var numvars = _universe.Count();

            Amat = new DenseMatrix(new double[numcons, numvars]);
            Bvec = new double[numcons];
            int i = 0, j = 0, numequals = 0;

            _constraints.Sort();

            // TODO: write more functional (map instead of loop)
            foreach (var c in _constraints)
            {
                if (c.Relation == Relational.Equal)
                {
                    numequals++;
                    foreach (var w in c.Coefficients)
                    {
                        Amat[i, j] = w.Value;
                        j++;
                    }
                    Bvec[i] = c.BValue;
                }
                else if (c.Relation == Relational.Smaller)
                {
                    foreach (var w in c.Coefficients)
                    {
                        Amat[i, j] = -w.Value;
                        j++;
                    }
                    Bvec[i] = -c.BValue;
                }
                else
                {
                    foreach (var w in c.Coefficients)
                    {
                        Amat[i, j] = w.Value;
                        j++;
                    }
                    Bvec[i] = c.BValue;
                }

                i++;
                j = 0;
            }
            NumEquals = numequals;
        }

        private class LinearConstraint : IComparable<LinearConstraint>
        {
            public SortedList<string, double> Coefficients { get; private set; }
            public double BValue { get; private set; }
            public Relational Relation { get; private set; }
            private static SortedSet<string> _universe { get; set; }
            public SortedSet<string> Universe 
            {
                get { return _universe; }
            }

            private LinearConstraint(SortedList<string, double> instruments, Relational relation, double value)
            {
                Coefficients = instruments;
                BValue = value;
                Relation = relation;
            }

            public static void SetUniverse(IEnumerable<string> instruments)
            {
                _universe = new SortedSet<string>(instruments);
            }

            public static void SetUniverse(SortedSet<string> instruments)
            {
                _universe = instruments;
            }

            #region Create Constraints

            public static LinearConstraint Create(IDictionary<string, double> instruments, Relational relation, double value)
            {
                var sl = new SortedList<string, double>();
                foreach (var element in _universe)
                {
                    if (instruments.ContainsKey(element))
                        sl.Add(element, instruments[element]);
                    else
                        sl.Add(element, 0);
                }
                return new LinearConstraint(sl, relation, value);
            }

            public static LinearConstraint Create(IEnumerable<string> instruments, Relational relation, double value)
            {
                // for every instrument that is also in the universe, set matrix coeffcient to 1, for all others in universe set to 0
                var sl = new SortedList<string, double>();
                foreach (var element in _universe)
                {
                    if (instruments.Contains(element))
                        sl.Add(element, 1);
                    else
                        sl.Add(element, 0);
                }
                return new LinearConstraint(sl, relation, value);
            }

            public static LinearConstraint Create(string instrument, Relational relation, double value)
            {
                throw new NotImplementedException();
            }

            public int CompareTo(LinearConstraint constrB)
            {
                if (this.Relation != Relational.Equal && constrB.Relation == Relational.Equal)
                {
                    return 1;
                }
                else if (this.Relation == Relational.Equal)
                    return -1;
                else
                    return 0;
            }

            #endregion
        }
    }
}
