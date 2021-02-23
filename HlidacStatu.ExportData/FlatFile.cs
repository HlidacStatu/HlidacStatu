using System;
using System.Globalization;
using System.IO;
using System.Text;

using CsvHelper;

namespace HlidacStatu.ExportData
{
    public abstract class FlatFile : IExport
    {
        protected static CsvHelper.Configuration.CsvConfiguration DefaultConfig = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);

        CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
        public FlatFile(string delimiter, char quote)
        {
            config.Delimiter = delimiter;
            config.Quote = quote;
            config.Encoding = Encoding.UTF8;
        }
        public string Delimiter { get; set; }

        public byte[] ExportData(Data data)
        {

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    using (var csv = new CsvWriter(writer, config))
                    {
                        for (int c = 0; c < data.Columns.Count; c++)
                        {
                            csv.WriteField(data.Columns[c].Name, true);
                        }
                        csv.NextRecord();

                        for (int r = 0; r < data.Rows.Count; r++)
                        {
                            for (int c = 0; c < data.Columns.Count; c++)
                            {
                                var col = data.Columns[c];
                                if (data.Rows[r].Values.Count - 1 < c)
                                    continue; //skip nonexistent column

                                if (!string.IsNullOrWhiteSpace(col.FormatString))
                                {
                                    csv.WriteField(string.Format("{0:" + col.FormatString + "}", data.Rows[r].Values[c]));
                                }
                                else if (data.Rows[r].Values[c] == null)
                                {
                                    csv.WriteField("");
                                }
                                else
                                {
                                    // specific type
                                    //The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, 
                                    //UInt64, IntPtr, UIntPtr, Char, Double, and Single.
                                    if (Nullable.GetUnderlyingType(col.ValueType) != null)
                                        col.ValueType = Nullable.GetUnderlyingType(col.ValueType);

                                    if (col.ValueType.IsPrimitive
                                        || col.ValueType == typeof(decimal)
                                        || col.ValueType == typeof(string)
                                        || col.ValueType == typeof(DateTime)
                                        || col.ValueType == typeof(bool)
                                        )
                                    {

                                        if (col.ValueType == typeof(string))
                                        {
                                            csv.WriteField(data.Rows[r].Values[c].ConvertToFormatedString<string>());
                                        }
                                        else if (col.ValueType == typeof(int) || col.ValueType == typeof(long)
                                            || col.ValueType == typeof(short) || col.ValueType == typeof(byte)
                                            )
                                        {
                                            csv.WriteField(data.Rows[r].Values[c].ConvertToFormatedString<long>());
                                        }
                                        else if (col.ValueType == typeof(double))
                                        {
                                            csv.WriteField(data.Rows[r].Values[c].ConvertToFormatedString<double>());
                                        }
                                        else if (col.ValueType == typeof(float))
                                        {
                                            csv.WriteField(data.Rows[r].Values[c].ConvertToFormatedString<float>());
                                        }
                                        else if (col.ValueType == typeof(decimal))
                                        {
                                            csv.WriteField(data.Rows[r].Values[c].ConvertToFormatedString<float>());
                                        }
                                        else if (col.ValueType == typeof(bool))
                                        {
                                            csv.WriteField(data.Rows[r].Values[c].ConvertToFormatedString<bool>());
                                        }
                                        else if (col.ValueType == typeof(DateTime))
                                        {
                                            csv.WriteField(data.Rows[r].Values[c].ConvertToFormatedString<DateTime>());
                                        }
                                        else
                                        {
                                            csv.WriteField( data.Rows[r].Values[c].ToString());
                                        }
                                    } //types

                                }

                            }
                            csv.NextRecord();
                        }
                    }
                }
                return stream.ToArray();

            }
        }
    }
}
