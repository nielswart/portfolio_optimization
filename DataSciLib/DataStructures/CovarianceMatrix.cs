using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.IO;
using System.IO;
using System;
using MathNet.Numerics.Distributions;
using System.Collections.Generic;
using LINQtoCSV;
using System.Linq;

namespace DataSciLib.DataStructures
{
    public class CovarianceMatrix : DenseMatrix
    {
        private Dictionary<string, Dictionary<string, double>> _storage;

        public Dictionary<string, double> this[string key]
        {
            get { return _storage[key]; }
        }

        public IEnumerable<string> Keys
        {
            get { return _storage.Keys; }
        }

        private CovarianceMatrix(Dictionary<string, Dictionary<string, double>> dict, double[,] matrix)
            : base(matrix)
        {
            _storage = dict;
        }

        public static CovarianceMatrix Create(Dictionary<string, Dictionary<string, double>> varCov)
        {
            int numvars = varCov.Keys.Count;
            double[,] matrix = new double[numvars, numvars];

            int r = 0, c = 0;
            foreach (var key in varCov.Keys)
            {
                foreach (var k in varCov[key].Keys)
                {
                    matrix[r, c] = varCov[key][k];
                    r++;
                }
                c++;
                r = 0;
            }

            return new CovarianceMatrix(varCov, matrix);
        }

        internal class CsvDataRow : List<DataRowItem>, IDataRow
        {

        }

        /// <summary>
        /// Create a covariance matrix from a delimited file
        /// </summary>
        /// <param name="filename">The name of the file from which to read</param>
        /// <param name="delimiter">The delimiter to use - default ','</param>
        /// <returns></returns>
        public static CovarianceMatrix Create(string filename, char delimiter = ',')
        {
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = delimiter, // comma delimited
                FirstLineHasColumnNames = false // column names in first record
            };

            CsvContext cc = new CsvContext();
            var cov = cc.Read<CsvDataRow>(filename, inputFileDescription);

            var heading = from line in cov
                          from item in line
                          where item.LineNbr == 1
                          select item.Value;

            var tickers = from t in heading.Skip(1)
                          select t;

            var covdict = new Dictionary<string, Dictionary<string, double>>();
            foreach (var row in cov.Skip(1))
            {
                var ticker = row.First().Value;
                var values = row.Skip(1);

                int count = 0;
                var tempdict = new Dictionary<string, double>();
                foreach (var v in values)
                {
                    tempdict.Add(tickers.ToArray()[count], Convert.ToDouble(v.Value));
                    count++;
                }

                covdict.Add(ticker, tempdict);
            }

            return CovarianceMatrix.Create(covdict);
        }
    }

    public static class CovarianceExtensions
    {
        public static double[,] ToCorrelation(this CovarianceMatrix covariancematrix)
        {
            var normalized = new double[covariancematrix.RowCount, covariancematrix.ColumnCount];
            for (int r = 0; r < covariancematrix.RowCount; r++)
            {
                for (int c = 0; c < covariancematrix.ColumnCount; c++)
                {
                    normalized[r, c] = covariancematrix[r, c] / (System.Math.Sqrt(covariancematrix[r, r]) * System.Math.Sqrt(covariancematrix[c, c]));
                }
            }
            return normalized;
        } 
    }

}
