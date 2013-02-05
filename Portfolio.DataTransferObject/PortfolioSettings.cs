// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using PortfolioEngine;

namespace PortfolioEngine
{
    [DataContract]
    public class PortfolioSettings
    {
        [DataMember]
        public OptimizationConstraints Constr { get; private set; }

        [DataMember]
        public PortfolioSpecification Spec { get; private set; }
        public SortedSet<string> Instruments { get; private set; }

        public PortfolioSettings(List<string> assets)
        {
            Constr = new OptimizationConstraints(ConstraintType.LongOnly);
            Spec = new PortfolioSpecification(50, 0);
            Instruments = new SortedSet<string>(assets);
        }

        public PortfolioSettings(OptimizationConstraints constr, PortfolioSpecification spec)
        {
            Constr = constr;
            Spec = spec;
            Instruments = new SortedSet<string>();
        }

        public void AddConstraints(OptimizationConstraints constr)
        {
            Constr = constr;
        }

        public void AddSpecifications(PortfolioSpecification spec)
        {
            Spec = spec;
        }

        public static PortfolioSettings Deserialize(string message)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PortfolioSettings));
            
            byte[] byteArray = Encoding.ASCII.GetBytes(message);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;

            var req = (PortfolioSettings)ser.ReadObject(stream);

            return req;
        }

        public static PortfolioSettings Deserialize(byte[] message)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PortfolioSettings));

            MemoryStream stream = new MemoryStream(message);
            stream.Position = 0;

            var req = (PortfolioSettings)ser.ReadObject(stream);

            return req;
        }

        public string Serialize()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PortfolioSettings));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, this);

            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);

            return sr.ReadToEnd();
        }
    }
}
