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
                var ts = from td in _timeDataDict
                         join d in dates on td.Key equals d
                         select new TSDataPoint<T>(td.Key, td.Value);

                return new TimeSeries<T>(ts, this.Name, this.IntegrationOrder);
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

        public uint IntegrationOrder
        {
            get;
            private set;
        }

        public int RowCount
        {
            get { return _timeDataDict.Count; }
        }

        public DataFrequency Frequency { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public TimeSeries()
        {
            Name = "Series1";
            IntegrationOrder = 1;
            Frequency = DataFrequency.Daily;
            _timeDataDict = new SortedList<DateTime, T>();
        }

        public TimeSeries(IEnumerable<TSDataPoint<T>> datapoints, string name, uint integrationOrder, DataFrequency freq = DataFrequency.Daily)
        {
            Name = "Series1";
            IntegrationOrder = integrationOrder;
            Frequency = freq;
            _timeDataDict = new SortedList<DateTime, T>();

            foreach (var d in datapoints)
            {
                _timeDataDict.Add(d.Date, d.Value);
            }
        }
        
        #endregion
        
        /*
        public ITimeSeries<T> Exclude(DateTime date)
        {
            throw new NotImplementedException();
        }

        public ITimeSeries<T> Exclude(DateTime[] dates)
        {
            throw new NotImplementedException();
        }
        */
        public IEnumerator<TSDataPoint<T>> GetEnumerator()
        {
            foreach (var td in _timeDataDict)
            {
                yield return new TSDataPoint<T>(td.Key, td.Value);
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
    /// Non-generic implementation of TimeSeries
    /// </summary>
    public sealed class TimeSeries : TimeSeries<double>
    {
        // TODO: do it all with LINQ - lazy evaluation 
        public TimeSeries(IEnumerable<TSDataPoint<double>> datapoints, string name, uint integrationOrder)
            : base(datapoints, name, integrationOrder)
        {
           
        }

        public TimeSeries(IEnumerable<TSDataPoint<double>> datapoints, string name, uint integrationOrder, DataFrequency freq)
            : base(datapoints, name, integrationOrder, freq)
        {
            
        }

        // Operator overloads
        public static TimeSeries operator +(TimeSeries ts1, TimeSeries ts2)
        {
            var addts = from a in ts1
                        select new TSDataPoint<double>(a.Date, a.Value + ts2[a.Date]);

            return new TimeSeries(addts, "Sum of " + ts1.Name + " and " + ts2.Name, ts1.IntegrationOrder);
        }

        public static TimeSeries operator +(TimeSeries timeseries, double scalar)
        {
            var newts = from td in timeseries
                        select new TSDataPoint<double>(td.Date, td.Value + scalar);

            return new TimeSeries(newts, timeseries.Name, timeseries.IntegrationOrder);
        }

        public static TimeSeries operator -(TimeSeries ts1, TimeSeries ts2)
        {
            var mints = from a in ts1
                        select new TSDataPoint<double>(a.Date, a.Value - ts2[a.Date]);

            return new TimeSeries(mints, "Difference between " + ts1.Name + " and " + ts2.Name, ts1.IntegrationOrder);
        }

        public static TimeSeries operator -(TimeSeries timeseries, double scalar)
        {
            var mints = from td in timeseries
                        select new TSDataPoint<double>(td.Date, td.Value - scalar);

            return new TimeSeries(mints, timeseries.Name, timeseries.IntegrationOrder);
        }

        public static TimeSeries operator *(TimeSeries timeseries, double scalar)
        {
            //add corresponding elements that match on time
            var mults = from ts in timeseries
                        select new TSDataPoint<double>(ts.Date, ts.Value / scalar);

            return new TimeSeries(mults, timeseries.Name, timeseries.IntegrationOrder);
        }

        public static TimeSeries operator /(TimeSeries timeseries, double scalar)
        {
            //add corresponding elements that match on time
            var divts = from ts in timeseries
                        select new TSDataPoint<double>(ts.Date, ts.Value / scalar);

            return new TimeSeries(divts, timeseries.Name, timeseries.IntegrationOrder);
        }
    }

}
