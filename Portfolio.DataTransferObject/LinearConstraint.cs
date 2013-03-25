using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioEngine
{
    public enum Relational
    {
        Larger,
        Smaller,
        Equal
    }

    public interface IConstraint
    {
        Relational Relation { get; }
    }

    public class LinearConstraint : IConstraint, IComparable<LinearConstraint>
    {
        public Dictionary<string, double> EquationTerms { get; private set; }
        public double ConstantTerm { get; private set; }
        public Relational Relation { get; private set; }
        
        public LinearConstraint()
        {

        }

        private LinearConstraint(Dictionary<string, double> instruments, Relational relation, double value)
        {
            EquationTerms = instruments;
            ConstantTerm = value;
            Relation = relation;
        }

        #region Create Constraints

        /// <summary>
        /// Sum of instruments constraint - coefficients specified
        /// </summary>
        /// <param name="instruments"></param>
        /// <param name="relation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LinearConstraint Create(IDictionary<string, double> instruments, Relational relation, double value)
        {
            return new LinearConstraint((Dictionary<string, double>)instruments, relation, value);
        }

        /// <summary>
        /// Sum of instruments constraint
        /// </summary>
        /// <param name="instruments"></param>
        /// <param name="relation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LinearConstraint Create(IEnumerable<string> instrumentIds, Relational relation, double value)
        {
            var sl = new Dictionary<string, double>();
            foreach (var element in instrumentIds)
            {
                sl.Add(element, 1);
            }
            return new LinearConstraint(sl, relation, value);
        }

        /// <summary>
        /// Specific instrument constraint
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="relation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LinearConstraint Create(string instrumentId, Relational relation, double value)
        {
            var sl = new Dictionary<string, double>();
            sl.Add(instrumentId, 1);
            return new LinearConstraint(sl, relation, value);
        }

        public int CompareTo(LinearConstraint constrB)
        {
            if (this.Relation == Relational.Equal && constrB.Relation != Relational.Equal)
            {
                return -1;
            }
            else if (this.Relation == Relational.Larger && constrB.Relation == Relational.Smaller)
                return -1;
            else if (this.Relation == constrB.Relation)
                return 0;
            else
                return 1;
        }

        #endregion
    }
}
