using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataSciLib.DataStructures;
using System.Collections.Generic;

namespace DataSciLib.Tests
{
    [TestClass]
    public class TimeSeriesExtensionMethods
    {
        [TestMethod]
        public void FirstDifferenceTest()
        {
            var tsl = new List<TimeDataPoint<double>>();

            tsl.Add(new TimeDataPoint<double>(DateTime.Now.AddDays(2), 10));
            tsl.Add(new TimeDataPoint<double>(DateTime.Now.AddDays(1), 11));
            tsl.Add(new TimeDataPoint<double>(DateTime.Now.AddDays(3), 13));
            tsl.Add(new TimeDataPoint<double>(DateTime.Now.AddDays(5), 11));
            tsl.Add(new TimeDataPoint<double>(DateTime.Now.AddDays(8), 12));
            tsl.Add(new TimeDataPoint<double>(DateTime.Now.AddDays(7), 14));
            tsl.Add(new TimeDataPoint<double>(DateTime.Now.AddDays(11), 15));

            var ts = TimeSeriesFactory<double>.Create(tsl, "Lala", frequency: DataFrequency.Daily);

            var ret = ts.FirstDifference(ReturnMethod.Arithmetic);
        }
    }
}
