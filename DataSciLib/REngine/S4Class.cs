using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;

namespace DataSciLib.REngine
{
    public abstract class S4Class
    {
        public SymbolicExpression Expression { get; protected set; }
        public static R Engine = R.Instance;
     
        protected S4Class(SymbolicExpression expression)
        {
            if (!expression.IsProtected)
                expression.Protect();
            this.Expression = expression;
        }

        ~S4Class()
        {
            //if (Expression.IsProtected)
            //    Expression.Unprotect();
        }
    }
}
