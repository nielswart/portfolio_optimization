// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using Stats = MathNet.Numerics.Statistics;


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

        public static double AnnualisedMean(this ITimeSeries<double> timeseries)
        {
            if (timeseries.IntegrationOrder == 0)
            {
                return Stats.Statistics.Mean(timeseries.Data) * timeseries.GetFrequency();
            }
            else
                return Stats.Statistics.Mean(timeseries.Returns<double>().Data) * timeseries.GetFrequency();
        }

        public static double AnnualisedStdDev(this ITimeSeries<double> timeseries)
        {
            if (timeseries.IntegrationOrder == 0)
            {
                return Stats.Statistics.Mean(timeseries.Data) * Math.Sqrt(timeseries.GetFrequency());
            }
            else
                return Stats.Statistics.StandardDeviation(timeseries.Returns<double>().Data) * Math.Sqrt(timeseries.GetFrequency());
        }

        public static ITimeSeries<T> Returns<T>(this ITimeSeries<double> timeseries, ReturnMethod method = ReturnMethod.Geometric)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> Cumulate<T>(this ITimeSeries<double> timeseries, double baseVal = 100)
        {
            throw new NotImplementedException();
        }

        public static IMultiTimeSeries<T> ColumnBind<T>(this ITimeSeries<T> timeseries, ITimeSeries<T> newtimeseries)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> RowBind<T>(this ITimeSeries<T> timeseries, ITimeSeries<T> newtimeseries)
        {
            throw new NotImplementedException();
        }

        public static IMultiTimeSeries<T> RowBind<T>(this IMultiTimeSeries<T> timeseries, IMultiTimeSeries<T> newtimeseries)
        {
            throw new NotImplementedException();
        }


        public static T First<T>(this ITimeSeries<T> timeseries)
        {
            throw new NotImplementedException();
        }

        public static T Last<T>(this ITimeSeries<T> timeseries)
        {
            throw new NotImplementedException();
        }

    }
}
