using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSciLib.BackTest
{
    /*
    public class RollingWindows
    {
        public int Horizon { get; private set; }
        public int Shift { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public List<Tuple<DateTime, DateTime>> Windows { get; private set; }
    }

    public class BackTest
    {
        public delegate IPortfolio StrategyFunction(PortfolioSettings portfset, ITimeSeries<double> data);

        private StrategyFunction _Strategy;

        public RollingWindows Windows { get; private set; }

        public ITimeSeries<double> Recommendations { get; private set; }

        public BackTest(ITimeSeries<double> benchmark, ITimeSeries<double> assets, PortfolioSettings portfset, StrategyFunction strategy)
        {
            _Strategy = strategy;

      
            var res = from rows in benchmark
                      let ts = rows
                      from t in ts
                      where t. < 0
                      select t;
             *
        }

        public void Run()
        {

        }

        public void Setup(DateTime startdate, DateTime enddate, int horizon, int shift)
        {

        }
    }
*/
}
