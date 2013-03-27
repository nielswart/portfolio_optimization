using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;
using MathLib = MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DataSciLib.REngine.Optimization
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
        public static R Engine = R.Instance;

        private static bool packageloaded = false;

        private static void Initialize()
        {
            if (!packageloaded)
            {
                if (!Engine.IsRunning)
                    throw new ApplicationException("R Engine not yet initialized");
                Engine.LoadPackage("quadprog");
                packageloaded = true;
            }
        }

        /// <summary>
        /// Solve the quadratic programming optimization problem using the Goldfarb-Idnani dual method
        /// </summary>
        /// <param name="Dmat"></param>
        /// <param name="dvec"></param>
        /// <param name="Amat"></param>
        /// <param name="bvec"></param>
        /// <param name="meq"></param>
        /// <param name="factorized"></param>
        /// <returns></returns>
        public static OptimizationResult Solve(MathLib.Matrix<double> Dmat, double[] dvec, MathLib.Matrix<double> Amat, double[] bvec, int meq = 0, bool factorized = false)
        {
            //PerformanceLogger.Start("QuadProg", "Solve", "Initialize");
            Initialize();
            //PerformanceLogger.Stop("QuadProg", "Solve", "Initialize");

            DenseVector vec;
            if (dvec == null)
                vec = new DenseVector(Dmat.ColumnCount, 0);
            else
                vec = new DenseVector(dvec);

            //PerformanceLogger.Start("QuadProg", "Solve", "R.CallFunction");

            Engine.SetSymbol("result",  Engine.CallFunction("solve.QP", Engine.RMatrix(Dmat.ToArray()), 
                Engine.RVector(vec.ToArray()), Engine.RMatrix(Amat.ToArray()),
                Engine.RVector(bvec), Engine.RNumeric(meq), Engine.RBool(factorized)));

            var sol = Engine.RunCommand("result$solution").AsNumeric().ToArray<double>();
            var val = Engine.RunCommand("result$value").AsNumeric().ToArray<double>().First();

            //PerformanceLogger.Stop("QuadProg", "Solve", "R.CallFunction");

            return new OptimizationResult(sol, val*2);

        }
    }
}
