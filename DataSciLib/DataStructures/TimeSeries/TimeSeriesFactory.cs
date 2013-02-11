// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.Distributions;
using MathNet;
using MathNet.Numerics;

namespace DataSciLib.DataStructures
{
    public class TimeSeriesFactory<T>
    {
        public static ITimeSeries<T> Create(List<TimeDataPoint<T>> timeseries, string name)
        {
            return new TimeSeries<T>(timeseries, name);
        }

        public static ITimeSeries<T> Create(T[] data, DateTime[] datevector, string name)
        {
            if (data.GetLength(0) != datevector.GetLength(0))
                throw new ArgumentException("The time and data series are not of the same length");
            else
            {
                int c = 0;
                List<TimeDataPoint<T>> timeseries = new List<TimeDataPoint<T>>();
                foreach (var d in data)
                {
                    timeseries.Add(new TimeDataPoint<T>(datevector[c], d));
                    c++;
                }
                return new TimeSeries<T>(timeseries, name);
            }
        }

        public static ITimeSeries<T> Create(T[] data, DateTime[] datevector)
        {
            if (data.GetLength(0) != datevector.GetLength(0))
                throw new ArgumentException("The time and data series are not of the same length");
            else
            {
                int c = 0;
                List<TimeDataPoint<T>> timeseries = new List<TimeDataPoint<T>>();
                foreach (var d in data)
                {
                    timeseries.Add(new TimeDataPoint<T>(datevector[c], d));
                    c++;
                }
                return new TimeSeries<T>(timeseries, "Series1");
            }
        }    

        public static ITimeSeries<T> Empty()
        {
            return new TimeSeries<T>();
        }

        public static ITimeSeries<T> Create(IEnumerable<TimeDataPoint<T>> datapoints)
        {
            return new TimeSeries<T>(datapoints, "Series1");
        }
        
        public static ITimeSeries<T> Create(T[] data, DataFrequency freq)
        {
            string name = "series1";
            int numperiods = data.GetLength(0);
            List<TimeDataPoint<T>> datapoints = new List<TimeDataPoint<T>>();

            switch (freq)
            {
                case DataFrequency.Monthly:
                    {
                        for (int a = 0; a < numperiods; a++)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddMonths(-a), data[a]));
                        }
                    }
                    break;

                case DataFrequency.Daily:
                    {
                        for (int a = 0; a < numperiods; a++)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddDays(-a), data[a]));
                        }
                    }
                    break;

                default:
                    {
                        for (int a = 0; a < numperiods; a++)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddDays(-a), data[a]));
                        }
                    }
                    break;
            }

            return new TimeSeries<T>(datapoints, name);
        }

        public static class SampleData
        {
            public static int RandomSeed = 5; 

            public static class Gaussian
            {
                public static ITimeSeries<double> Create(double mu, double sigma, int numperiods = 100, DataFrequency freq = DataFrequency.Daily)
                {
                    var norm = new Normal(mu, sigma);
                    norm.RandomSource = new Random(RandomSeed);
                    var data = norm.Samples();

                    double[] tsdata = new double[numperiods];

                    int stop = numperiods;
                    int row = 0;
                    foreach (var d in data)
                    {
                        tsdata[row] = d;
                        row++;

                        if (row >= stop)
                            break;
                    }
                    return TimeSeriesFactory<double>.Create(tsdata, freq);
                }
            }
        }
    }
}
