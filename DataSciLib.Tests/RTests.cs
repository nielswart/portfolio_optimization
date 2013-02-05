
using DataSciLib.REngine;
using System;
using DataSciLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RDotNet;
using DataSciLib;
using DataSciLib.REngine.Rmetrics;
using DataSciLib.REngine.Rmetrics.Constraints;
using DataSciLib.REngine.Rmetrics.Specification;
using DataSciLib.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace REngine.Tests
{
    [TestClass]
    public class RTests
    {
        R Engine;
        
        [TestInitialize]
        public void StartEngine()
        {
            Engine = R.Instance;
            Engine.Start(REngineOptions.QuietMode);
        }
        

        [TestMethod]
        public void IsNull()
        {
            Assert.AreNotEqual(Engine, null);
        }

        [TestMethod]
        public void IsRunning()
        {
            Engine = R.Instance;
            Assert.IsTrue(Engine.IsRunning);
        }

        [TestMethod]
        public void CanAdd()
        {
            Assert.AreEqual(4, 2 + 2);
        }
        [TestMethod]
        public void LoadDllTest()
        {
            var R_HOME = System.Environment.GetEnvironmentVariable("R_HOME");

            Console.WriteLine("R_HOME environment variable: " + R_HOME);

            if (string.IsNullOrEmpty(R_HOME))
                R_HOME = @"C:\Program Files\R\R-2.15.1";

            var envPath = Environment.GetEnvironmentVariable("PATH");

            Console.WriteLine("Path environment variables: " + envPath); 
            string rBinPath;
            if (R_HOME.EndsWith("\\"))
            {
                rBinPath = R_HOME + @"bin\x64";
            }
            else
            {
                rBinPath = R_HOME + @"\bin\x64";
            }

            Console.WriteLine("Rbinpath: " + rBinPath); 
            /*
            Environment.SetEnvironmentVariable("R_HOME", R_HOME);
            Environment.SetEnvironmentVariable("PATH", envPath + Path.PathSeparator + rBinPath);
            */
            string InstanceID = "RDotNet";
            var engine = RDotNet.REngine.CreateInstance(InstanceID);
            engine.Initialize();
        }

        [TestMethod]
        public void RNullTest()
        {
            /*
            Assert.DoesNotThrow(() =>
                {
                    var expr = Engine.RNull();
                });
             * */
        }

        [TestMethod]
        public void RNumericTest()
        {
            /*
            double number = new double();

            Assert.DoesNotThrow(() => { Engine.RNumeric(number); });

            double? number2 = null;
            
            Assert.DoesNotThrow(() => { Engine.RNumeric(number2); });

            double? number3 = 10.0;

            Assert.DoesNotThrow(() => { Engine.RNumeric(number3); });
            */
        }

        [TestMethod]
        public void RVectorTest()
        {
            /*
            double[] vector = new double[10];
            Assert.DoesNotThrow(() => { Engine.RVector(vector); });

            double[] vectornull = null;
            Assert.Throws(typeof(ArgumentNullException), () => { Engine.RVector(vectornull); });

            string[] vectorstr = new string[10];
            Assert.DoesNotThrow(() => { Engine.RVector(vectorstr); });

            string[] vectorstrnull = null;
            Assert.Throws(typeof(ArgumentNullException), () => { Engine.RVector(vectorstrnull); });
             * */
        }

        [TestMethod]
        public void RListTest()
        {
            /*
            Assert.DoesNotThrow(() =>
            {
                Engine.RList(new Tuple<string, SymbolicExpression>("solver", Engine.RString(SolverType.QP.ToRString())),
                    new Tuple<string, SymbolicExpression>("objective", Engine.RString(Objective.MinimizeRisk.ToRString())),
                    new Tuple<string, SymbolicExpression>("params", Engine.RList()),
                    new Tuple<string, SymbolicExpression>("control", Engine.RList()),
                    new Tuple<string, SymbolicExpression>("trace", Engine.RBool(false)));
            });
             * */
        }

        [TestMethod]
        public void RStringTest()
        {
            /*
            Assert.DoesNotThrow(() =>
            {
                Engine.RString(SolverType.QP.ToRString());
            });

            Assert.Throws(typeof(NullReferenceException), ()=> {
                Engine.RString("\"Hello\"");
            });
             * */
        }

        [TestMethod]
        public void LoadPackageTest()
        {
            //var ts = timeSeries.Create(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, numseries: 10, freq: DataFrequency.Monthly));
            
        }

        [TestMethod]
        public void CallFunctionTest()
        {
            var weight = 1.0 / 10;
            double[] weights = new double[10];
            for (int i = 0; i < 10; i++)
            {
                weights[i] = weight;
            }
            var spec = fPortfolioSpec.Create(weights);
            Engine.Print(spec.Expression);

            var constr = fPortfolioConstraint.Create();
            Engine.Print(constr.Expression);
            Engine.Print(spec.Expression);

            /*
            Assert.DoesNotThrow(() =>
            {
                /*
                Engine.CallFunction("feasiblePortfolio",
                    timeSeries.Create(TimeSeriesFactory<double>.SampleData.Gaussian.Create(0.01, 0.02, numseries: 10, freq: DataFrequency.Monthly)).Expression,
                    spec.Expression,
                    constr.Expression);
                 
            });
        */
        }

        [TestMethod]
        public void FunctionCallPerformanceTest()
        {
            int runs = 100;

            var mean = 0.001;
            var stddev = 0.02;
            List<double[,]> res = new List<double[,]>();
            var starttime = DateTime.Now;

            var weight = 1.0 / 10;
            double[] weights = new double[10];
            for (int i = 0; i < 10; i++)
            {
                weights[i] = weight;
            }

            var spec = fPortfolioSpec.Create(weights);
            Engine.Print(spec.Expression);

            var constr = fPortfolioConstraint.Create();
            Engine.Print(constr.Expression);

            for (int c = 0; c < runs; c++)
            {
                /*
                Engine.CallFunction("feasiblePortfolio",
                    timeSeries.Create(TimeSeriesFactory<double>.SampleData.Gaussian.Create(mean, stddev, numseries: 10, freq: DataFrequency.Monthly)).Expression,
                    spec.Expression,
                    constr.Expression);
                */
                Console.WriteLine(c);
            }

            var stoptime = DateTime.Now;
            Console.WriteLine("{0} milliseconds per calculation", (stoptime - starttime).Milliseconds / runs);
        }


    }
}
