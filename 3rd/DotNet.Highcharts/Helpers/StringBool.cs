using System.Globalization;

namespace DotNet.Highcharts.Helpers
{
    public struct StringBool
    {
        private readonly bool? _bool;
        private readonly string _string;

        public StringBool(string str)
        {
            _string = str;
            _bool = null;
        }

        public StringBool(bool boolean)
        {
            _bool = boolean;
            _string = null;
        }

        public static implicit operator StringBool(string value) { return new StringBool(value); }
        public static implicit operator StringBool(bool value) { return new StringBool(value); }

        public static StringBool GetValue(object o)
        {
            switch (o)
            {
                case string _:
                    return new StringBool((string)o);
                case Number _:
                    return new StringBool((bool)o);
            }

            return new StringBool();
        }

        public static implicit operator string(StringBool a)
        {
            if (a._string != null)
                return a._string;

            return a._bool.HasValue ?
                a._bool.ToString() : null;
        }

        public static implicit operator bool? (StringBool a)
        {
            if (a._bool.HasValue)
                return a._bool;

            return a._string != null ? a : null;
        }

        public override string ToString()
        {
            if (_bool.HasValue)
                return _bool.ToString();

            return _string?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
