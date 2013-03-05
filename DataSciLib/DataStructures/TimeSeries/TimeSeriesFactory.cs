// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.Distributions;
using MathNet;
using MathNet.Numerics;
using System.Linq;

namespace DataSciLib.DataStructures
{
    public enum Rebalance
    {
        Never,
        Continuous,
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        SemiAnnually,
        Annually
    }

    public class TimeSeriesFactory<T>
    {
        public static ITimeSeries<T> Create(IEnumerable<TimeDataPoint<T>> timeseries, string name, uint integrationOrder = 1, DataFrequency frequency = DataFrequency.Daily)
        {
            return new TimeSeries<T>(timeseries, name, integrationOrder, frequency);
        }
        
        public static ITimeSeries<T> Create(IEnumerable<TimeDataPoint<T>> timeseries, uint integrationOrder= 1, DataFrequency frequency = DataFrequency.Daily)
        {
            return new TimeSeries<T>(timeseries, "Series1", integrationOrder, frequency);
        }

        public static ITimeSeries<T> Create(IEnumerable<TimeDataPoint<T>> timeseries, string name)
        {
            return new TimeSeries<T>(timeseries, name, 1);
        }

        public static ITimeSeries<T> Create(IEnumerable<TimeDataPoint<T>> timeseries, uint integrationOrder)
        {
            return new TimeSeries<T>(timeseries, "Series1", integrationOrder);
        }

        public static ITimeSeries<T> Create(IEnumerable<T> data, uint integrationOrder, DataFrequency freq)
        {
            string name = "series1";
            List<TimeDataPoint<T>> datapoints = new List<TimeDataPoint<T>>();

            switch (freq)
            {
                case DataFrequency.Monthly:
                    {
                        int count = 0;
                        foreach (var d in data)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddMonths(-count), d));
                            count++;
                        }
                    }
                    break;

                case DataFrequency.Daily:
                    {
                        int count = 0;
                        foreach (var d in data)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddDays(-count), d));
                            count++;
                        }
                    }
                    break;

                default:
                    {
                        int count = 0;
                        foreach (var d in data)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddDays(-count), d));
                            count++;
                        }
                    }
                    break;
            }

            return new TimeSeries<T>(datapoints, name, integrationOrder, freq);
        }

        /// <summary>
        /// Create a single variable time series object with a specific length, given an enumerable sequance with 'infinite' length
        /// </summary>
        /// <param name="data">Infinite enumerable sequence</param>
        /// <param name="integrationOrder">The time series integration order, in finance this translates to 0 for returns series and 1 for a price series</param>
        /// <param name="numperiods">The length (number of time periods) of the time series, takes the first n samples from the enumerable sequence</param>
        /// <param name="freq">The frequency of the data, i.e. Daily, Weekly, Monthly etc</param>
        /// <returns>A single variable time series object that implements the generic ITimeSeries interface</returns>
        public static ITimeSeries<T> Create(IEnumerable<T> data, uint integrationOrder, int numperiods, DataFrequency freq)
        {
            string name = "series1";
            List<TimeDataPoint<T>> datapoints = new List<TimeDataPoint<T>>();

            switch (freq)
            {
                case DataFrequency.Monthly:
                    {
                        int count = 0;
                        // Use TakeWhile ??
                        foreach (var d in data)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddMonths(-count), d));
                            count++;
                            if (count >= numperiods)
                                break;
                        }
                    }
                    break;

                case DataFrequency.Daily:
                    {
                        int count = 0;
                        foreach (var d in data)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddDays(-count), d));
                            count++;
                            if (count >= numperiods)
                                break;
                        }
                    }
                    break;

                default:
                    {
                        int count = 0;
                        foreach (var d in data)
                        {
                            datapoints.Add(new TimeDataPoint<T>(DateTime.Today.AddDays(-count), d));
                            count++;
                            if (count >= numperiods)
                                break;
                        }
                    }
                    break;
            }

            return new TimeSeries<T>(datapoints, name, integrationOrder, freq);
        }

        public static ITimeSeries<T> Create(T[] data, DateTime[] datevector, string name, uint integrationOrder)
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
                return new TimeSeries<T>(timeseries, name, integrationOrder);
            }
        }

        public static ITimeSeries<T> Empty()
        {
            return new TimeSeries<T>();
        }

        public static class SampleData
        {
            public static int RandomSeed = 5;

            public static ITimeSeries<double> Gaussian(double mu, double sigma, int numperiods = 100, DataFrequency freq = DataFrequency.Daily)
            {
                var norm = new Normal(mu, sigma);
                norm.RandomSource = new Random(RandomSeed);
                var data = norm.Samples();

                return TimeSeriesFactory<double>.Create(data, 0, numperiods, freq);
            }

            public static ITimeSeries<double> RandomWalk(double drift, double stepsigma, int numsteps = 100, DataFrequency freq = DataFrequency.Daily)
            {
                var norm = new Normal(drift, stepsigma);
                norm.RandomSource = new Random(RandomSeed);
                var data = norm.Samples();

                var walk = new List<double>();
                var last = 1.0;
                walk.Add(last);

                int count = 0;
                // Use TakeWhile ??
                foreach (var d in data)
                {
                    last = last + d;
                    walk.Add(last);
                    count++;
                    if (count >= numsteps)
                        break;
                }

                return TimeSeriesFactory<double>.Create(walk, integrationOrder: 1, freq: freq);
            }
        }
    }
}
