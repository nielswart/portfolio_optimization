using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSciLib.DataStructures
{
    public interface IMultiTimeSeries<T> : IEnumerable<TimeSeriesItem<T>>
    {
        ITimeSeries<T> this[string name] { get; }

        ITimeSeries<T> this[string[] names] { get; }

        string[] Names { get; }

        T this[DateTime date, string name] { get; }

        ITimeSeries<T> this[DateTime[] dates, string[] names] { get; }

        int ColumnCount { get; }
    }

    /*
    public class MultiTimeSeries<T> : IMultiTimeSeries<T>
    {
    
     * public ITimeSeries<T> this[string name]
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
     * public T this[DateTime date, string name]
        {
            get
            {
                return TimeSeriesFactory<T>.Create(_timeDataDict[date], date, this.Names)[name].DataMatrix[0, 0];
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
     * 
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
    }
     * */
}
