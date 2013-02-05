using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PortfolioEngine;
using PortfolioEngine.RInternals;

namespace PortfolioEngineTests
{
    [TestFixture]
    public class PortfolioManagerTest
    {
        R Engine = R.Instance;

        PortfolioManager pman;
        [TestFixtureSetUp]
        public void Constructor()
        {
            pman = new PortfolioManager(Engine);
        }



        [Test]
        public void CalculateEfficientFrontier()
        {
            pman.CalcEfficientFrontier(new PortfolioSettings());
        }
        
    }
}
