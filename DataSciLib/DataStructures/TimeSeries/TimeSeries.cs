// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace DataSciLib.DataStructures
{
    // TODO: Make IEnumerable - iterate through dataset by date/time 
    // Symbol based indexer for columns - e.g. TimeSeries["ASA"] to select ASA time series && multiple column indexing e.g. TimeSeries[new []{"ASA", "SOL"}]
    // DateTime indexer for rows - e.g. TimeSeries[Date]
    // implement window functions etc..
    // LINQ - window function as LINQ extension etc

    /// <summary>
    /// 
    /// </summary>
    public sealed class TimeSeries<T> : ITimeSeries<T>, IEnumerable<TimeItem<T>> 
    {
        #region Internal storage mechanism

        private Dictionary<DateTime, T[]> _dateValueDict;
        private Dictionary<string, T[]> _nameValueDict;

        #endregion

        #region Indexers
        public ITimeSeries<T> this[string name]
        {
            get
            {
                return TimeSeriesFactory<T>.Create(_nameValueDict[name], this.DateTime, name);
            }
            private set { }
        }

        public ITimeSeries<T> this[string[] names]
        {
            get
            {
                throw new NotImplementedException();
            }
            private set
            {

            }
        }

        public ITimeSeries<T> this[DateTime date]
        {
            get
            {
                return TimeSeriesFactory<T>.Create(_dateValueDict[date], date, this.Names);
            }
            private set { }
        }

        public ITimeSeries<T> this[DateTime[] dates]
        {
            get
            {
                throw new NotImplementedException();
            }
            private set { }
        }

        public T this[DateTime date, string name]
        {
            get
            {
                return TimeSeriesFactory<T>.Create(_dateValueDict[date], date, this.Names)[name].DataMatrix[0, 0];
            }
            private set { }
        }

        public ITimeSeries<T> this[DateTime[] dates, string[] names]
        {
            get
            {
                throw new NotImplementedException();
            }
            private set { }
        }

        #endregion

        #region Public Properties

        public DateTime[] DateTime
        { get; private set; }

        public T[,] DataMatrix
        { get; private set; }

        public string[] Names
        {
            get;
            private set;
        }

        public int RowCount
        {
            get { return DateTime.Length; }
        }

        public int ColumnCount
        {
            get { return Names.Length; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public TimeSeries()
        {
            DataMatrix = new T[,] { { }, { } };
            DateTime = new DateTime[] {};
            Names = new string[] {};

            _dateValueDict = new Dictionary<DateTime, T[]>();
            _nameValueDict = new Dictionary<string, T[]>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timevector"></param>
        public TimeSeries(T[,] data, string[] timevector, string[] names)
        {
            // Check to see if date and data dimensions agree
            if (data.GetLength(0) == timevector.GetLength(0))
            {
                DataMatrix = data;
                // Initialize with correct size
                DateTime = new DateTime[timevector.GetLength(0)];

                _dateValueDict = new Dictionary<DateTime, T[]>();
                _nameValueDict = new Dictionary<string, T[]>();
                int dc = 0;

                foreach (string s in timevector)
                {
                    var date = ConvertToDate(s);
                    DateTime[dc] = date;

                    T[] vectc = new T[data.GetLength(1)];
                    vectc = data.GetRow(dc);
                    _dateValueDict.Add(date, vectc);
                    dc++;
                }
                dc = 0;
                foreach (var n in names)
                {
                    T[] vectr = new T[data.GetLength(0)];
                    vectr = data.GetColumn(dc);
                    _nameValueDict.Add(n, vectr);
                    dc++;
                }

                Names = names;
            }
            else
                throw new ArgumentException("Data and date dimensions do not agree");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timevector"></param>
        /// <param name="names"></param>
        public TimeSeries(T[,] data, DateTime[] timevector, string[] names)
        {
            // Check to see if date and data dimensions agree
            if (data.GetLength(0) == timevector.GetLength(0))
            {
                DataMatrix = data;
                DateTime = timevector;

                _dateValueDict = new Dictionary<DateTime, T[]>();
                _nameValueDict = new Dictionary<string, T[]>();
                int dc = 0;

                foreach (var d in timevector)
                {
                    T[] vectc = new T[data.GetLength(1)];
                    for (int i = 0; i < data.GetLength(1); i++)
                    {
                        vectc[i] = data[dc, i];
                    }
                    _dateValueDict.Add(d, vectc);
                    dc++;
                }
                dc = 0;
                foreach (var n in names)
                {
                    T[] vectr = new T[data.GetLength(0)];
                    for (int r = 0; r < data.GetLength(0); r++)
                    {
                        vectr[r] = data[r, dc];
                    }
                    _nameValueDict.Add(n, vectr);
                    dc++;
                }

                Names = names;
            }
            else
                throw new ArgumentException("Data and date dimensions do not agree");
        }

        #endregion

        public void RowBind(ITimeSeries<T> newtimeseries)
        {
            throw new NotImplementedException();
        }

        public void ColumnBind(ITimeSeries<T> newtimeseries)
        {
            throw new NotImplementedException();
        }

        public ITimeSeries<T> Exclude(string name)
        {
            throw new NotImplementedException();
        }

        public ITimeSeries<T> Exclude(string[] names)
        {
            throw new NotImplementedException();
        }

        public ITimeSeries<T> Exclude(DateTime date)
        {
            throw new NotImplementedException();
        }

        public ITimeSeries<T> Exclude(DateTime[] dates)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TimeSeriesItem<T>> GetEnumerator()
        {
            int rows = DataMatrix.GetLength(0);
            int cols = DataMatrix.GetLength(1);

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; c++)
                {
                    yield return new TimeSeriesItem<T>(this.DateTime[r], this.Names[c], DataMatrix[r, c]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Helper functions

        private static DateTime ConvertToDate(string date)
        {
            var year = Convert.ToInt32(date.Substring(0, 4));
            var month = Convert.ToInt32(date.Substring(5, 2));
            var day = Convert.ToInt32(date.Substring(8, 2));

            return new DateTime(year, month, day);
        }
        #endregion
    }
}
