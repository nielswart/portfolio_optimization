// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PortfolioEngine
{
    [DataContract]
    public class OptimizationConstraints
    {
        [DataMember]
        public List<BoxConstraints> BoxConstr;

        [DataMember]
        public List<GroupConstraints> GroupConstr;

        [DataMember]
        public ConstraintType? SimpleConstraint;

        public OptimizationConstraints()
        {
            SimpleConstraint = null;
            BoxConstr = null;
            GroupConstr = null;
        }

        public OptimizationConstraints(ConstraintType sconstr)
            : this()
        {
            this.SimpleConstraint = sconstr;
        }

        public void AddBoxConstraint(HashSet<string> assets, double lowerlimit, double upperlimit)
        {
            this.BoxConstr.Add(new BoxConstraints(assets, lowerlimit, upperlimit));
        }

        public void AddBoxConstraint(string asset, double lowerlimit, double upperlimit)
        {
            this.BoxConstr.Add(new BoxConstraints( new HashSet<string>{ asset }, lowerlimit, upperlimit));
        }

        public void AddGroupConstraints(List<string> assets, double lowerlimit, double upperlimit)
        {
            this.GroupConstr.Add(new GroupConstraints(assets, lowerlimit, upperlimit));
        }
    }

    [DataContract]
    public struct BoxConstraints
    {
        [DataMember]
        public HashSet<string> Assets { get; private set; }

        [DataMember]
        public double UpperLimit { get; private set; }

        [DataMember]
        public double LowerLimit { get; private set; }

        public BoxConstraints(HashSet<string> assets, double lowlim, double upperlim)
            : this()
        {
            this.Assets = assets;
            this.UpperLimit = upperlim;
            this.LowerLimit = lowlim;
        }
    }

    [DataContract]
    public struct GroupConstraints
    {
        [DataMember]
        public List<string> Assets { get; private set; }
        [DataMember]
        public double UpperLimit { get; private set; }
        [DataMember]
        public double LowerLimit { get; private set; }

        public GroupConstraints(List<string> assets, double lowlim, double upperlim)
            : this()
        {
            this.Assets = assets;
            this.UpperLimit = upperlim;
            this.LowerLimit = lowlim;
        }
    }
}
