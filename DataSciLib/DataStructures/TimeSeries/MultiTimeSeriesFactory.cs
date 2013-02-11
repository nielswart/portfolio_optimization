using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataSciLib.DataStructures
{
    public class MultiTimeSeriesFactory<T>
    {
        /*
        public static IMultiTimeSeries<T> Create(T[,] data, string[] datevector, string[] names)
        {
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
            var matdata = new T[data.GetLength(0), 1];
            matdata = data.ToMatrix(0);
            return new TimeSeries<T>(matdata, datevector, names);
        }
         * */

        /*
        public static IMultiTimeSeries<double> FromFile(string filename)
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
                    names[i - 1] = heading[i].TrimEnd('-').TrimEnd(' ').Replace('/', '_');
                }

                matdata = new double[1, heading.GetLength(0) - 1];
                while ((line = readFile.ReadLine()) != null)
                {
                    row = line.Split(',');

                    int rowlength = row.GetLength(0);
                    double[] v = new double[rowlength - 1];

                    for (int i = 1; i < rowlength; i++)
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
                        matdata = matdata.AddRow(v, r);
                    r++;
                }
            }

            return newTimeSeries<double>(matdata, dates.ToArray(), names);
        }
         * */
    }
}
