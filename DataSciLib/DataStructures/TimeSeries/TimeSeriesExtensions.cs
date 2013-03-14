// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using Stats = MathNet.Numerics.Statistics;
using System.Collections.Generic;
using System.Linq;

namespace DataSciLib.DataStructures
{
    public static class TimeSeriesExtensions
    {
        public static int PeriodsInYear<T>(this ITimeSeries<T> timeseries)
        {
            switch (timeseries.Frequency)
            {
                case DataFrequency.Daily:
                    return 252;
                case DataFrequency.Weekly:
                    return 52;
                case DataFrequency.Monthly:
                    return 12;
                case DataFrequency.Quarterly:
                    return 4;
                default:
                    {
                        // return 252 if frequency daily; 12 monhtly; 4 quarterly
                        var numvals = timeseries.DateTime.GetLength(0);
                        var diff = (timeseries.DateTime[numvals - 1] - timeseries.DateTime[0]).Days / (double)numvals;
                        var avgtimespan = 252.0 / diff;

                        if (avgtimespan > 0.5 && avgtimespan < 1.5)
                            return 252;
                        if (avgtimespan > 4 && avgtimespan < 9)
                            return 52;
                        if (avgtimespan > 19 && avgtimespan < 32)
                            return 12;
                        if (avgtimespan > 50 && avgtimespan < 95)
                            return 4;
                        else
                            return 1;
                    }
            }
        }

        public static double AnnualisedMean(this ITimeSeries<double> timeseries)
        {
            return Stats.Statistics.Mean(timeseries.Data) * timeseries.PeriodsInYear();
        }

        public static double AnnualisedStdDev(this ITimeSeries<double> timeseries)
        {
            return Stats.Statistics.StandardDeviation(timeseries.Data) * Math.Sqrt(timeseries.PeriodsInYear());
        }

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

        public static ITimeSeries<T> Cumulate<T>(this ITimeSeries<double> timeseries, double baseVal = 100)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<T> AddRows<T>(this ITimeSeries<T> timeseries, ITimeSeries<T> newtimeseries)
        {
            throw new NotImplementedException();
        }

        public static ITimeSeries<double> FirstDifference(this ITimeSeries<double> timeseries, ReturnMethod method = ReturnMethod.Geometric)
        {
            if (method == ReturnMethod.Arithmetic)
            {
                var res = from cur in timeseries.Skip(1)
                          select new TSDataPoint<double>(cur.Date, cur.Value / timeseries[(from prev in timeseries where prev.Date < cur.Date select prev.Date).Max()] -1);

                return TimeSeriesFactory<double>.Create(res, timeseries.Name, timeseries.IntegrationOrder -1, timeseries.Frequency);
            }
            else
            {
                var res = from cur in timeseries.Skip(1)
                          select new TSDataPoint<double>(cur.Date, Math.Log(cur.Value / timeseries[(from prev in timeseries where prev.Date < cur.Date select prev.Date).Max()]));

                return TimeSeriesFactory<double>.Create(res, timeseries.Name, timeseries.IntegrationOrder - 1, timeseries.Frequency);
            }
        }

        public static ITimeSeries<double> CumSum(this ITimeSeries<double> timeseries)
        {
            throw new NotImplementedException();
        }

        public static IMultiTimeSeries<T> AddColumns<T>(this ITimeSeries<T> timeseries, ITimeSeries<T> newtimeseries)
        {
            throw new NotImplementedException();
        }

        public static IMultiTimeSeries<T> AddRows<T>(this IMultiTimeSeries<T> timeseries, IMultiTimeSeries<T> newtimeseries)
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

        public static TimeSeries AsTimeSeries(this ITimeSeries<double> timeseries)
        {
            return new TimeSeries(timeseries.AsEnumerable(), timeseries.Name, timeseries.IntegrationOrder, timeseries.Frequency);
        }

    }
}
