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
        private List<Constraint> _constraints;
        private SortedList<string, double> _meanReturns;
        private SortedSet<string> _instruments;

        /*
        public StringBuilder ConstraintString { get; private set; }
        public ModelInfo Model { get; private set; }
        public PortfolioInfo Portfolio { get; private set; }
        public SolverInfo Solver { get; private set; }
         * */
        public DenseMatrix Amat { get; private set; }
        public double[] Bvec { get; private set; }
        public int NumEquals { get; private set; }

        /// <summary>
        /// Create the portfolio configuration for optimization
        /// </summary>
        /// <param name="portfSet"></param>
        /// <param name="expectedReturns"></param>
        /// <param name="targetReturn"></param>
        public PortfolioConfig(PortfolioSettings portfSet, SortedList<string, double> expectedReturns, double targetReturn)
        {
            //ConstraintString = new StringBuilder();
            
            int c=0;
            _instruments = portfSet.Instruments;
            _meanReturns = expectedReturns;

            // Set constraint universe
            Constraint.SetUniverse(_instruments);
            _constraints = new List<Constraint>();

            /*
            Model = new ModelInfo();
            Portfolio = new PortfolioInfo();
            Solver = new SolverInfo();
            */

            setConstraints(portfSet.Constr);
            setTargetReturnConstraints(targetReturn);
            extractConstraintMatrix();
        }
        
        public PortfolioConfig(PortfolioSettings portfSet)
        {
            //ConstraintString = new StringBuilder();
            _constraints = new List<Constraint>();
            _instruments = new SortedSet<string>();
            _meanReturns = new SortedList<string, double>();

            /*
            Model = new ModelInfo();
            Portfolio = new PortfolioInfo();
            Solver = new SolverInfo();
            */
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
            var sl1 = new SortedList<string, double>();
            foreach (var s in _instruments)
            {
                sl1.Add(s, 1);
            }
            _constraints.Add(Constraint.Create(sl1, Relational.Equal, 1));

            // Add LongOnly constraint
            if (constr.SimpleConstraint != null)
            {
                // Add Rmetrics string representation of constraint
                //this.addStringConstraints((ConstraintType)constr.SimpleConstraint);

                int index = 0;
                if (constr.SimpleConstraint == ConstraintType.LongOnly)
                {
                    foreach (var s in _instruments)
                    {
                        // No short positions allowed -> min = 0
                        _constraints.Add(Constraint.Create(_instruments, Relational.Larger, 0, index));
                        index++;
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

            /*
            if (constr.GroupConstr != null)
            {
                foreach (GroupConstraints g in constr.GroupConstr)
                {
                    setGroupConstraints(g.Assets, g.LowerLimit, g.UpperLimit);
                }
            }
             * */
        }

        private void setBoxConstraints(List<string> instruments, double lowerlimit = 0, double upperlimit = 1)
        {
            /*
            this.addMinWConstraints(instruments, lowerlimit);
            this.addMaxWConstraints(instruments, upperlimit);
            */
            var sl = new SortedList<string, double>();
            foreach (var s in _instruments)
            {
                sl.Add(s, 0);
            }

            // Add max and min weight constraints to list
            foreach (var i in instruments)
            {
                sl[i] = 1;
                _constraints.Add(Constraint.Create(sl, Relational.Smaller, upperlimit));
                _constraints.Add(Constraint.Create(sl, Relational.Larger, lowerlimit));
            }
        }

        private void setGroupConstraints(List<string> group, double lowerlimit = 0, double upperlimit = 1, double? equal = null)
        {
            /*
            if (equal == null)
            {
                this.addMinsumWConstraints(group, lowerlimit);
                this.addMaxsumWConstraints(group, upperlimit);
            }
            else
            {
                this.addEqsumWConstraints(group, (double)equal);
            }
             * */
        }

        private void setTargetReturnConstraints(double targetReturn)
        {
            // Sum of all weighted returns should be equal to the target return
            _constraints.Add(Constraint.Create(_meanReturns, Relational.Equal, targetReturn));
        }

        private void extractConstraintMatrix()
        {
            // Get the number of constraints
            var numcons = _constraints.Count();
            // Get the number of constrained variables
            var numvars = _instruments.Count();

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
                    foreach (var w in c.Weights)
                    {
                        Amat[i, j] = w.Value;
                        j++;
                    }
                    Bvec[i] = c.BValue;
                }
                else if (c.Relation == Relational.Smaller)
                {
                    foreach (var w in c.Weights)
                    {
                        Amat[i, j] = -w.Value;
                        j++;
                    }
                    Bvec[i] = -c.BValue;
                }
                else
                {
                    foreach (var w in c.Weights)
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

        #region Rmetrics string representation
        /*
        private void addStringConstraints(ConstraintType constr)
        {
            ConstraintString.Append(",\"").Append(constr.ToString()).Append("\"");
        }

        private void addMinWConstraints(List<string> instruments, double lowerbound)
        {
            // "minW[<...>]=<...>"
            ConstraintString.Append(",").Append("\"").Append("minW[c(\"");
            foreach (string s in instruments)
            {
                ConstraintString.Append("\",\"").Append(s);
            }
            ConstraintString.Append("\")]=").Append(lowerbound).Append("\"");
        }

        private void addMaxWConstraints(List<string> instruments, double upperbound)
        {
            // "maxW[<...>]=<...>"
            ConstraintString.Append(",").Append("\"").Append("maxW[c(\"");
            foreach (string s in instruments)
            {
                ConstraintString.Append("\",\"").Append(s);
            }
            ConstraintString.Append("\")]=").Append(upperbound).Append("\"");
        }

        private void addEqsumWConstraints(List<string> group, double weight)
        {
            // "eqsumW[<...>]=<...>"
            // e.g. "eqsumW[c(\"SPI\", \"SII\")]=0.6"
            ConstraintString.Append(",").Append("\"").Append("eqsumW[c(\"");
            foreach (string s in group)
            {
                ConstraintString.Append("\",\"").Append(s);
            }
            ConstraintString.Append("\")]=").Append(weight).Append("\"");
        }

        private void addMinsumWConstraints(List<string> group, double lowerbound)
        {
            // "minsumW[<...>]=<...>"
            ConstraintString.Append(",").Append("\"").Append("minsumW[c(\"");
            foreach (string s in group)
            {
                ConstraintString.Append("\",\"").Append(s);
            }
            ConstraintString.Append("\")]=").Append(lowerbound).Append("\"");
        }

        private void addMaxsumWConstraints(List<string> group, double upperbound)
        {
            // "maxsumW[<...>]=<...>"
            ConstraintString.Append(",").Append("\"").Append("maxsumW[c(\"");
            foreach (string s in group)
            {
                ConstraintString.Append("\",\"").Append(s);
            }
            ConstraintString.Append("\")]=").Append(upperbound).Append("\"");
        }
         * */
        #endregion

        #region Specification settings
        /*
        private void setSpecifications(PortfolioSpecification _spec)
        {
            // Implement rules for ensuring the correct solver is chosen
            if (Model.Type == ModelType.MarkowitzMeanVariance && Solver.OptimizationObjective == Objective.MinimizeRisk)
            {
                // Solver type must be either QP or if unconstrained Analytic
                Solver.Solver = SolverType.QP;  // Check constraints first
            }
            else if (Model.Type == ModelType.MarkowitzMeanVariance && Solver.OptimizationObjective == Objective.MaximizeReturn)
            {
            }
        }
        */
        #endregion

        private class Constraint : IComparable<Constraint>
        {
            public SortedList<string, double> Weights { get; private set; }
            public double BValue { get; private set; }
            public Relational Relation { get; private set; }
            public static SortedSet<string> Universe { get; set; }

            private Constraint(SortedList<string, double> instruments, Relational relation, double value)
            {
                Weights = instruments;
                BValue = value;
                Relation = relation;
            }

            public static void SetUniverse(SortedList<string, double> instruments)
            {
                var list = (IEnumerable<string>)instruments.Keys;
                Universe = new SortedSet<string>(list);
            }

            public static void SetUniverse(SortedSet<string> instruments)
            {
                Universe = instruments;
            }

            public static Constraint Create(SortedList<string, double> instruments, Relational relation, double value)
            {
                return new Constraint(instruments, relation, value);
            }

            
            public static Constraint Create(SortedSet<string> instruments, Relational relation, double value, int index)
            {
                if (index > instruments.Count)
                    throw new ArgumentOutOfRangeException("The specifed index is out of range");

                var sl = new SortedList<string, double>();
                int c = 0;
                foreach (var i in instruments)
                {
                    if (c == index)
                        sl.Add(i, 1);
                    else
                        sl.Add(i, 0);
                    c++;
                }
                return new Constraint(sl, relation, value);
            }

            public int CompareTo(Constraint constrB)
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
        }
    }
}
