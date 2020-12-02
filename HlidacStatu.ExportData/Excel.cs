using System;
using System.Net.Mail;

using OfficeOpenXml;

namespace HlidacStatu.ExportData
{
    public class Excel : IExport
    {
        public const string CurrencyFormat = "#,##0.00\\ \"Kč\"";

        public byte[] ExportData(Data data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add("Data");
                //Header
                for (int c = 0; c < data.Columns.Count; c++)
                {
                    ws.Cells[1, c + 1].Value = data.Columns[c].Name;
                    ws.Cells[1, c + 1].Style.Font.Bold = true;

                }
                for (int r = 0; r < data.Rows.Count; r++)
                {
                    for (int c = 0; c < data.Columns.Count; c++)
                    {
                        var col = data.Columns[c];
                        if (data.Rows[r].Values.Count - 1 < c)
                            continue; //skip nonexistent column

                        if (!string.IsNullOrWhiteSpace(col.FormatString))
                        {
                            ws.Cells[r + 2, c + 1].Value = string.Format("{0:" + col.FormatString + "}", data.Rows[r].Values[c]);
                            ws.Cells[r + 2, c + 1].Style.Numberformat.Format = col.FormatString;
                        }
                        else if (data.Rows[r].Values[c] == null)
                        {
                            ws.Cells[r + 2, c + 1].Value = null;
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
                                    ws.Cells[r + 2, c + 1].Value = data.Rows[r].Values[c].ToString();
                                }
                                else if (col.ValueType == typeof(int) || col.ValueType == typeof(long)
                                    || col.ValueType == typeof(short) || col.ValueType == typeof(byte)
                                    )
                                {
                                    ws.Cells[r + 2, c + 1].Value = (long)data.Rows[r].Values[c];
                                    ws.Cells[r + 2, c + 1].Style.Numberformat.Format = "#,##0";
                                }
                                else if (col.ValueType == typeof(double))
                                {
                                    ws.Cells[r + 2, c + 1].Value = ((double)data.Rows[r].Values[c]);
                                    ws.Cells[r + 2, c + 1].Style.Numberformat.Format = "#,##0.00";
                                }
                                else if (col.ValueType == typeof(float))
                                {
                                    ws.Cells[r + 2, c + 1].Value = ((float)data.Rows[r].Values[c]);
                                    ws.Cells[r + 2, c + 1].Style.Numberformat.Format = "#,##0.00";
                                }
                                else if (col.ValueType == typeof(decimal))
                                {
                                    ws.Cells[r + 2, c + 1].Value = ((decimal)data.Rows[r].Values[c]);
                                    ws.Cells[r + 2, c + 1].Style.Numberformat.Format = "#,##0.00";
                                }
                                else if (col.ValueType == typeof(bool))
                                {
                                    ws.Cells[r + 2, c + 1].Value = ((bool)data.Rows[r].Values[c]) ? 1 : 0;
                                }
                                else if (col.ValueType == typeof(DateTime))
                                {
                                    DateTime val = ((DateTime)data.Rows[r].Values[c]);
                                    if (val.Hour == 0 && val.Minute == 0 && val.Second == 0 && val.Millisecond == 0)
                                    {

                                        ws.Cells[r + 2, c + 1].Value = ((DateTime)data.Rows[r].Values[c]);
                                        ws.Cells[r + 2, c + 1].Style.Numberformat.Format = "mm-dd-yy";

                                    }
                                    else
                                    {
                                        ws.Cells[r + 2, c + 1].Value = ((DateTime)data.Rows[r].Values[c]);
                                        ws.Cells[r + 2, c + 1].Style.Numberformat.Format = "m/d/yy h:mm";
                                    }
                                }
                                else
                                {
                                    ws.Cells[r + 2, c + 1].Value = data.Rows[r].Values[c].ToString();
                                }
                            }

                        }
                    }
                }

                ws.Cells[1, 1, 1, data.Columns.Count].AutoFilter = true;

                ws.Calculate();
                ws.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                ws.View.FreezePanes(2, 1);
                p.Workbook.Properties.Title = "Export dat z www.hlidacstatu.cz";
                p.Workbook.Properties.Author = "Hlídač státu z.ú.";
                p.Workbook.Properties.Comments = "Export dat z www.hlidacstatu.cz pod licencí www.hlidacstatu.cz/texty/licence";

                // Set some extended property values
                p.Workbook.Properties.Company = "Hlídač státu z.ú.";


                return p.GetAsByteArray();
            }

        }

    }


}
