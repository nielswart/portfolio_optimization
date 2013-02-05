// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System;
using RDotNet;
using System.Collections.Generic;
using DataSciLib.DataStructures;

namespace DataSciLib.REngine.Rmetrics
{
    public sealed class timeSeries : S4Class
    {
        private static bool packageloaded = false;

        private static void Initialize()
        {
            if (!packageloaded)
            {
                Engine.LoadPackage("timeSeries");
                Engine.LoadPackage("timeDate");
                packageloaded = true;
            }
        }
       
        private timeSeries(SymbolicExpression expression)
            : base(expression)
        {
        }

        private static Function ftimeSeries()
        {
            Initialize();
            return Engine.GetFunction("timeSeries");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="charvec"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static timeSeries Create(double[,] data, string[] charvec, string[] units)
        {
            return new timeSeries(ftimeSeries().Invoke(new SymbolicExpression[] {Engine.RMatrix(data), Engine.RVector(charvec), Engine.RVector(units)}));
        }

        public static timeSeries Create(ITimeSeries<double> timeseries)
        {
            Initialize();

            var data = timeseries.DataMatrix;
            string[] charvec = new string[timeseries.DateTime.GetLength(0)];
            int i = 0;
            foreach (var d in timeseries.DateTime)
            {
                charvec[i] = d.ToShortDateString().Replace('/', '-');
                i++;
            }

            return new timeSeries(Engine.CallFunction("timeSeries", Engine.RMatrix(data), Engine.RVector(charvec), Engine.RVector(timeseries.Names)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeseries"></param>
        /// <returns></returns>
        public static timeSeries Create(double[,] timeseriesData, DateTime[] datevector, string[] seriesNames)
        {
            Initialize();

            var data = timeseriesData;
            string[] charvec = new string[datevector.GetLength(0)];
            int i = 0;
            foreach (var d in datevector)
            {
                charvec[i] = d.ToShortDateString().Replace('/','-');
                i++;
            }

            return new timeSeries(Engine.CallFunction("timeSeries",  Engine.RMatrix(data), Engine.RVector(charvec), Engine.RVector(seriesNames)) );
        }

#if DEBUG
        public static bool IsLoaded()
        {
            return packageloaded;
        }
#endif
    }
}
