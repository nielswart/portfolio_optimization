// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;

namespace DataSciLib.DataStructures
{
    public static class ArrayExtensions
    {
        public static double[] Seq(this double[] vector, double from, double to, int num)
        {
            vector = new double[num];
            double step = (to - from)/num;
            vector[0] = from;
            for (int i=1; i<num-1; i++)
            {
                vector[i] = vector[i - 1] + step;
            }

            vector[num - 1] = to;
            return vector;
        }

        public static double[] Seq(this double[] vector, double from, double to, uint num)
        {
            vector = new double[num];
            double step = (to - from) / num;
            vector[0] = from;
            for (int i = 1; i < num - 1; i++)
            {
                vector[i] = vector[i - 1] + step;
            }

            vector[num - 1] = to;
            return vector;
        }

        public static double[] Seq(this double[] vector, double from, double to, double by)
        {
            throw new NotImplementedException();
        }

        //public static double[] Round

        public static double[] Ones(this double[] vector, int num)
        {
            vector = new double[num];
            for (int i=0; i<num; i++)
            {
                vector[i] = 1;
            }
            return vector;
        }

        public static double[] Zeros(this double[] vector, int num)
        {
            vector = new double[num];
            for (int i = 0; i < num; i++)
            {
                vector[i] = 0;
            }
            return vector;
        }

        public static double[,] I(this double[,] mat,  int num)
        {
            mat = new double[num,num];
            for (int i = 0; i < num; i++)
            {
                mat[i,i] = 1;
            }
            return mat;
        }

        public static double Min(this double[] vector)
        {
            return vector.Minimum();
        }

        public static double Max(this double[] vector)
        {
            return vector.Maximum();
        }

        public static double[] Range(this double[] vector)
        {
            return new double[] { vector.Min(), vector.Max() };
        }


        /// <summary>
        /// Convert a one dimensional array (vector) to a multi-dimensional array (matrix)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="dimension">Specify the dimension of the matrix to which the vector should be copied (0 - row; 1 - column) </param>
        /// <returns>Mulit-dimensional array (matrix) with same total length as original array</returns>
        public static T[,] ToMatrix<T>(this T[] data, int dimension = 0)
        {           
            if (dimension > 1 || dimension < 0)
                throw new ArgumentException("Incorrect dimension specified. Please provide a zero based value no larger than the output matrix' dimensions.");

            T[,] matdata;
           
            if (dimension == 0)
            {
                matdata = new T[data.GetLength(0), 1];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    matdata[i, 0] = data[i];
                }
            }
            else
            {
                matdata = new T[1, data.GetLength(0)];
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    matdata[0, i] = data[i];
                }
            }

            return matdata;
        }

        /// <summary>
        /// Creates a shallow copy of a two dimensional array (matrix)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="outdata"></param>
        public static void Copy<T>(this T[,] data, T[,] outdata)
        {
            if (outdata.GetLength(0) < data.GetLength(0) || outdata.GetLength(0) < data.GetLength(0))
                throw new ArgumentException("Matrix dimensions do not agree; Destination array must be the same size or larger than source array.");

            for (int c = 0; c < data.GetLength(1); c++)
            {
                for (int r = 0; r < data.GetLength(0); r++)
                {
                    outdata[r, c] = data[r, c];
                }
            }
        }

        public static T[,] AddRow<T>(this T[,] data, T[] newrow, int rownum=-1)
        {
            if (data.GetLength(1) != newrow.GetLength(0))
                throw new ArgumentException("Matrix dimensions do not agree; Number of columns must be equal.");

            T[,] matrix = new T[data.GetLength(0)+1, data.GetLength(1)];
            data.Copy(matrix);

            if (rownum == -1)
            {
                // Add new row after last row
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    matrix[data.GetLength(0), i] = newrow[i];
                }
            }
            else
            {
                if (rownum < data.GetLength(0))
                    throw new ArgumentException("Invalid row number specified.  Data will be overwritten.");

                for (int i = 0; i < data.GetLength(1); i++)
                {
                    matrix[rownum, i] = newrow[i];
                }
            }
            return matrix;
        }

        public static T[,] AddColumn<T>(this T[,] data, T[] newcol, int colnum = -1)
        {
            if (data.GetLength(0) != newcol.GetLength(0) )
                throw new ArgumentException("Matrix dimensions do not agree; Number of rows must be equal");

            T[,] matrix = new T[data.GetLength(0) + 1, data.GetLength(1)];
            data.Copy(matrix);

            if (colnum == -1)
            {
                // Add new row after last row
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    matrix[i, data.GetLength(0)] = newcol[i];
                }
            }
            else
            {
                if (colnum < data.GetLength(0))
                    throw new ArgumentException("Invalid row number specified.  Data will be overwritten.");

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    matrix[i, colnum] = newcol[i];
                }
            }
            return matrix;
        }

        public static T[,] SetRow<T>(this T[,] data, T[] newrow, int rownum)
        {
            if (data.GetLength(1) != newrow.GetLength(0))
                throw new ArgumentException("Matrix dimensions do not agree; Number of columns must be equal.");

            if (rownum > data.GetLength(0))
                throw new ArgumentOutOfRangeException("Row number does not exist");
            else
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    data[rownum, i] = newrow[i];
                }
          
            return data;
        }

        public static T[,] SetColumn<T>(this T[,] data, T[] newcol, int colnum)
        {
            if (data.GetLength(0) != newcol.GetLength(0))
                throw new ArgumentException("Matrix dimensions do not agree; Number of rows must be equal.");

            if (colnum > data.GetLength(1))
                throw new ArgumentOutOfRangeException("Column number does not exist.");
            else
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    data[i, colnum] = newcol[i];
                }

            return data;
        }

        public static T[] GetRow<T>(this T[,] data, int rownum)
        {
            T[] vectr = new T[data.GetLength(1)];
            for (int col = 0; col < data.GetLength(1); col++)
            {
                vectr[col] = data[rownum, col];
            }

            return vectr;
        }

        public static T[] GetColumn<T>(this T[,] data, int colnum)
        {
            T[] vectr = new T[data.GetLength(0)];
            for (int row = 0; row < data.GetLength(0); row++)
            {
                vectr[row] = data[row, colnum];
            }

            return vectr;
        }

        public static string Print(this double[,] matrix)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    sb.Append(matrix[i, j].ToString("F2")).Append(" ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string Print(this double[] vector)
        {
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < vector.GetLength(0); j++)
            {
                sb.Append(vector[j].ToString("F2")).Append(" ");
            }
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
