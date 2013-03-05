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
            _samplePortfolio = samplePortfolio;
            setTargetReturnConstraints(targetReturn);
            extractConstraintMatrix();
        }
        
        public ConfigurationManager(IPortfolio samplePortfolio)
        {
            _samplePortfolio = samplePortfolio;
            extractConstraintMatrix();
        }
        
        private void setTargetReturnConstraints(double targetReturn)
        {
            // Sum of all weighted returns should be equal to the target return
            var instList = from mr in _samplePortfolio
                           select new KeyValuePair<string, double>(mr.Name, mr.Mean);

            _samplePortfolio.Constraints.Add(LinearConstraint.Create(instList.ToDictionary(a => a.Key, b => b.Value), Relational.Equal, targetReturn));
        }

        private void extractConstraintMatrix()
        {
            // Get the number of constraints
            var numcons = _samplePortfolio.Constraints.Count();
            // Get the number of constrained variables
            var numvars = _samplePortfolio.Count;

            Amat = new DenseMatrix(new double[numcons, numvars]);
            Bvec = new double[numcons];
            int i = 0, j = 0, numequals = 0;

            _samplePortfolio.Constraints.Sort();

            // TODO: write more functional (map instead of loop)
            foreach (var c in _samplePortfolio.Constraints)
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
    }
}
