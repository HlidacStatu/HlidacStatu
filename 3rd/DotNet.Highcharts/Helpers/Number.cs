using System;
using System.Globalization;

namespace DotNet.Highcharts.Helpers
{
    public struct Number
    {
        double? _DoubleNumber;
        int? _IntNumber;

        Number(double value)
        {
            _DoubleNumber = value;
            _IntNumber = null;
        }

        Number(int value)
        {
            _IntNumber = value;
            _DoubleNumber = null;
        }

        public static implicit operator Number(double value) { return new Number(value); }
        public static implicit operator Number(int value) { return new Number(value); }

        public static Number GetNumber(object o)
        {
            if (o is int)
                return new Number((int)o);
            if (o is double)
                return new Number((double)o);
            return new Number();
        }

        public static implicit operator int?(Number a)
        {
            if (a._IntNumber.HasValue)
                return a._IntNumber.Value;
            if (a._DoubleNumber.HasValue)
                return Convert.ToInt32(a._DoubleNumber.Value);
            return null;
        }

        public static implicit operator double?(Number a)
        {
            if (a._DoubleNumber.HasValue)
                return a._DoubleNumber;
            if (a._IntNumber.HasValue)
                return Convert.ToDouble(a._IntNumber.Value);
            return null;
        }

        public override string ToString()
        {
            if (_DoubleNumber.HasValue)
                return _DoubleNumber.Value.ToString(CultureInfo.InvariantCulture);
            if (_IntNumber.HasValue)
                return _IntNumber.Value.ToString(CultureInfo.InvariantCulture);
            return string.Empty;
        }
    }
}