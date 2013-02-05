using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;

namespace DataSciLib.REngine.Rmetrics.Constraints
{
    public sealed class fPortfolioConstraint : S4Class
    {
        private static bool packageloaded = false;

        private static void Initialize()
        {
            if (!packageloaded)
            {
                Engine.LoadPackage("fPortfolio");
                Engine.LoadPackage("quadprog");
                packageloaded = true;
            }
        }

        private fPortfolioConstraint(SymbolicExpression expression)
            : base(expression)
        {
        }

        ~fPortfolioConstraint()
        {
        }

        private static Function portfolioConstraint()
        {
            return Engine.GetFunction("portfolioConstraints");
        }

        public static fPortfolioConstraint Create()
        {
            return new fPortfolioConstraint(Engine.RString("LongOnly"));
        }

        public static fPortfolioConstraint Create(StringBuilder constraintString)
        {
            // check if string starts with comma, if true remove
            string dastring = string.Empty;
            if (constraintString.ToString().StartsWith(","))
                dastring = constraintString.ToString().Remove(0, 1);
            else
                dastring = constraintString.ToString();

            SymbolicExpression constrexpr;

            if (dastring.Length > 1)
            {
                constrexpr = Engine.RunCommand(dastring);
            }
            else
            {
                constrexpr = Engine.RunCommand("c(" + dastring + ")");
            }

            constrexpr.Protect();
            return new fPortfolioConstraint(constrexpr);
        }
        
        
    }
}
