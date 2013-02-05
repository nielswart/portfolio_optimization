using System;
using System.Collections.Generic;
using System.Text;
using PortfolioEngine.RInternals;
using PortfolioEngine.Rmetrics;
using PortfolioEngine.Semantics;
using PortfolioEngine.RInternals.Rmetrics.Specification;
using RDotNet;

namespace PortfolioEngine.Constraints
{
    internal sealed class PortfolioConstraints
    {
        internal bool NonLinearFlag { get; private set; }
        internal bool ChangedFlag { get; private set; }

        private StringBuilder constraintString;

        internal PortfolioConstraints(PortfolioSpec spec)
        {
            this.ChangedFlag = true;
            constraintString = new StringBuilder();
        }

        internal PortfolioConstraints(PortfolioSpec spec, string[] constraints)
        {
            this.ChangedFlag = true;
            constraintString = new StringBuilder();

            // Add string array
        }

        internal PortfolioConstraints(PortfolioSpec spec, ConstraintType constr)
        {
            this.ChangedFlag = true;
            constraintString = new StringBuilder();
        }

        internal string ToRValue()
        {
            return string.Empty;
        }

        public override string ToString()
        {
            return constraintString.ToString();
        }

        public SymbolicExpression Create(R Engine, Varnames variable)
        {
            // check if string starts with comma, if true remove
            string dastring = string.Empty;
            if (constraintString.ToString().StartsWith(","))
                dastring = constraintString.ToString().Remove(0, 1);
            else
                dastring = constraintString.ToString();

            if (dastring.Length > 1)
            {
                Engine.Workspace.SetVariable(variable.Constr, "\"" + dastring + "\"");
            }
            else
                Engine.Workspace.SetVariable(variable.Constr, "c(\"" + dastring + "\")");

            //Console.WriteLine("Creating constraints object {0} in R...", variable.Constr);
        }

        #region Build Constraint strings

        internal StringBuilder addStringConstraints(ConstraintType constr)
        {
            constraintString.Append(",\"").Append(constr.ToString()).Append("\"");
            return constraintString;
        }

        /// <summary>
        /// Minimum weight constraint - lower box bounds
        /// </summary>
        /// <returns>string representation of constraint</returns>
        internal StringBuilder addMinWConstraints(List<string> instruments, double lowerbound)
        {
            // "minW[<...>]=<...>"
            constraintString.Append(",").Append("\"").Append("minW[c(\"");
            foreach (string s in instruments)
            {
                constraintString.Append("\",\"").Append(s);
            }
            constraintString.Append("\")]=").Append(lowerbound).Append("\""); 
            return constraintString;
        }

        /// <summary>
        /// Maximum weight constraint - upper box bounds
        /// </summary>
        /// <returns>string representation of constraint</returns>
        internal StringBuilder addMaxWConstraints(List<string> instruments, double upperbound)
        {
            StringBuilder sb = new StringBuilder();
            // "maxW[<...>]=<...>"
            constraintString.Append(",").Append("\"").Append("maxW[c(\"");
            foreach (string s in instruments)
            {
                constraintString.Append("\",\"").Append(s);
            }
            constraintString.Append("\")]=").Append(upperbound).Append("\""); 
            return constraintString;
        }

        /// <summary>
        /// Equality group constraint - sets the total amount of an investment in a group of assets
        /// </summary>
        /// <returns>string representation of constraint</returns>
        internal StringBuilder addEqsumWConstraints(List<string> group, double weight)
        {
            // "eqsumW[<...>]=<...>"
            // e.g. "eqsumW[c(\"SPI\", \"SII\")]=0.6"
            StringBuilder sb = new StringBuilder();
            constraintString.Append(",").Append("\"").Append("eqsumW[c(\"");
            foreach (string s in group)
            {
                constraintString.Append("\",\"").Append(s);
            }
            constraintString.Append("\")]=").Append(weight).Append("\""); 

            return constraintString;
        }

        /// <summary>
        /// Minimum group constraint - sets a lower bound on the weight allocated to a group of assets
        /// </summary>
        /// <returns></returns>
        internal StringBuilder addMinsumWConstraints(List<string> group, double lowerbound)
        {
            // "minsumW[<...>]=<...>"
            StringBuilder sb = new StringBuilder();
            constraintString.Append(",").Append("\"").Append("minsumW[c(\"");
            foreach (string s in group)
            {
                constraintString.Append("\",\"").Append(s);
            }
            constraintString.Append("\")]=").Append(lowerbound).Append("\""); 

            return constraintString;
        }

        /// <summary>
        /// Maximum group constraint - sets an upper bound on the weight allocated to a group of assets
        /// </summary>
        /// <returns></returns>
        internal StringBuilder addMaxsumWConstraints(List<string> group, double upperbound)
        {
            // "maxsumW[<...>]=<...>"
            StringBuilder sb = new StringBuilder();
            constraintString.Append(",").Append("\"").Append("maxsumW[c(\"");
            foreach (string s in group)
            {
                constraintString.Append("\",\"").Append(s);
            }
            constraintString.Append("\")]=").Append(upperbound).Append("\""); 

            return constraintString;
        }

        /// <summary>
        /// Minimum covariance budget constraint - sets a lower bound on the covariance risk attributable to certain assets
        /// </summary>
        /// <returns>string representation of constraint</returns>
        internal StringBuilder addMinBConstraints()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Maximum covariance budget constraint - sets an upper bound on the covariance risk attributable to certain assets
        /// </summary>
        /// <returns></returns>
        internal StringBuilder addMaxBConstraints()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a list of non-linear functions used for the calculation of non-linear risk measures
        /// </summary>
        /// <returns></returns>
        internal StringBuilder addListFConstraints()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Minimum Non-linear functional constraint - sets a lower bound on the non-linear functional constraint 
        /// </summary>
        /// <returns></returns>
        internal StringBuilder addMinFConstraints()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Maximum Non-linear functional constraint - sets an upper bound on the non-linear functional constraint 
        /// </summary>
        /// <returns></returns>
        private string addMaxFConstraints()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
