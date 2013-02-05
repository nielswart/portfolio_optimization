using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math.Optimization;

namespace DataSciLib.Optimization.QuadProg
{
    public class Solver
    {
        /// <summary>
        /// quadratic programming optimization solver using the dual method algorithm of Goldfarb and Idnani 
        /// </summary>
        /// <param name="Dmat"></param>
        /// <param name="dvec"></param>
        /// <param name="Amat"></param>
        /// <param name="bvec"></param>
        /// <param name="meq"></param>
        public static Tuple<double[], double> Solve(double[,] Q, double[] d, double[,] Amat, double[] bvec, int numeq)
        {
            try
            {
                // return optim = optim, weights= weights, targetReturn = bvec[1],  targetRisk = sqrt(weights*Dmat*weights)[[1,1]]
                var target = new GoldfarbIdnaniQuadraticSolver(Q.GetLength(0), Amat, bvec, numeq);
                var min = target.Minimize(Q, d);
                var sol = target.Solution;
                return new Tuple<double[],double>(sol, min);
            }
            catch 
            {
                return null;
            }
        }
    }
}
