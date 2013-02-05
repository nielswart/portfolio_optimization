using DataSciLib.REngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;
using DataSciLib;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.REngine.Rmetrics.Constraints;
using DataSciLib.REngine.Rmetrics.Specification;
using DataSciLib.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace REngine.Tests.RmetricsTests
{
    [TestClass]
    public class timeSeriesTests
    {
        R Engine;
        [TestInitialize]
        public void StartEngine()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void CreateFromTimeSeriesTest()
        {
            timeSeries ts = null;
            /*
            Assert.DoesNotThrow(() =>
                {
                    ts = timeSeries.Create(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, numseries: 10, freq: DataFrequency.Monthly));
                });
            */
            Engine.Print(ts.Expression);
        }
    }
}
