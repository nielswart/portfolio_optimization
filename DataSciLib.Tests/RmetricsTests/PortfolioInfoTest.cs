using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics.Specification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace REngine.Tests.RmetricsTests
{
    [TestClass]
    public class PortfolioInfoTest
    {
        [TestInitialize]
        public void StartEngine()
        {
            R Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void ConstructorTest()
        {
            Console.WriteLine("Default Constructor");
            /*
            Assert.DoesNotThrow(() =>
            {
                var pi = new PortfolioInfo();
            });

            Console.WriteLine("Constructor with weights specified");
            double[] weights = new double[10];
            Assert.DoesNotThrow(() =>
            {
                var pi2 = new PortfolioInfo(weights: weights);
            });
             * */
        }

        [TestMethod]
        public void ConsistencyChecksTest()
        {

        }

    }
}
