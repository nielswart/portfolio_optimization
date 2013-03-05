using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.IO;
using System.IO;
using System;
using MathNet.Numerics.Distributions;
using System.Collections.Generic;

namespace DataSciLib.DataStructures
{
    public class CovarianceMatrix : DenseMatrix
    {
        public CovarianceMatrix(double[,] matrix)
            : base(matrix)
        {
            
        }

        /// <summary>
        /// Create a covariance matrix from a delimited file
        /// </summary>
        /// <param name="filename">The name of the file from which to read</param>
        /// <param name="delimiter">The delimiter to use - default ','</param>
        /// <returns></returns>
        public static CovarianceMatrix CreateFromDelimitedFile(string filename, char delimiter = ',')
        {
            DelimitedReader<DenseMatrix, double> reader = new DelimitedReader<DenseMatrix, double>(delimiter);

            if (reader == null)
                throw new System.NullReferenceException();

            if (File.Exists(filename))
                return (CovarianceMatrix)reader.ReadMatrix(filename);
            else
                throw new FileNotFoundException();
        }

        public static CovarianceMatrix Create(Dictionary<string, Dictionary<string, double>> varCov)
        {
            int numvars = varCov.Keys.Count;
            double[,] matrix = new double[numvars, numvars];

            int r=0,c =0;
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

            return new CovarianceMatrix(matrix);
        }

        /// <summary>
        /// NOT YET IMPLEMENTED
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stddev"></param>
        /// <param name="distribution"></param>
        /// <returns></returns>
        public static CovarianceMatrix CreateRandom(double mean, double stddev, IContinuousDistribution distribution)
        {
            throw new NotImplementedException();
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
