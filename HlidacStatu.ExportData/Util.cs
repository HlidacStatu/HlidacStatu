using System;
using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.ExportData
{
    public static class Util
    {
        public static bool TryCast<T>(this object obj, out T result)
        {
            if (obj is T)
            {
                result = (T)obj;
                return true;
            }

            result = default(T);
            return false;
        }
        public static string ConvertToFormatedString<T>(this object obj)
        {
            if (obj.TryConvert<T>(out object res))
            {
                if (typeof(T) == typeof(string))
                {
                    return res.ToString();
                }
                else if (typeof(T) == typeof(int) || typeof(T) == typeof(long)
                    || typeof(T) == typeof(short) || typeof(T) == typeof(byte)
                    )
                {
                    return ((long)res).ToString("0");
                }
                else if (typeof(T) == typeof(double))
                {
                    return ((double)res).ToString("0.000");
                }
                else if (typeof(T) == typeof(float))
                {
                    return ((float)res).ToString("0.000");
                }
                else if (typeof(T) == typeof(decimal))
                {
                    return ((decimal)res).ToString("0.000");
                }
                else if (typeof(T) == typeof(bool))
                {
                    return ((bool)res) ? "true" : "false";
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    DateTime val = ((DateTime)res);
                    if (val.Hour == 0 && val.Minute == 0 && val.Second == 0 && val.Millisecond == 0)
                    {

                        return ((DateTime)res).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        return ((DateTime)res).ToString("yyyy-MM-dd HH:mm:ss:ffff");
                    }
                }
                else
                {
                    return res.ToString();
                }
            }
            else
                return obj.ToString();

        }
        public static bool TryConvert<T>(this object obj, out object result)
        {
            result=default(T);

            if (typeof(T) == typeof(string))
            {
                result= obj.ToString();
                return true;
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long)
                || typeof(T) == typeof(short) || typeof(T) == typeof(byte)
                )
            {
                if (obj.TryCast<long>(out long xvar0))
                {
                    result = xvar0;
                    return true;
                }
                else if (long.TryParse(obj?.ToString(), out long xvar))
                {
                    result = xvar;
                    return true;
                }
                else
                    return false;

            }
            else if (typeof(T) == typeof(double))
            {
                if (obj.TryCast<double>(out double xvar0))
                {
                    result = xvar0;
                    return true;
                }
                else if (double.TryParse(obj?.ToString(), out double xvar))
                {
                    result = xvar;
                    return true;
                }
                else
                    return false;
            }
            else if (typeof(T) == typeof(float))
            {
                if (obj.TryCast<float>(out float xvar0))
                {
                    result = xvar0;
                    return true;
                }
                else if (float.TryParse(obj?.ToString(), out float xvar))
                {
                    result = xvar;
                    return true;
                }
                else
                    return false;
            }
            else if (typeof(T) == typeof(decimal))
            {
                if (obj.TryCast<decimal>(out decimal xvar0))
                {
                    result = xvar0;
                    return true;
                }
                else if (decimal.TryParse(obj?.ToString(), out decimal xvar))
                {
                    result = xvar;
                    return true;
                }
                else
                    return false;
            }
            else if (typeof(T) == typeof(bool))
            {
                if (obj.TryCast<bool>(out bool xvar0))
                {
                    result = xvar0;
                    return true;
                }
                else if (bool.TryParse(obj?.ToString(), out bool xvar))
                {
                    result = xvar;
                    return true;
                }
                else
                    return false;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                DateTime val;
                if (obj.TryCast<DateTime>(out DateTime xvar0))
                {
                    result = xvar0;
                    return true;
                }
                else if (Devmasters.DT.Util.ToDateTime(obj.ToString()) != null)
                {
                    result = xvar0;
                    return true;
                }
                else
                    return false;

            }
            else
            {
                return false;
            }
        }
    }
}
