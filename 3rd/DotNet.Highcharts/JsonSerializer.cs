using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using DotNet.Highcharts.Attributes;
using DotNet.Highcharts.Helpers;
using System.Globalization;
using DotNet.Highcharts.Enums;

namespace DotNet.Highcharts
{
    public class JsonSerializer
    {
        const string JSON_NUMBER_ARRAY = "[{0}]";
        const string JSON_OBJECT_FORMAT = "{{ {0} }}";
        const string JSON_PROPERTY_WITH_VALUE_FORMAT = "{0}: {1}";
        const string JSON_STRING_FORMAT = "'{0}'";
        const string JSON_DEFAULT_FORMAT = "{0}";
        const string JSON_DATE_FORMAT = "Date.parse('{0}')";
        const string NULL_STRING = "null";


        public static string Serialize<T>(T obj) where T : class { return Serialize(obj, true); }

        public static string Serialize<T>(T obj, bool appendCurlyBrackets) where T : class
        {
            if (obj is Array)
                return GetJsonArray(obj as Array, true);

            return GetJsonObject(obj, appendCurlyBrackets);
        }


        static string GetJsonArray(Array obj, bool useCurlyBracketsForObject)
        {
            if (obj is string[])
            {
                StringBuilder values = new StringBuilder();
                foreach (object value in obj)
                {
                    values.Append(GetJsonString(string.Empty, JSON_STRING_FORMAT, value.ToString()) + ", ");
                }
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, (values.Length > 2) ? values.ToString(0, values.Length - 2) : values.ToString());
            }
            if (obj is int[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", (obj as int[])));
            if (obj is int?[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from int? item in obj select GetJsonObject(item, false)));
            if (obj is short[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", (obj as short[])));
            if (obj is short?[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from short? item in obj select GetJsonObject(item, false)));
            if (obj is long[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", (obj as long[])));
            if (obj is long?[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from long? item in obj select GetJsonObject(item, false)));
            if (obj is double[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", (obj as double[]).Select(p => p.ToString(CultureInfo.InvariantCulture))));
            if (obj is double?[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from double? item in obj select GetJsonObject(item, false)));
            if (obj is decimal[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", (obj as decimal[]).Select(p => p.ToString(CultureInfo.InvariantCulture))));
            if (obj is decimal?[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from decimal? item in obj select GetJsonObject(item, false)));
            if (obj is Number[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", (obj as Number[])));
            if (obj is Number[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from Number? item in obj select GetJsonObject(item, false)));
            if (obj is PercentageOrPixel[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from object item in obj select GetJsonObject((item as PercentageOrPixel).Value, false)));
            if (obj is bool[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from object item in obj select GetJsonObject(item, true)));
            if (obj is Color[]) 
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from Color item in obj select GetColorString(JSON_STRING_FORMAT, item))); 
            if (obj is object[])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, String.Join(", ", from object item in obj select GetJsonObject(item, true)));
            if (obj is object[,])
                return string.Format(useCurlyBracketsForObject ? JSON_NUMBER_ARRAY : JSON_DEFAULT_FORMAT, GetMultiDimentionArray(obj as object[,]));

            throw new NotImplementedException("Not implemented serialization array of type: " + obj.GetType());
        }

        static string GetJsonObject(object obj, bool appendCurlyBrackets)
        {
            if (obj == null)
                return NULL_STRING;
            if (obj.GetType().IsPrimitive || obj is decimal || obj is String || obj is Color || obj is DateTime)
            {
                return GetValue(obj, obj.GetType(), GetJsonFormatter(null));
            }

            if (obj is Array)
                return GetJsonArray(obj as Array, appendCurlyBrackets);

            // Handle generic dictionaries with self defined properties. string: data.
            if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
                obj.GetType().GetGenericArguments()[0] == typeof(string))
            {
                List<string> jsonEntry = new List<string>();
                JsonFormatter form = GetJsonFormatter(null);

                // For each dictionary entry prepare it as a property to be added
                foreach (KeyValuePair<string, object> entry in (Dictionary<string, object>)obj)
                {
                    string value = GetValue(entry.Value, entry.Value.GetType(), form);
                    jsonEntry.Add(form.AddPropertyName ? string.Format(JSON_PROPERTY_WITH_VALUE_FORMAT, GetFirstLetterLower(entry.Key), value) : value);
                }

                // End formatting and return the result.
                return appendCurlyBrackets ? string.Format(JSON_OBJECT_FORMAT, string.Join(", ", jsonEntry)) : string.Join(", ", jsonEntry);
            }

            List<string> json = new List<string>();

            foreach (PropertyInfo property in obj.GetType().GetMembers().Where(x => x.MemberType == MemberTypes.Property))
            {
                object propertyValue = property.GetValue(obj, null);
                JsonFormatter formatter = GetJsonFormatter(property);

                if (propertyValue != null)
                {
                    Type propertyType = property.PropertyType;
                    string propertyName = GetPropertyName(property);

                    string value;
                    if (propertyType == typeof(Array) || propertyType.BaseType == typeof(Array) || propertyValue is Array)
                        value = GetJsonArray(propertyValue as Array, formatter.UseCurlyBracketsForObject);
                    else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        value = GetValue(propertyValue, propertyType.GetGenericArguments()[0], formatter);
                    else
                        value = GetValue(propertyValue, propertyType, formatter);

                    json.Add(formatter.AddPropertyName ? string.Format(JSON_PROPERTY_WITH_VALUE_FORMAT, GetFirstLetterLower(propertyName), value) : value);
                }
            }
            return appendCurlyBrackets ? string.Format(JSON_OBJECT_FORMAT, string.Join(", ", json)) : string.Join(", ", json);
        }

        static string GetValue(object value, Type type, JsonFormatter formatter)
        {
            if (value is string)
            {
                string jsonFormat = JSON_STRING_FORMAT;
                if (IsNumeric(value))
                    jsonFormat = JSON_DEFAULT_FORMAT;
                return GetJsonString(formatter.JsonValueFormat, jsonFormat, value.ToString().Replace("\r\n", " ").Replace("  ", ""));
            }
            if (value is int || value is long || value is short || value is byte || type == typeof(Number) || type == typeof(StringNumber) || type == typeof(StringBool))
                return GetJsonString(formatter.JsonValueFormat, JSON_DEFAULT_FORMAT, value.ToString());
            if (value is double)
                return GetJsonString(formatter.JsonValueFormat, JSON_DEFAULT_FORMAT, ((double)value).ToString(CultureInfo.InvariantCulture));
            if (value is float)
                return GetJsonString(formatter.JsonValueFormat, JSON_DEFAULT_FORMAT, ((float)value).ToString(CultureInfo.InvariantCulture));
            if (value is decimal)
                return GetJsonString(formatter.JsonValueFormat, JSON_DEFAULT_FORMAT, ((decimal)value).ToString(CultureInfo.InvariantCulture));
            if (value is bool)
                return GetJsonString(formatter.JsonValueFormat, JSON_DEFAULT_FORMAT, value.ToString().ToLowerInvariant());
            if (value is Color)
                return GetColorString(formatter.JsonValueFormat, value);
            if (value is DateTime)
                return GetJsonString(formatter.JsonValueFormat, JSON_DATE_FORMAT, ((DateTime)value).ToString(CultureInfo.InvariantCulture));
            if (type.IsEnum)
                return GetEnumString(formatter.JsonValueFormat, type, value);
            if ((type.BaseType != null && type.BaseType == typeof(object)) || type.IsClass)
                return GetJsonString(formatter.JsonValueFormat, JSON_DEFAULT_FORMAT, GetJsonObject(value, formatter.UseCurlyBracketsForObject));

            throw new NotImplementedException("Not implemented serialization for type: " + type.Name);
        }

        private static bool IsNumeric(object expression)
        {
            if (expression == null)
                return false;

            double number;
            return Double.TryParse(Convert.ToString(expression, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);
        }

        private static string GetEnumString(string format, Type type, object value)
        {
            if (type == typeof(WeekDays))
                return GetJsonString(format, JSON_DEFAULT_FORMAT, ((int)value).ToString());

            return GetJsonString(format, JSON_STRING_FORMAT, GetFirstLetterLower(value.ToString().Replace("_", "-")));
        }

        private static string GetColorString(string format, object obj)
        {
            Color color = (Color)obj;
            if (color.IsNamedColor)
                return color.IsKnownColor ? GetJsonString(format, JSON_STRING_FORMAT, color.Name.ToLower()) : GetJsonString(format, JSON_DEFAULT_FORMAT, color.Name);
            if (color.A == 255)
                return GetJsonString(format, JSON_STRING_FORMAT, ColorTranslator.ToHtml(color));
            return GetJsonString(format, JSON_STRING_FORMAT, GetRgbColor(color));
        }

        static string GetRgbColor(Color color)
        {
            double htmlAlpha = (double)color.A / 255;
            return string.Format("rgba({0}, {1}, {2}, {3})", color.R, color.G, color.B, htmlAlpha.ToString("#.#", CultureInfo.InvariantCulture));
        }

        static string GetJsonString(string format, string defaultFormat, string value)
        {
            string usedFormat = (!string.IsNullOrEmpty(format)) ? format : defaultFormat;

            // Check if the format includes quotes, so needing to escape the string.
            if (usedFormat.Contains("'{0}'"))
            {
                // In this case escape the quotes in the string.
                value = value.Replace("'", "\\'");
            }

            return string.Format(usedFormat, value);
        }

        static string GetPropertyName(PropertyInfo property)
        {
            string propertyName = String.Empty;
            foreach (NameAttribute attribute in property.GetCustomAttributes(typeof(NameAttribute), true))
                propertyName = attribute.Name;

            if (String.IsNullOrEmpty(propertyName))
                propertyName = property.Name;

            return propertyName;
        }

        static string GetFirstLetterLower(string name) { return Char.ToLowerInvariant(name[0]) + name.Substring(1); }

        static JsonFormatter GetJsonFormatter(PropertyInfo property)
        {
            JsonFormatter formatter = new JsonFormatter { JsonValueFormat = string.Empty, AddPropertyName = true, UseCurlyBracketsForObject = true };
            if (property != null)
                foreach (JsonFormatterAttribute attribute in property.GetCustomAttributes(typeof(JsonFormatterAttribute), true))
                    formatter = attribute.JsonFormatter;

            return formatter;
        }

        static string GetMultiDimentionArray(object[,] array)
        {
            var arrays = new List<string>();
            for (var x = 0; x < array.GetLength(0); x++)
            {
                var rowList = new List<string>();
                var sb = new StringBuilder();

                sb.Append("[");

                for (var y = 0; y < array.GetLength(1); y++)
                {
                    rowList.Add(GetJsonObject(array[x, y], true));
                }

                sb.Append(string.Join(", ", rowList));
                sb.Append("]");
                arrays.Add(sb.ToString());
            }

            return string.Join(", ", arrays);
        }
    }
}
