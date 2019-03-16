using System;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Options;

namespace DotNet.Highcharts.Helpers
{
    public class Data
    {
        public Data(object[] data) { ArrayData = data; }

        public Data(object[,] data) { DoubleArrayData = data; }

        public Data(Point[] data) { SeriesData = data; }

        public Data(Flag[] data) { FlagData = data; }

        [Name("data")]
        public object[] ArrayData { get; private set; }

        [Name("data")]
        public object[,] DoubleArrayData { get; private set; }

        [Name("data")]
        public Point[] SeriesData { get; private set; }

        [Name("data")]
        public Flag[] FlagData { get; private set; }
    }
}