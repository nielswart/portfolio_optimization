// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

namespace DataSciLib.REngine
{
    public enum Estimator
    {
        Sample,
        MVE,        // Minimum volume ellipsoid estimator
        MCD,        // minimum covariance determinant
        OGK,        // Orthogonalized Gnanadesikan-Kettenring estimator
        Shrink,     // Shrinkage estimator
        Bagged,     // Bootstrap aggregator
        NNVE        // Nearest Neighbour Variance estimator
    }

    public enum ModelType
    {
        MarkowitzMeanVariance,  // mean-variance Markowitz
        ConditionalVaR,         // mean-conditional Value at Risk
        APTVariance,            // Factor in APT fashion with risk.
        APTCVaR
    }

    public enum Objective
    {
        MinimizeRisk,
        MaximizeReturn
    }

    public enum SolverType
    {
        QP,   // Quadratic Programming solver - default QP solver
        LP,                     // Linear Programming solver - default LP solver
        Analytic,               // Analytical solver
        Ipop,                   // Alternative QP solver
        LPapi,                  // Alternative LP solver
        Symphony,               // Alternative LP solver
        Socp,                   // QP solver for quadrativ constaints
        Sonlp2                  // Non-linear solver for non-linear constraints
    }

    public enum PortfolioWeighting
    {
        EquallyWeighted,
        ValueWeighted
    }

    public static partial class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static string ToRString(this ModelType type)
        {
            switch(type)
            {
                case ModelType.ConditionalVaR: 
                    return "CVaR";

                case ModelType.MarkowitzMeanVariance:
                    return "MV";

                default:
                    return "MV";
            }
        }

        public static string ToRString(this Estimator est)
        {
            switch (est)
            {
                case Estimator.Sample:
                    return "covEstimator";

                default:
                    return "covEstimator";
            }
            
        }

        public static string ToRString(this Objective obj)
        {
            switch (obj)
            {
                case Objective.MinimizeRisk:
                    return "minRisk";

                default:
                    return "minRisk";
            }
        }

        public static string ToRString(this SolverType solvr)
        {
            switch (solvr)
            {
                case SolverType.QP:
                    return "solveRquadprog";

                default:
                    return "solveRquadprog";
            }
        }
    }

}
