using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace DataSciLib.Statistics
{
    public struct RegressionCoefficients
    {
        public double Intercept;
        public IEnumerable<double> Coefficients;
        public IEnumerable<double> Residuals; 

        public RegressionCoefficients(double intercept, double coefficient, double[] residuals)
        {
            Intercept = intercept;
            Coefficients = new double[] { coefficient };
            Residuals = residuals;
        }

        public RegressionCoefficients(double intercept, double[] coefficients, double[] residuals)
        {
            Intercept = intercept;
            Coefficients = coefficients;
            Residuals = residuals;
        }
    }

    public class Model
    {
        public static RegressionCoefficients LinearRegression(double[] ydata, double[] xdata)
        {
            // build matrices
            var xvec = new DenseVector(xdata);
            var X = DenseMatrix.CreateFromColumns(new[] { new DenseVector(xdata.Length, 1), xvec });
            var y = new DenseVector(ydata);

            // solve
            var p = X.QR().Solve(y);
            var a = p[0];
            var b = p[1];

            var yfit = (b*xvec).Add(a);
            var resid = yfit - y;

            var regr = new RegressionCoefficients(a, b, resid.ToArray());
            return regr;
        }
    }
}
