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
        /// <summary>
        /// Create a TimeSeries object from
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[,] data, string[] datevector, string[] names)
        {
            return new TimeSeries<T>(data, datevector, names);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[,] data, DateTime[] datevector, string[] names)
        {
            return new TimeSeries<T>(data, datevector, names);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[,] data, DateTime[] datevector)
        {
            int l = data.GetLength(1);
            string[] names = new string[l];
            for (int i = 0; i < l; i++)
            {
                names[i] = "series" + l;
            }
            return new TimeSeries<T>(data, datevector, names);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[,] data, string[] datevector)
        {
            int l = data.GetLength(1);
            string[] names = new string[l];
            for (int i = 0; i < l; i++)
            {
                names[i] = "series" + l;
            }
            return new TimeSeries<T>(data, datevector, names);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[] data, string[] datevector, string[] names)
        {
            var matdata = new T[data.GetLength(0),1];
            matdata = data.ToMatrix(0);
            return new TimeSeries<T>(matdata, datevector, names);
        }

        /// <summary>
        /// Create ITimeSeries from vector data series, , 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[] data, DateTime[] datevector, string[] names)
        {
            var matdata = new T[data.GetLength(0), 1];
            matdata = data.ToMatrix(0);
            return new TimeSeries<T>(matdata, datevector, names);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[] data, DateTime[] datevector, string name)
        {
            var matdata = new T[data.GetLength(0), 1];
            matdata = data.ToMatrix(0);
            return new TimeSeries<T>(matdata, datevector, new[]{ name });
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="date"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[] data, DateTime date, string[] names)
        {
            var matdata = new T[data.GetLength(0), 1];
            matdata = data.ToMatrix(0);
            return new TimeSeries<T>(matdata, new [] { date }, names);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[] data, DateTime[] datevector)
        {
            int l = data.GetLength(1);
            string[] names = new string[l];
            for (int i = 0; i < l; i++)
            {
                names[i] = "series" + l;
            }

            var matdata = new T[data.GetLength(0), 1];
            matdata = data.ToMatrix(0);
            return new TimeSeries<T>(matdata, datevector, names);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="datevector"></param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[] data, string[] datevector)
        {
            int l = data.GetLength(1);
            string[] names = new string[l];
            for (int i = 0; i < l; i++)
            {
                names[i] = "series" + l;
            }

            var matdata = new T[data.GetLength(0), 1];
            matdata = data.ToMatrix(0);
            return new TimeSeries<T>(matdata, datevector, names);
        }

        /// <summary>
        /// Create Empty TimeSeries
        /// </summary>
        /// <returns>Object that implements the ITimeSeries<typeparamref name="T"/></returns>
        public static ITimeSeries<T> Empty()
        {
            return new TimeSeries<T>();
        }

        public static ITimeSeries<T> Create(IEnumerable<IEnumerable<TimeSeriesItem<T>>> data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="frequency">Number of periods in year</param>
        /// <returns></returns>
        public static ITimeSeries<T> Create(T[] data, DataFrequency freq)
        {
            string[] names = { "series1" };
            int numperiods = data.GetLength(0);

            DateTime[] dates = new DateTime[numperiods];
            int a = 0;

            switch (freq)
            {
                case DataFrequency.Monthly:
                    {
                        for (int c = numperiods - 1; c > -1; c--)
                        {
                            dates[c] = DateTime.Today.AddMonths(-a);
                            a++;
                        }
                    }
                    break;

                case DataFrequency.Daily:
                    {
                        for (int c = numperiods - 1; c > -1; c--)
                        {
                            dates[c] = DateTime.Today.AddDays(-a);
                            a++;
                        }
                    }
                    break;

                default:
                    {
                        for (int c = numperiods - 1; c > -1; c--)
                        {
                            dates[c] = DateTime.Today.AddDays(-a);
                            a++;
                        }
                    }
                    break;

            }

            T[,] mat = new T[numperiods, 1];
            mat = data.ToMatrix();

            return new TimeSeries<T>(mat, dates, names);
        }

        /// <summary>
        /// Create a TimeSeries from a CSV file with a single row of headings on top and dates to the left, no spaces
        /// </summary>
        /// <param name="filename">Name of CSV file</param>
        /// <returns></returns>
        public static ITimeSeries<double> FromFile(string filename)
        {
            string line;
            string[] row;
            double[,] matdata;
            string[] names;
            List<string> dates = new List<string>();

            if (!File.Exists(filename))
                throw new FileNotFoundException();

            using (StreamReader readFile = new StreamReader(filename))
            {
                int r = 0;
                var heading = readFile.ReadLine().Split(',');
                names = new string[heading.GetLength(0) - 1];

                for (int i = 1; i < heading.GetLength(0); i++)
                {
                    names[i-1] = heading[i].TrimEnd('-').TrimEnd(' ').Replace('/', '_');
                }

                matdata = new double[1, heading.GetLength(0)-1];
                while ((line = readFile.ReadLine()) != null)
                {
                    row = line.Split(',');

                    int rowlength = row.GetLength(0);
                    double[] v = new double[rowlength-1];
                    
                    for (int i=1; i<rowlength; i++)
                    {
                        if (row[i] == "#N/A" || row[i] == string.Empty)
                            v[i - 1] = double.NaN;
                        else
                            v[i - 1] = Convert.ToDouble(row[i]);
                        
                    }
                    dates.Add(row[0]);
                    if (r == 0)
                        matdata = matdata.SetRow(v, r);
                    else
                        matdata = matdata.AddRow(v,r);
                    r++;
                }
            }

            return new TimeSeries<double>(matdata, dates.ToArray(), names);
        }

        public static class SampleData
        {
            public static int RandomSeed = 5; 

            public static class Gaussian
            {
                public static ITimeSeries<double> Create(double mu, double sigma, int numperiods = 100, int numseries = 1, DataFrequency freq = DataFrequency.Daily)
                {
                    var norm = new Normal(mu, sigma);
                    norm.RandomSource = new Random(RandomSeed);
                    var data = norm.Samples();

                    double[,] mat = new double[numperiods, numseries];

                    int stop = numperiods * numseries;
                    int row = 0;
                    int col = 0;
                    foreach (var d in data)
                    {
                        mat[row, col] = d;
                        row++;

                        if (row * (col + 1) >= stop)
                            break;

                        if (row == numperiods)
                        {
                            row = 0;
                            col++;
                        }
                    }
                    string[] names = new string[numseries];

                    for (int i = 0; i < numseries; i++)
                    {
                        names[i] = "series" + (i + 1);
                    }

                    DateTime[] dates = new DateTime[numperiods];
                    int a = 0;

                    switch (freq)
                    {

                        case DataFrequency.Daily:
                            {
                                for (int c = numperiods - 1; c > -1; c--)
                                {
                                    dates[c] = DateTime.Today.AddDays(-a);
                                    a++;
                                }
                            }
                            break;

                        case DataFrequency.Monthly:
                            {
                                for (int c = numperiods - 1; c > -1; c--)
                                {
                                    dates[c] = DateTime.Today.AddMonths(-a);
                                    a++;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    return new TimeSeries<double>(mat, dates, names);
                }
            }
        }
    }
}
