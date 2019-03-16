using System.Globalization;

namespace DotNet.Highcharts.Helpers
{
    public struct StringNumber
    {
        private readonly Number? _heightInPixels;
        private readonly string _heightInPercent;

        public StringNumber(string percent)
        {
            _heightInPercent = percent;
            _heightInPixels = null;
        }

        public StringNumber(Number number)
        {
            _heightInPixels = number;
            _heightInPercent = null;
        }

        public static implicit operator StringNumber(string value) { return new StringNumber(value); }
        public static implicit operator StringNumber(Number value) { return new StringNumber(value); }
        public static implicit operator StringNumber(int value) { return new StringNumber(value); }
        public static implicit operator StringNumber(double value) { return new StringNumber(value); }

        public static StringNumber GetHeight(object o)
        {
            switch (o)
            {
                case string _:
                    return new StringNumber((string)o);
                case Number _:
                    return new StringNumber((Number)o);
            }

            return new StringNumber();
        }

        public static implicit operator string(StringNumber a)
        {
            if (a._heightInPercent != null)
                return a._heightInPercent;

            return a._heightInPixels.HasValue ?
                a._heightInPixels.ToString() : null;
        }

        public static implicit operator Number? (StringNumber a)
        {
            if (a._heightInPixels.HasValue)
                return a._heightInPixels;

            return a._heightInPercent != null ? a : null;
        }

        public override string ToString()
        {
            if (_heightInPixels.HasValue)
                return _heightInPixels.ToString();

            return _heightInPercent?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
