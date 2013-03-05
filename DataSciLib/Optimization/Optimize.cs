using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSciLib.Optimization
{
    public class Optimize
    {
        // TODO: Use function composition

        /// <summary>
        /// One dimensional function optimization by approximating the point where the <paramref name="func"/> f(x) attains a minimum on the interval (<paramref name="ax"/>, <paramref name="bx"/>)
        /// </summary>
        /// <param name="function">Function that evaluates f(x) for any x in the interval (<paramref name="ax"/>, <paramref name="bx"/>)</param>
        /// <param name="ax">Left endpoint of interval</param>
        /// <param name="bx">Right endpoint of interval</param>
        /// <param name="precision"></param>
        /// <returns>The value at which the function attains a minimum</returns>
        public static double FindMinimum(Func<double, double> func, double ax, double bx, double tol)
        {
            // The method used is a combination of golden section search and successive parabolic interpolation.  
            // Convergence is never much slower than that for a fibonacci search.  If func has a continuous second
            // derivative which is positive at the minimum (which is not at ax or bx), then convergence is superlinear, 
            // and usually of the order of about  1.324....  
            // The function func is never evaluated at two points closer together than eps*abs(fmin) + (tol/3), 
            // where eps is approximately the square root of the relative machine precision. 
            // If func is a unimodal function and the computed values of func are always unimodal when
            // separated by at least eps*abs(x) + (tol/3), then fmin approximates the abcissa of the global minimum of func on the interval (ax,bx) with
            // an error less than 3*eps*abs(fmin) + tol.  
            // If func is not unimodal, then fmin may approximate a local, but perhaps non-global, minimum to the same accuracy.   
            // This function is a slightly modified version of the Fortran algorithm found at http://www.netlib.org/fmm/fmin.f, 
            // which in turn is a slight modification of the Algol 60 procedure localmin given in:
            // Richard Brent, Algorithms for minimization without derivatives, Prentice - Hall, Inc. (1973).

            throw new NotImplementedException();
        }

        //
    }
}
