// Copyright (c) 2012: DJ Swart, AJ Hoffman

using System;
using System.Collections.Generic;

namespace DataSciLib.DataStructures
{
    // TODO: Inherit IEnumerable Interface to implement iterator

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITimeSeries<T> : IEnumerable<TimeSeriesItem<T>> 
    {
        T[,] DataMatrix { get; }  

        DateTime[] DateTime { get; }

        string[] Names { get; }

        ITimeSeries<T> this[string name] { get; }

        ITimeSeries<T> this[string[] names] { get; }

        ITimeSeries<T> this[DateTime date] { get; }

        ITimeSeries<T> this[DateTime[] dates] { get; }

        T this[DateTime date, string name] { get; }

        ITimeSeries<T> this[DateTime[] dates, string[] names] { get; }

        void RowBind(ITimeSeries<T> timeseries);

        void ColumnBind(ITimeSeries<T> timeseries);

        int RowCount { get; }

        int ColumnCount { get; }

    }
}
