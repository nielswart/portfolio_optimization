using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioEngine;
using PortfolioEngine.Portfolios;
using DataSciLib.REngine;
using DataSciLib.DataStructures;
using PortfolioEngine.Settings;
using DataSciLib;
using MathNet.Numerics.Distributions;
using PerformanceTools;
using DataSciLib.IO;

namespace PortfolioEngineLib.Tests
{
    [TestClass]
    public class EfficientFrontierTests
    {
        Optimization optimizer;
        R Engine;
        SortedList<string, double> mean;
        CovarianceMatrix cov;

        [TestInitialize]
        public void StartEngine()
        {
            R Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
            optimizer = new Optimization();

            mean = new SortedList<string, double>();
            mean.Add("DST", 0.01443717);
            mean.Add("GPL", 0.01018943);
            mean.Add("ASR", 0.02955213);

            cov = new CovarianceMatrix(new double[,] { 
                    { 0.0038525447, 0.001269398, -0.0002083454 }, 
                    { 0.0012693982, 0.002107381,  0.001064057 }, 
                    { -0.0002083454, 0.0010640575, 0.0062396967} });

        }

        [TestMethod]
        public void CalculateEfficientFrontier()
        {

            //OptimizationConstraints constr = new OptimizationConstraints(ConstraintType.LongOnly);
            //PortfolioSpecification spec = new PortfolioSpecification(numpoints: 50, rfrate: 0);
            //PortfolioSettings pset = new PortfolioSettings(new List<string> { "DST", "GPL", "ASR" });
            //pset.AddConstraints(constr);
            //pset.AddSpecifications(spec);

            /*var res = optimizer.CalcEfficientFrontier(pset, mean, cov);
            var riskreturn = from p in res
                             select new List<string> { p.Metrics[Metrics.StdDev].ToString(), p.Metrics[Metrics.Mean].ToString() };
            
            int c =0;
            Writer csv = new Writer("riskReturn.csv");
            csv.CSV(riskreturn);
             * */
        }
        
        [TestMethod]
        public void EfficientFrontierPerformanceTest()
        {
            //OptimizationConstraints constr = new OptimizationConstraints(ConstraintType.LongOnly);
            //PortfolioSpecification spec = new PortfolioSpecification(numpoints: 50, rfrate: 0);
            //PortfolioSettings pset = new PortfolioSettings(new List<string> { "DST", "GPL", "ASR" });
            //pset.AddConstraints(constr);
            //pset.AddSpecifications(spec);

            int runs = 100;
            for (int c = 0; c < runs; c++)
            {
                PerformanceLogger.Start("EfficientFrontierTests", "EfficientFrontierPerformanceTest", "Optimization.CalcEfficientFrontier");
                //var res = optimizer.CalcEfficientFrontier(pset, mean, cov);
                PerformanceLogger.Stop("EfficientFrontierTests", "EfficientFrontierPerformanceTest", "Optimization.CalcEfficientFrontier");
            }
            PerformanceLogger.WriteToCSV("performancedata.csv");
        }
    }
}
