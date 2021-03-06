﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSciLib.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimeSeries.Tests
{
    [TestClass]
    public class TimeSeriesFactoryTests
    {
        //[TestMethod]
        //public void CreateFromFile()
        //{
        //    var timeseries = TimeSeriesFactory<double>.FromFile("small_test.csv");
        //    Console.WriteLine(timeseries.DataMatrix.Print());
        //}

        [TestMethod]
        public void CreateFromVector()
        {
            var data = TimeSeriesFactory<double>.Create(new[] { 0, 0.02, -0.03, 0.02, -0.015, 0, 0.02, 0.03, 0.02, -0.015, 0, -0.02, 0.03, 0.02, -0.015 }, 0, DataFrequency.Monthly);
        }

        [TestMethod]
        public void CreateGaussianSample()
        {
            var ts = TimeSeriesFactory<double>.SampleData.Gaussian(0.01, 0.02, freq: DataFrequency.Monthly);
            Assert.AreEqual(ts.RowCount, 100);

            Console.WriteLine(ts.Data.Print());
            Console.WriteLine(ts.Name);
        }

    }
}
