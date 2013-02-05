
using DataSciLib.REngine;
using DataSciLib.REngine.Rmetrics.Specification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace REngine.Tests.RmetricsTests
{
    [TestClass]
    public class fPortfolioSpecTest
    {
        [TestInitialize]
        public void StartEngine()
        {
            R Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void CreateSpecTest()
        {
            double[] weights = new double[10];
            fPortfolioSpec.Create();
        }
    }
}
