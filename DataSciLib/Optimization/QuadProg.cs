using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math.Optimization;
using MathLib = MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using PerformanceTools;

namespace DataSciLib.Optimization
{
    public struct OptimizationResult
    {
        public readonly double[] Solution;
        public readonly double Value;

        public OptimizationResult(double[] sol, double val)
        {
            Solution = sol;
            Value = val;
        }
    }

    public class QuadProg
    {
        /// <summary>
        /// Quadratic programming optimization solver using the dual method algorithm of Goldfarb and Idnani 
        /// </summary>
        /// <param name="Dmat"></param>
        /// <param name="dvec"></param>
        /// <param name="Amat"></param>
        /// <param name="bvec"></param>
        /// <param name="meq"></param>
        public static OptimizationResult Solve(MathLib.Matrix<double> Dmat, double[] dvec, MathLib.Matrix<double> Amat, double[] bvec, int meq = 0, bool factorized = false)
        {
            // return optim = optim, weights= weights, targetReturn = bvec[1],  targetRisk = sqrt(weights*Dmat*weights)[[1,1]]
            var target = new GoldfarbIdnaniQuadraticSolver(Dmat.RowCount, Amat.ToArray(), bvec, meq);
            var min = target.Minimize(Dmat.ToArray(), dvec);
            var sol = target.Solution;
            return new OptimizationResult(sol, min*2);
        }
    }
}
