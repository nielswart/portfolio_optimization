using MathNet.Numerics.LinearAlgebra.Double;
using PortfolioEngine.Portfolios;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioEngine.Settings
{
    internal class ConfigurationManager
    {
        private IPortfolio _samplePortfolio;

        public DenseMatrix Amat { get; private set; }
        public double[] Bvec { get; private set; }
        public int NumEquals { get; private set; }

        public ConfigurationManager(IPortfolio samplePortfolio, double targetReturn)
        {
            _samplePortfolio = samplePortfolio.Copy();
            setTargetReturnConstraints(targetReturn);
            extractConstraintMatrix();
        }
        
        public ConfigurationManager(IPortfolio samplePortfolio)
        {
            _samplePortfolio = samplePortfolio.Copy();
            extractConstraintMatrix();
        }
        
        private void setTargetReturnConstraints(double targetReturn)
        {
            // Sum of all weighted returns should be equal to the target return
            var instList = from mr in _samplePortfolio
                           select new KeyValuePair<string, double>(mr.ID, mr.Mean);

            _samplePortfolio.Constraints.Add(LinearConstraint.Create(instList.ToDictionary(a => a.Key, b => b.Value), Relational.Equal, targetReturn));
        }

        private void extractConstraintMatrix()
        {
            // Get the number of constraints
            var numcons = _samplePortfolio.Constraints.Count();

            // Get the constraint variable IDs - (weights of specific instruments in portfolio)
            var vars = from ins in _samplePortfolio
                       select ins.ID;

            // Get the number of constrained variables
            var numvars = _samplePortfolio.Count;

            Amat = new DenseMatrix(new double[numcons, numvars]);
            Bvec = new double[numcons];
            int i = 0, j = 0, numequals = 0;
            _samplePortfolio.Constraints.Sort();

            // for each constraint in list of constraints
            foreach (var c in _samplePortfolio.Constraints)
            {
                if (c.Relation == Relational.Equal)
                {
                    numequals++;
                    foreach (var v in vars)
                    {
                        Amat[i, j] = c.EquationTerms.ContainsKey(v) ? c.EquationTerms[v] : 0;
                        j++;
                    }
                    Bvec[i] = c.ConstantTerm;
                }

                else if (c.Relation == Relational.Smaller)
                {
                    foreach (var v in vars)
                    {
                        Amat[i, j] = c.EquationTerms.ContainsKey(v) ? -c.EquationTerms[v] : 0;
                        j++;
                    }
                    Bvec[i] = -c.ConstantTerm;
                }
                else
                {
                    foreach (var v in vars)
                    {
                        Amat[i, j] = c.EquationTerms.ContainsKey(v) ? c.EquationTerms[v] : 0;
                        j++;
                    }
                    Bvec[i] = c.ConstantTerm;
                }
                i++;
                j = 0;
                
            }
            NumEquals = numequals;
        }
    }
}
