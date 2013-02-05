// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;

namespace DataSciLib.DataStructures
{
    public static class TimeSeriesExtensions
    {
        public static ITimeSeries<T> Window<T>(this ITimeSeries<T> timeseries, DateTime startdate, DateTime enddate)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> SlidingWindow<T>(this ITimeSeries<T> timeseries, DateTime startdate, DateTime enddate)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> ToMonthly<T>(this ITimeSeries<T> timeseries)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> ToQuarterly<T>(this ITimeSeries<T> timeseries)
        {
            throw new NotImplementedException();
        }

        public static int GetFrequency<T>(this ITimeSeries<T> timeseries)
        {
            // return 252 if frequency daily; 12 monhtly; 4 quarterly
            throw new NotImplementedException();
        }

        public static double[] AnnualisedMean<T>(this ITimeSeries<T> timeseries)
        {
            throw new NotImplementedException();
        }

        public static double[] AnnualisedStdDev<T>(this ITimeSeries<T> timeseries)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> Returns<T>(this ITimeSeries<T> timeseries, CalculationMethod method = CalculationMethod.Geometric)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> Cumulate<T>(this ITimeSeries<T> timeseries, double baseVal = 100)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> Concat<T>(this ITimeSeries<T> first, ITimeSeries<T> second)
        {
            throw new NotImplementedException();
        }
    }
}
