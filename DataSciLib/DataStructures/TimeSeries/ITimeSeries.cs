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
    public interface ITimeSeries<T> : IEnumerable<TimeDataPoint<T>> 
    {
        uint IntegrationOrder { get; }

        T[] Data { get; }

        DateTime[] DateTime { get; }

        string Name { get; }

        T this[DateTime date] { get; }

        ITimeSeries<T> this[IEnumerable<DateTime> dates] { get; }

        int RowCount { get; }

        DataFrequency Frequency { get; }
    }
}
