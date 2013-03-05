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

    public class LinearConstraint : IConstraint
    {
        public SortedList<string, double> Coefficients { get; private set; }
        public double BValue { get; private set; }
        public Relational Relation { get; private set; }
        
        public LinearConstraint()
        {

        }

        private LinearConstraint(SortedList<string, double> instruments, Relational relation, double value)
        {
            Coefficients = instruments;
            BValue = value;
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
            var sl = new SortedList<string, double>();
            foreach (var element in instruments)
            {
               sl.Add(element.Key, element.Value);
            }
            return new LinearConstraint(sl, relation, value);
        }

        /// <summary>
        /// Sum of instruments constraint
        /// </summary>
        /// <param name="instruments"></param>
        /// <param name="relation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LinearConstraint Create(IEnumerable<string> instruments, Relational relation, double value)
        {
            // for every instrument that is also in the universe, set matrix coeffcient to 1, for all others in universe set to 0
            var sl = new SortedList<string, double>();
            foreach (var element in instruments)
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
