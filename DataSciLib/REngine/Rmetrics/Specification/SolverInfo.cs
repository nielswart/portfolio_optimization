using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.REngine.Rmetrics;

namespace DataSciLib.REngine.Rmetrics.Specification
{
    public class SolverInfo
    {
        public SolverInfo(SolverType slvr = SolverType.QP, Objective opt = Objective.MinimizeRisk,
            bool trace = false)
        {
            // Initialize with default values if none are passed as parameters
            Solver = slvr;
            OptimizationObjective = opt;
            Trace = trace;
        }
        public SolverType Solver { get; set; }
        public Objective OptimizationObjective { get; private set; }
        public bool Trace { get; private set; }

        public override string ToString()
        {
            return "Not yet implemented for class SolverInfo!";
        }

        private void setSolverTrace(bool trace)
        {
            throw new NotImplementedException();
        }

        private void setSolverParams(params string[] ptionalParameters)
        {
            throw new NotImplementedException();
        }
    }
}
