using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataSciLib.DataStructures;

namespace DataSciLib.Tests
{
    [TestClass]
    public class CovarianceMatrixTests
    {
        [TestMethod]
        public void CreateFromDelimitedFileTest()
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            var cov = CovarianceMatrix.CreateFromDelimitedFile("../../data/covariance.csv");
        }
    }
}
