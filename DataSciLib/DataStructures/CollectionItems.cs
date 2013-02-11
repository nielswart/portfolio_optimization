using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSciLib.DataStructures
{
    public class TableItem<Tidrow, Tidcol, Tval>
    {
        protected Tidrow _rowID;
        protected Tidcol _colID;
        protected Tval _value;

        public TableItem(Tidrow rowid, Tidcol colid, Tval value)
        {
            _rowID = rowid;
            _colID = colid;
            _value = value;
        }
    }

    /// <summary>
    /// An atomic value that uniquely identifies an entry in a 2 dimensional time series object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TimeSeriesItem<Tval> : TableItem<DateTime, string, Tval>
    {
        public DateTime Date
        {
            get { return _rowID; }
        }
        public Tval Value
        {
            get { return _value; }
        }
        public string SeriesName
        {
            get { return _colID; }
        }

        public TimeSeriesItem(DateTime date, string name, Tval value)
            : base(date, name, value)
        {
        }
    }

    public class ArrayItem<Tid, Tval>
    {
        protected Tid _id;
        protected Tval _value;

        public ArrayItem(Tid id, Tval value)
        {
            _id = id;
            _value = value;
        }
    }

    /// <summary>
    /// An atomic value that uniquely identifies an entry in a 1 dimensional time series object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TimeItem<Tval> : ArrayItem<DateTime, Tval>
    {
        public DateTime Date
        {
            get { return _id; }
        }
        public Tval Value
        {
            get { return _value; }
        }

        public TimeItem(DateTime date, Tval value)
            : base(date, value)
        {
            _id = date;
            _value = value;
        }
    }
}
