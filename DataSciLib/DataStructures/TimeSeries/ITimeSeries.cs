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
    public interface ITimeSeries<T> 
    {
        double[] Data;

        DateTime[] DateTime { get; }

        string Name;

        ITimeSeries<T> this[DateTime date] { get; }

        ITimeSeries<T> this[DateTime[] dates] { get; }

        void RowBind(ITimeSeries<T> timeseries);

        int RowCount { get; }

    }
}
