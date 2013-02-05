using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortfolioEngine;
using PortfolioEngine.Portfolios;
using DataSciLib.REngine;
using DataSciLib.DataStructures;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PortfolioEngineLib.Tests
{
    [TestClass]
    public class VaRTests
    {
        [TestInitialize]
        public void StartEngine()
        {
            R Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }

        [TestMethod]
        public void HistoricalVaRTest()
        {

        }

        [TestMethod]
        public void GaussianVaRTest()
        {

        }

        [TestMethod]
        public void CornishFisherVaRTest()
        {

        }
    }
}
