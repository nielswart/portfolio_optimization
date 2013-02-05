using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.REngine;

namespace DataSciLib.REngine.Rmetrics.Specification
{
    public class ModelInfo
    {
        public ModelType Type { get; private set; }
        public Objective OptimizationObjective { get; private set; }
        public Estimator Estimator { get; private set; }

        public ModelInfo(ModelType type = ModelType.MarkowitzMeanVariance, Objective opt = Objective.MinimizeRisk,
            Estimator est = Estimator.Sample)
        {
            // Initialize with default values if none are passed as parameters
            Type = type;
            OptimizationObjective = opt;
            Estimator = est;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public void SetModelObjective(Objective obj)
        {
            this.OptimizationObjective = obj;
        }

        public void SetModelEstimator(Estimator estim)
        {
            this.Estimator = estim;
        }
    }
}
