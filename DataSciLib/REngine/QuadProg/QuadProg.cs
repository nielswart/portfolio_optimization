using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;
using MathLib = MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using PerformanceTools;

namespace DataSciLib.REngine.QuadProg
{
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
        /// Algorithm implementing the dual method of Goldfarb and Idnani (1982, 1983) for solving quadratic optimization problems of the form min(-d'b + 1/2b'Db) with constraints specified as A'b>=b_0
        /// </summary>
        /// <param name="Dmat">Matrix in the quadratic function to be minimized</param>
        /// <param name="dvec">Vector in the quadratic function to be minimized</param>
        /// <param name="Amat">Matrix defining the constraints under which the quadratic function is to be minimized</param>
        /// <param name="bvec">Vector holding the values of b_0</param>
        /// <param name="meq">The number of equality constraints (the first <param name="meq"> constraints in the Matrix)</param>
        /// <param name="factorized">Logical flag indicating whether R^-1 (where D = R'R) are passed instead of the matrix D</param>
        /// <returns></returns>
        public static void Solve(double[,] Dmat, double[] dvec, DenseMatrix Amat, double[] bvec, int meq=0, bool factorized=false)
        {
            Initialize();
            //check if dvec is null

            var expr = Engine.CallFunction("solve.QP", Engine.RMatrix(Dmat), Engine.RVector(dvec), Engine.RMatrix(Amat.ToArray()), Engine.RVector(bvec), Engine.RNumeric(meq), Engine.RBool(factorized));
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
        public static Tuple<double[], double> Solve(MathLib.Matrix<double> Dmat, double[] dvec, MathLib.Matrix<double> Amat, double[] bvec, int meq = 0, bool factorized = false)
        {
            PerformanceLogger.Start("QuadProg", "Solve", "Initialize");
            Initialize();
            PerformanceLogger.Stop("QuadProg", "Solve", "Initialize");

            DenseVector vec;
            if (dvec == null)
                vec = new DenseVector(Dmat.ColumnCount, 0);
            else
                vec = new DenseVector(dvec);

            PerformanceLogger.Start("QuadProg", "Solve", "R.CallFunction");

            Engine.SetSymbol("result",  Engine.CallFunction("solve.QP", Engine.RMatrix(Dmat.ToArray()), 
                Engine.RVector(vec.ToArray()), Engine.RMatrix(Amat.ToArray()),
                Engine.RVector(bvec), Engine.RNumeric(meq), Engine.RBool(factorized)));

            var sol = Engine.RunCommand("result$solution").AsNumeric().ToArray<double>();
            var val = Engine.RunCommand("result$value").AsNumeric().ToArray<double>().First();

            PerformanceLogger.Stop("QuadProg", "Solve", "R.CallFunction");

            return new Tuple<double[], double>(sol, val*2);

            
        }
    }
}
