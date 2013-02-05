using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PortfolioEngine.Representation;
using PortfolioEngine;

namespace PortfolioEngineTests
{
    [TestFixture]
    public class PortfolioSettingsTest
    {
        PortfolioSettings pset;
        [TestFixtureSetUp]
        public void Constructor()
        {
            pset = new PortfolioSettings();

        }

        [Test]
        public void TestConstructors()
        {
            var ps1 = new PortfolioSettings();

            var ps2 = new PortfolioSettings(new[] { "ASA", "AGL", "BIL", "OML", "SBK" }.ToList<string>());

            var ps3 = new PortfolioSettings(new Constraints(), new Specification(), new DataSet());

        }
    }
}
