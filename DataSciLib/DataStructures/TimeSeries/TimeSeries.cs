// Copyright (c) 2012: DJ Swart, AJ Hoffman
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public class TimeSeries<T> : ITimeSeries<T>
    {
        #region Internal storage mechanism
        // Hashtable type data storage for quick searching, automatically sorted
        protected SortedList<DateTime, T> _timeDataDict;

        #endregion

        #region Indexers
        public T this[DateTime date]
        {
            get
            {
                return _timeDataDict[date];
            }
        }

        public ITimeSeries<T> this[IEnumerable<DateTime> dates]
        {
            get
            {
                throw new NotImplementedException();
            }
            private set { }
        }
        #endregion

        #region Public Properties
        // Convenience methods for retrieving the data from internal structures
        public DateTime[] DateTime
        {
            get
            {
                return _timeDataDict.Keys.ToArray();
            }
        }

        // Column major format
        public T[] Data
        {
            get
            {
                return _timeDataDict.Values.ToArray();
            }
        }

        public string Name
        {
            get;
            private set;
        }

        public int IntegrationOrder
        {
            get;
            private set;
        }

        public int RowCount
        {
            get { return _timeDataDict.Count; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public TimeSeries()
        {
            Name = "Series1";

            _timeDataDict = new SortedList<DateTime, T>();
        }

        public TimeSeries(IEnumerable<TimeDataPoint<T>> datapoints, string name)
        {
            _timeDataDict = new SortedList<DateTime, T>();

            foreach (var d in datapoints)
            {
                _timeDataDict.Add(d.Date, d.Value);
            }
        }
        
        #endregion
        
        public ITimeSeries<T> Exclude(DateTime date)
        {
            throw new NotImplementedException();
        }

        public ITimeSeries<T> Exclude(DateTime[] dates)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TimeDataPoint<T>> GetEnumerator()
        {
            foreach (var td in _timeDataDict)
            {
                yield return new TimeDataPoint<T>(td.Key, td.Value);
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

    /// <summary>
    /// Non-generic implementation of TimeSeries of type double
    /// </summary>
    public sealed class TimeSeries : TimeSeries<double>
    {
        public TimeSeries(IEnumerable<TimeDataPoint<double>> datapoints, string name)
        {
            _timeDataDict = new SortedList<DateTime, double>();

            foreach (var d in datapoints)
            {
                _timeDataDict.Add(d.Date, d.Value);
            }
        }

        // Operator overloads
        public static TimeSeries operator +(TimeSeries ts1, TimeSeries ts2)
        {
            //add corresponding elements that match on time
            List<TimeDataPoint<double>> newts = new List<TimeDataPoint<double>>();
            foreach (var td in ts1)
            {
                var sum = td.Value + ts2[td.Date];
                newts.Add(new TimeDataPoint<double>(td.Date, sum));
            }

            return new TimeSeries(newts, "Sum of " + ts1.Name + " and " + ts2.Name);
        }

        public static TimeSeries operator -(TimeSeries ts1, TimeSeries ts2)
        {
            //add corresponding elements that match on time
            List<TimeDataPoint<double>> newts = new List<TimeDataPoint<double>>();
            foreach (var td in ts1)
            {
                var answ = td.Value - ts2[td.Date];
                newts.Add(new TimeDataPoint<double>(td.Date, answ));
            }

            return new TimeSeries(newts, "Difference between " + ts1.Name + " and " + ts2.Name);
        }

        public static TimeSeries operator *(TimeSeries ts1, double multiplier)
        {
            //add corresponding elements that match on time
            List<TimeDataPoint<double>> newts = new List<TimeDataPoint<double>>();
            foreach (var td in ts1)
            {
                var answ = td.Value * multiplier;
                newts.Add(new TimeDataPoint<double>(td.Date, answ));
            }

            return new TimeSeries(newts, ts1.Name);
        }

        public static TimeSeries operator /(TimeSeries ts1, double div)
        {
            //add corresponding elements that match on time
            List<TimeDataPoint<double>> newts = new List<TimeDataPoint<double>>();
            foreach (var td in ts1)
            {
                var answ = td.Value / div;
                newts.Add(new TimeDataPoint<double>(td.Date, answ));
            }

            return new TimeSeries(newts, ts1.Name);
        }
    }

}
