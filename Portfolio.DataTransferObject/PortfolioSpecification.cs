// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System.Runtime.Serialization;

namespace PortfolioEngine
{
    [DataContract]
    public class PortfolioSpecification
    {
        [DataMember]
        public uint NumberofFrontierPoints { get; set; }

        [DataMember]
        public double RiskFreeRate { get; set; }
        
        [DataMember]
        public double? TargetRisk { get; set; }

        [DataMember]
        public double? TargetReturn { get; set; }

        [DataMember]
        public double[] TargetWeights { get; set; }

        public PortfolioSpecification(double rfrate, uint numpoints = 50)
        {
            this.NumberofFrontierPoints = numpoints;
            this.RiskFreeRate = rfrate;

            this.TargetReturn = null;
            this.TargetRisk = null;
            this.TargetWeights = null;
        }

        public PortfolioSpecification()
        {
            this.NumberofFrontierPoints = 50;
            this.RiskFreeRate = 0;
            this.TargetReturn = null;
            this.TargetRisk = null;
            this.TargetWeights = null;
        }
    }
}
