using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSciLib.DataStructures.TimeSeries
{
    public interface IMultiTimeSeries<T> : ITimeSeries<T> 
    {
        ITimeSeries<T> this[string name] { get; }

        ITimeSeries<T> this[string[] names] { get; }

        string[] Names { get; }

        T this[DateTime date, string name] { get; }

        ITimeSeries<T> this[DateTime[] dates, string[] names] { get; }

        void ColumnBind(ITimeSeries<T> timeseries);

        int ColumnCount { get; }
    }

    public class MultiTimeSeries<T> : IMultiTimeSeries<T>, IEnumerable<TimeSeriesItem<T>> 
    {

    }
}
